FROM mcr.microsoft.com/dotnet/aspnet:3.1@sha256:fdfe7ac9875f35ebba493862fb2a3fcf14b8174987d8959a4cb6da0302197014 AS runtime
ARG source
WORKDIR /app
EXPOSE 5000
COPY ${source:-publish/web} .
ENTRYPOINT ["dotnet", "PTV.Application.Web.dll"]
