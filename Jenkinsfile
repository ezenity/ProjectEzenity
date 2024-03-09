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
                echo 'Publish application...'
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
                    // Using the script that handles deployment and service restart
                    sh '/usr/local/bin/restart-ezenity-api.sh'
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
