pipeline {
  agent any

  options {
    disableConcurrentBuilds()
    timestamps()
  }

  environment {
    DEPLOY_DIR = '/srv/ezenity/apps/project-ezenity'
  }

  stages {
    stage('Checkout') {
      steps { checkout scm }
    }

    stage('Compute TAG') {
      steps {
        script {
          env.TAG = sh(script: "git rev-parse --short HEAD", returnStdout: true).trim()
          echo "TAG=${env.TAG}"
          echo "BRANCH_NAME=${env.BRANCH_NAME}"
        }
      }
    }

    stage('Tooling check') {
      steps {
        sh '''#!/usr/bin/env bash
          set -euo pipefail
          docker --version
          docker compose version
        '''
      }
    }

    // Optional: quick sanity checks for folder layout (helps catch wrong paths early)
    stage('Repo layout check') {
      steps {
        sh '''#!/usr/bin/env bash
          set -euo pipefail

          [ -f "ProjectEzenity.sln" ] || (echo "Missing ProjectEzenity.sln at repo root" && exit 1)
          [ -d "Ezenity.API" ] || (echo "Missing Ezenity.API folder" && exit 1)
          [ -f "docker-compose.yml" ] || (echo "Missing docker-compose.yml at repo root" && exit 1)

          if [ -d "Ezenity_Frontend" ]; then echo "Found Ezenity_Frontend"; else echo "WARNING: No Ezenity_Frontend folder found"; fi
          if [ -d "Ezenity_Trina" ]; then echo "Found Ezenity_Trina"; else echo "WARNING: No Ezenity_Trina folder found"; fi
        '''
      }
    }

    stage('Unit Tests (.NET)') {
      steps {
        sh '''#!/usr/bin/env bash
          set -euo pipefail

          # Run the Dockerfile test stage only (fast fail).
          # Uses repo root as context; dockerfile is in Ezenity.API/.
          docker build \
            --pull \
            -f Ezenity.API/Dockerfile \
            --target test \
            -t ezenity/project-ezenity-tests:${TAG} \
            .
        '''
      }
    }

    stage('Docker: build images (tagged)') {
      steps {
        sh '''#!/usr/bin/env bash
          set -euo pipefail

          echo "WORKSPACE=$WORKSPACE"
          cd "$WORKSPACE"

          export TAG="${TAG}"

          # Use server .env if it exists (main branch deploy server), otherwise build without it
          if [ -f "${DEPLOY_DIR}/.env" ]; then
            echo "Using env file: ${DEPLOY_DIR}/.env"
            docker compose --env-file "${DEPLOY_DIR}/.env" build --pull
          else
            echo "WARNING: ${DEPLOY_DIR}/.env not found yet; building without it."
            docker compose build --pull
          fi
        '''
      }
    }

    stage('Deploy (main only)') {
      when { branch 'main' }
      steps {
        sh '''#!/usr/bin/env bash
          set -euo pipefail

          mkdir -p "${DEPLOY_DIR}"

          echo "Deploying repo contents to ${DEPLOY_DIR} (preserving .env)..."

          # IMPORTANT:
          # If /srv/ezenity/apps/project-ezenity is mounted read-only or has immutable attributes,
          # rsync will fail with "Read-only file system (30)".
          # We detect that and fail with a clear message, OR (optional) skip syncing and just deploy images.
          #
          # This implementation:
          # - tries rsync
          # - if it fails due to RO filesystem, it prints a clear error and stops

          if command -v rsync >/dev/null 2>&1; then
            set +e
            rsync -av --delete --exclude '.env' ./ "${DEPLOY_DIR}/"
            RSYNC_RC=$?
            set -e

            if [ $RSYNC_RC -ne 0 ]; then
              echo "ERROR: rsync failed with exit code $RSYNC_RC"
              echo "If you see 'Read-only file system (30)', your ${DEPLOY_DIR} is not writable."
              echo "Fix by ensuring the filesystem/mount permissions allow writes, or change DEPLOY_DIR."
              exit $RSYNC_RC
            fi
          else
            # fallback without rsync: keep .env if it exists
            if [ -f "${DEPLOY_DIR}/.env" ]; then
              cp "${DEPLOY_DIR}/.env" /tmp/project-ezenity.env.backup
            fi

            rm -rf "${DEPLOY_DIR:?}/"*
            tar -cf - . | (cd "${DEPLOY_DIR}" && tar -xf -)

            if [ -f /tmp/project-ezenity.env.backup ]; then
              cp /tmp/project-ezenity.env.backup "${DEPLOY_DIR}/.env"
              rm -f /tmp/project-ezenity.env.backup
            fi
          fi

          # Ensure .env exists on VPS
          if [ ! -f "${DEPLOY_DIR}/.env" ]; then
            echo "ERROR: ${DEPLOY_DIR}/.env not found. Create it on the VPS first."
            exit 1
          fi

          echo "Validating required .env keys exist (not empty)..."
          # Minimal required set - add more if you want the pipeline to enforce them
          REQUIRED_KEYS=(
            EZENITY_DB_CONN
            EZENITY_SECRET_KEY
          )

          for key in "${REQUIRED_KEYS[@]}"; do
            if ! grep -qE "^${key}=" "${DEPLOY_DIR}/.env"; then
              echo "ERROR: Missing ${key} in ${DEPLOY_DIR}/.env"
              exit 1
            fi
            val="$(grep -E "^${key}=" "${DEPLOY_DIR}/.env" | head -n1 | cut -d= -f2-)"
            if [ -z "${val}" ]; then
              echo "ERROR: ${key} is present but empty in ${DEPLOY_DIR}/.env"
              exit 1
            fi
          done

          cd "${DEPLOY_DIR}"
          export TAG="${TAG}"

          echo "Stopping existing stack..."
          docker compose --env-file "${DEPLOY_DIR}/.env" down || true

          echo "NOTE: Not starting stack here yet. Migrations will run first in next stage."
        '''
      }
    }

    stage('DB Migrations (main only)') {
      when { branch 'main' }
      steps {
        sh '''#!/usr/bin/env bash
          set -euo pipefail

          cd "${DEPLOY_DIR}"
          export TAG="${TAG}"

          echo "Checking that migrator exists in compose (profile: migrate)..."
          if docker compose --env-file "${DEPLOY_DIR}/.env" --profile migrate config --services | grep -qx "ezenity_migrator"; then
            echo "Running EF migrations..."
            docker compose --env-file "${DEPLOY_DIR}/.env" --profile migrate run --rm ezenity_migrator
            echo "Migrations completed."
          else
            echo "ERROR: service 'ezenity_migrator' not found even with profile 'migrate'."
            echo "Dumping services for debugging:"
            docker compose --env-file "${DEPLOY_DIR}/.env" --profile migrate config --services || true
            exit 1
          fi
        '''
      }
    }

    stage('Start Stack (main only)') {
      when { branch 'main' }
      steps {
        sh '''#!/usr/bin/env bash
          set -euo pipefail

          cd "${DEPLOY_DIR}"
          export TAG="${TAG}"

          echo "Starting stack..."
          docker compose --env-file "${DEPLOY_DIR}/.env" up -d

          echo "Current stack:"
          docker compose --env-file "${DEPLOY_DIR}/.env" ps

          echo "Recent API logs (last 80 lines):"
          docker compose --env-file "${DEPLOY_DIR}/.env" logs --tail=80 ezenity_api || true
        '''
      }
    }
  }

  post {
    always {
      sh '''#!/usr/bin/env bash
        set +e
        cd "${DEPLOY_DIR}" 2>/dev/null || true
        docker compose --env-file "${DEPLOY_DIR}/.env" ps 2>/dev/null || true
      '''
      echo 'Pipeline finished.'
    }
  }
}
