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

          if [ -d "Ezenity_Frontend" ]; then
            echo "Found Ezenity_Frontend"
          else
            echo "WARNING: No Ezenity_Frontend folder found"
          fi

          if [ -d "Ezenity_Trina" ]; then
            echo "Found Ezenity_Trina"
          else
            echo "WARNING: No Ezenity_Trina folder found"
          fi

          [ -f "docker-compose.yml" ] || echo "WARNING: docker-compose.yml not found at repo root (update Jenkinsfile if different)"
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

          cd /srv/ezenity/apps/project-ezenity   # <-- FIX A (THIS IS THE KEY)
          export TAG="${TAG}"

          # Build all services declared in docker-compose.yml (uses default .env if present in workspace; not required for build)
          docker compose build --pull
        '''
      }
    }

    stage('Deploy (main only)') {
      when { branch 'main' }
      steps {
        sh '''#!/usr/bin/env bash
          set -euo pipefail

          mkdir -p "${DEPLOY_DIR}"

          # Deploy source to VPS folder BUT preserve existing .env
          if command -v rsync >/dev/null 2>&1; then
            rsync -av --delete --exclude '.env' ./ "${DEPLOY_DIR}/"
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

          cd "${DEPLOY_DIR}"
          export TAG="${TAG}"

          docker compose down
          docker compose up -d --build
          docker compose ps
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

          # Option A (recommended): run EF migrations in the API container image.
          # This assumes your API image contains the EF tooling and can run "dotnet ef".
          # If it doesn't, tell me and I'll adjust to a dedicated "migrator" image.
          #
          # Also assumes your docker-compose has a service named "api" (change if different).

          if docker compose config --services | grep -q '^ezenity_migrator$'; then
            echo "Running EF migrations using migrator image..."
            docker compose --profile migrate run --rm ezenity_migrator
          else
            echo "Skipping migrations: compose service 'ezenity_migrator' not found."
            exit 1
          fi

        '''
      }
    }
  }

  post {
    always {
      sh '''#!/usr/bin/env bash
        set +e
        cd "${DEPLOY_DIR}" 2>/dev/null || true
        docker compose ps 2>/dev/null || true
      '''
      echo 'Pipeline finished.'
    }
  }
}
