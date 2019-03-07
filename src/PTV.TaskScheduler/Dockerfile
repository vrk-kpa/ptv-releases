FROM microsoft/dotnet:2.2-aspnetcore-runtime
ARG source
RUN apt-get update && apt-get --yes --force-yes install gnupg
RUN echo deb http://apt.postgresql.org/pub/repos/apt/ stretch-pgdg main > /etc/apt/sources.list.d/pgdg.list && curl https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add -
RUN apt-get update && apt-get --yes --force-yes install postgresql-9.6 && apt-get --yes --force-yes upgrade postgresql-9.6
RUN update-ca-certificates  --fresh
WORKDIR /app
EXPOSE 5000
COPY ${source:-publish} .
ENTRYPOINT ["dotnet", "PTV.TaskScheduler.dll"]
