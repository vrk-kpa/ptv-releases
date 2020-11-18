FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS runtime
ARG source
WORKDIR /app
EXPOSE 5000
COPY ${source:-publish/console} .
ENTRYPOINT ["dotnet", "PTV.DataImport.Console.dll"]
RUN update-ca-certificates  --fresh
