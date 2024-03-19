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
                // Optional: Run migrations only on specific branches, such as 'main' or 'release'.
                // You can remove this if you want to apply migrations on every branch.
                branch 'main'
            }
            steps {
                script {
                    // Ensure that the migration step has proper error handling.
                    try {
                        // This command checks for any pending migrations.
                        def pendingMigrations = sh(
                            script: 'bash -c "source /etc/ezenity/api/prod/env_vars.sh && dotnet ef migrations list --project Ezenity.Infrastructure --startup-project Ezenity.API | tail -n +3"',
                            returnStdout: true
                        ).trim()
                        if (pendingMigrations && !pendingMigrations.isEmpty()) {
                            echo "Applying database migrations: ${pendingMigrations}"
                            // Apply the pending migrations.
                            sh 'bash -c "source /etc/ezenity/api/prod/env_vars.sh && dotnet ef database update --project Ezenity.Infrastructure --startup-project Ezenity.API"'
                        } else {
                            echo 'No pending database migrations to apply.'
                        }
                    } catch (Exception e) {
                        // Log the error and fail the build if migrations cannot be applied.
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
