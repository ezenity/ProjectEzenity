pipeline {
    agent { label 'ezenity-node' }

    environment {
        // Define environment variables here
        DOTNET_CORE_VERSION = '6.0'
        BUILD_CONFIGURATION = 'Release'
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
                sh 'dotnet restore ProjectEzenity.sln'
            }
        }
        stage('Build') {
            steps {
                echo 'Building the solution...'
                sh "dotnet build ProjectEzenity.sln -c ${BUILD_CONFIGURATION} --no-restore"
            }
        }
        stage('Test') {
            steps {
                echo 'Running unit tests...'
                sh "dotnet test Ezenity.Tests/Ezenity.Tests.csproj -c ${BUILD_CONFIGURATION} --no-build --logger \"trx;LogFileName=test_results.trx\""
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
                    sh "dotnet publish Ezenity.API/Ezenity.API.csproj -c ${BUILD_CONFIGURATION} -o /var/www/ezenity_api"
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
