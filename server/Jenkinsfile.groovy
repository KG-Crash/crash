pipeline {
    agent { label 'slave' }
    stages {
        stage('Preperence') {
            steps{ 
                    sh 'fab -f deploy/fabfile.py environment:e=development'
                }
        }
    }
}
