FROM microsoft/dotnet:2.2-aspnetcore-runtime
ARG source
WORKDIR /app
EXPOSE 5000
COPY ${source:-publish} .
ENTRYPOINT ["dotnet", "PTV.IdentityUserManager.dll"]
