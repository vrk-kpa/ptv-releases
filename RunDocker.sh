dotnet publish -c Release src/PTV.Application.Web -o publish
dotnet publish -c Release src/PTV.Application.Api -o publish
dotnet publish -c Release src/PTV.Application.OpenApi -o publish
docker rmi ptv.application.web 2>null
docker rmi ptv.application.api 2>null
docker rmi ptv.application.openapi 2>null
docker-compose -f docker-compose.yml -f docker-compose.ci.build.yml -f docker-compose.override.yml -f docker-compose.vs.release.yml build
mkdir DockerImages -p
DATE=$(date +"%Y%m%d-%H%M")
docker save ptv.application.web -o DockerImages/PtvAppWeb_$DATE.docker
docker save ptv.application.api -o DockerImages/PtvAppApi_$DATE.docker
docker save ptv.application.openapi -o DockerImages/PtvOpenApi_$DATE.docker
docker rmi ptv.application.web
docker rmi ptv.application.api
docker rmi ptv.application.openapi
/ptvimages/copyDockerImages.sh $DATE
