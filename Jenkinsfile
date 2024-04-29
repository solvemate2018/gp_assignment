pipeline{
    agent any
    triggers{
        pollSCM("* * * * *")
    }
    stages{
        stage("Build"){
            steps{
               sh("docker compose build")
            }
        }
        stage("Deliver"){
            steps{
               withCredentials([usernamePassword(credentialsId: 'DockerHub', usernameVariable: 'USERNAME', passwordVariable: 'PASSWORD')])
               sh 'docker login -u $USERNAME -p $PASSWORD'
               sh "docker compose push"
            }
        }
    }
}