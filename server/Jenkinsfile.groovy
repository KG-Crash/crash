pipeline {
    agent { label 'slave' }
    stages {
        stage('Build') {
            steps{ 
                sh 'fab -f deploy/fabfile.py environment:e=development build'
            }
        }

        stage('Deploy') {
            steps{ 
                sh 'fab -f deploy/fabfile.py environment:e=development deploy'
            }
        }

        stage('Clean up') {
            steps{ 
                sh 'fab -f deploy/fabfile.py environment:e=development cleanup'
            }
        }
    }

    post {
        always {
            discordSend description: "'crash' build completed.\nBuild ${env.BUILD_NUMBER}", 
                        footer: currentBuild.currentResult, 
                        link: env.BUILD_URL, 
                        result: currentBuild.currentResult, 
                        title: "KG CRASH", 
                        webhookURL: "https://discord.com/api/webhooks/855472559918546964/NP4xkqh0s25FO7N3eXXXAyf2jf2q13ou2w17v0e_YNO3NB4G9tZ_QPJxYIzBGxfUgkub"
        }
    }
}
