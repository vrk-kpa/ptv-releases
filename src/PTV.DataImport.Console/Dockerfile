FROM mcr.microsoft.com/dotnet/sdk:3.1@sha256:8e4a55f6349d9f04f1a02f9e1187ea350a3c77d0300624c5f6f70d5152b2abc1 AS runtime
ARG source
WORKDIR /app
EXPOSE 5000
COPY ${source:-publish/console} .
ENTRYPOINT ["dotnet", "PTV.DataImport.Console.dll"]
RUN update-ca-certificates  --fresh
