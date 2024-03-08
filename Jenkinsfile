pipeline {
    agent { label 'ezenity-node' }

    tools {
        // Make sure Jenkins has .NET Core 6.0 installed and configured
        dotnet '6.0'
    }

    environment {
        // Define environment variables
        DOTNET_CORE_VERSION = '6.0'
        BUILD_CONFIGURATION = 'Release'
    }

    stages {
        stage('Preparation') {
            steps {
                checkout scm
                // Optionally clean workspace
                sh 'dotnet tool restore'
                sh 'dotnet cake --target=Clean'
            }
        }
        stage('Restore') {
            steps {
                echo 'Restoring project dependencies...'
                bat 'dotnet restore Ezenity.sln'
            }
        }
        stage('Build') {
            steps {
                echo 'Building the solution...'
                bat "dotnet build Ezenity.sln -c ${BUILD_CONFIGURATION} --no-restore"
            }
        }
        stage('Test') {
            steps {
                echo 'Running unit tests...'
                bat "dotnet test Ezenity.Tests/Ezenity.Tests.csproj -c ${BUILD_CONFIGURATION} --no-build --logger \"trx;LogFileName=test_results.trx\""
                // TODO: Adding steps to publish test results for a plugin that supports it
            }
        }
        stage('Deploy') {
            when {
                branch 'main'
            }
            steps {
                echo 'Deploying application...'
                script {
                    bat "dotnet publish Ezenity.API/Ezenity.API.csproj -c ${BUILD_CONFIGURATION} -o /var/www/ezenity_api"
                    sh "sudo systemctl restart ezenity_api.service"
                }
            }
        }
    }

    post {
        always {
            echo 'Cleaning up...'
            // TODO: Add any cleanup steps if necessary
        }
        success {
            echo 'Build and deployment succeeded!'
        }
        failure {
            echo 'Build or deployment failed.'
        }
    }
}
