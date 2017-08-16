FROM microsoft/aspnetcore:1.1
ARG source
WORKDIR /app
COPY ${source:-publish} .
ENTRYPOINT ["dotnet", "PTV.DataImport.Console.dll"]
