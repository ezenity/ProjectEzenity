pipeline {
  agent any

  options {
    disableConcurrentBuilds()
    timestamps()
  }

  environment {
    DEPLOY_DIR = '/srv/ezenity/apps/project-ezenity'
    // This file should contain your EZENITY_* variables (DB, SMTP, AllowedOrigins, etc.)
    // Create it in Jenkins: Manage Jenkins -> Managed files -> add "Secret file"
    // Put its fileId here:
    ENV_FILE_ID = 'REPLACE_WITH_YOUR_MANAGED_FILE_ID'
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

    stage('Docker: build images (tagged)') {
      steps {
        sh '''#!/usr/bin/env bash
          set -euo pipefail
          export TAG="${TAG}"

          # Build all services declared in docker-compose.yml
          docker compose build --pull
        '''
      }
    }

    stage('Deploy (main only)') {
      when { branch 'main' }
      steps {
        // Pull env file from Jenkins and write it into DEPLOY_DIR as ".env"
        configFileProvider([configFile(fileId: "${ENV_FILE_ID}", variable: 'ENV_FILE')]) {
          sh '''#!/usr/bin/env bash
            set -euo pipefail

            mkdir -p "${DEPLOY_DIR}"

            # Deploy source to VPS folder (same machine Jenkins is on)
            if command -v rsync >/dev/null 2>&1; then
              rsync -av --delete ./ "${DEPLOY_DIR}/"
            else
              rm -rf "${DEPLOY_DIR:?}/"*
              tar -cf - . | (cd "${DEPLOY_DIR}" && tar -xf -)
            fi

            # Install the env file used by docker compose
            cp "$ENV_FILE" "${DEPLOY_DIR}/.env"
            chmod 600 "${DEPLOY_DIR}/.env"

            cd "${DEPLOY_DIR}"

            export TAG="${TAG}"

            # Start/update services
            docker compose down
            docker compose up -d --build

            docker compose ps
          '''
        }
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

          if docker compose config --services | grep -q '^api$'; then
            echo "Running EF migrations via docker compose run api..."
            docker compose run --rm ezenity_api bash -lc "dotnet ef database update --project Ezenity.Infrastructure --startup-project Ezenity.API"
          else
            echo "Skipping migrations: compose service 'api' not found."
            echo "Update service name in Jenkinsfile if needed."
          fi
        '''
      }
    }
  }

  post {
    always {
      sh '''#!/usr/bin/env bash
        set +e
        command -v docker >/dev/null 2>&1 && docker compose ps || true
      '''
      echo 'Pipeline finished.'
    }
  }
}
