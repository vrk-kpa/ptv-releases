FROM mcr.microsoft.com/dotnet/sdk:3.1@sha256:8ac91870fd88b539c40068da31d1ccd465062beaedb1e1fb3160e97aa1f6e4ad AS runtime
ARG source
WORKDIR /app
EXPOSE 5000
COPY ${source:-publish/sts} .
ENTRYPOINT ["dotnet", "PTV.IdentityUserManager.dll"]
