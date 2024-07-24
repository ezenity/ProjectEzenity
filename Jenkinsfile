pipeline {
    agent { label 'ezenity-node' }

    environment {
        // Define environment variables here
        DOTNET_CORE_VERSION = '6.0'
        BUILD_CONFIGURATION = 'Release'
        PATH = "${env.PATH}:${env.HOME}/.dotnet/tools"
        ASPNETCORE_ENVIRONMENT = 'Production'
    }

    stages {
        stage('Preparation') {
            steps {
                configFileProvider([configFile(fileId: 'd09a8769-3d7e-426f-9345-585423b74ac9', variable: 'ENV_FILE')]) {
                    // Shell step to source the environment variables
                    sh 'set -a && . $ENV_FILE && set +a'
                }
                checkout scm
                // Optionally clean workspace
                sh 'dotnet tool restore'
            }
        }
        stage('Restore') {
            steps {
                echo 'Restoring project dependencies...'
                sh 'bash -c "source /etc/ezenity/api/prod/env_vars.sh && dotnet restore ProjectEzenity.sln"'
            }
        }
        stage('Build') {
            steps {
                echo 'Building the solution...'
                sh 'bash -c "source /etc/ezenity/api/prod/env_vars.sh && dotnet build ProjectEzenity.sln -c ${BUILD_CONFIGURATION} --no-restore"'
            }
        }
        stage('Test') {
            steps {
                echo 'Running unit tests...'
                sh 'bash -c "source /etc/ezenity/api/prod/env_vars.sh && dotnet test Ezenity.Tests/Ezenity.Tests.csproj -c ${BUILD_CONFIGURATION} --no-build --logger \"trx;LogFileName=test_results.trx\""'
                // TODO: Adding steps to publish test results for a plugin that supports it
            }
        }
        stage('Publish') {
            steps {
                echo 'Stopping application...'
                script {
                    sh '/usr/local/bin/stop-ezenity-api.sh'
                }
                echo 'Publishing application...'
                script {
                    sh 'bash -c "source /etc/ezenity/api/prod/env_vars.sh && dotnet publish Ezenity.API/Ezenity.API.csproj -c ${BUILD_CONFIGURATION} -o /var/www/ezenity_api"'
                }
            }
        }
        stage('Database Migration') {
            when {
                branch 'main'
            }
            steps {
                script {
                    try {
                        def pendingMigrations = sh(
                            script: '''
                            bash -c 'set -a; source /etc/ezenity/api/prod/env_vars.sh; set +a;
                            echo "Checking environment variables...";
                            printenv | grep -E "ASPNETCORE_ENVIRONMENT|DOTNET_PRINT_TELEMETRY|EZENITY_DATABASE_NAME|EZENITY_DATABASE_USER|EZENITY_DATABASE_PASSWORD|EZENITY_BASE_URL|EZENITY_SECRET_KEY|EZENITY_SMTP_USER|EZENITY_SMTP_PASSWORD|EZENITY_ALLOWED_ORIGINS";
                            dotnet ef migrations list --project Ezenity.Infrastructure --startup-project Ezenity.API | tail -n +3'
                            ''',
                            returnStdout: true
                        ).trim()
                        if (pendingMigrations && !pendingMigrations.isEmpty()) {
                            echo "Applying database migrations: ${pendingMigrations}"
                            sh '''
                            bash -c 'set -a; source /etc/ezenity/api/prod/env_vars.sh; set +a;
                            echo "Re-confirming environment variables before migration...";
                            printenv | grep -E "ASPNETCORE_ENVIRONMENT|DOTNET_PRINT_TELEMETRY|EZENITY_DATABASE_NAME|EZENITY_DATABASE_USER|EZENITY_DATABASE_PASSWORD|EZENITY_BASE_URL|EZENITY_SECRET_KEY|EZENITY_SMTP_USER|EZENITY_SMTP_PASSWORD|EZENITY_ALLOWED_ORIGINS";
                            dotnet ef database update --project Ezenity.Infrastructure --startup-project Ezenity.API'
                            '''
                        } else {
                            echo 'No pending database migrations to apply.'
                        }
                    } catch (Exception e) {
                        echo "Database migration failed: ${e.getMessage()}"
                        error "Failed to apply database migrations."
                    }
                }
            }
        }
        stage('Deploy') {
            when {
                branch 'main'
            }
            steps {
                echo 'Deploying application...'
                script {
                    sh '/usr/local/bin/start-ezenity-api.sh'
                }
            }
        }
    }

    post {
        always {
            echo 'Cleaning up...'
        
            // Removing unused Docker images and containers
            echo 'Removing unused Docker images and containers...'
            sh 'docker system prune -af || true' // The `|| true` ensures that the pipeline doesn't fail if this command does.

            // Clearing NuGet package cache
            echo 'Clearing NuGet package cache...'
            sh 'dotnet nuget locals all --clear || true' // Similarly, `|| true` is used for safe failure.
        }
        success {
            echo 'Build and deployment succeeded!'
        }
        failure {
            echo 'Build or deployment failed.'
        }
    }
}
