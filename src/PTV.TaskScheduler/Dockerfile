FROM mcr.microsoft.com/dotnet/aspnet:3.1@sha256:fdfe7ac9875f35ebba493862fb2a3fcf14b8174987d8959a4cb6da0302197014 AS runtime
ARG source
RUN apt-get update && \
    apt-get --yes --force-yes install gnupg
RUN echo deb http://apt.postgresql.org/pub/repos/apt/ buster-pgdg main > /etc/apt/sources.list.d/pgdg.list && \
    curl https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add -
RUN apt-get update && \
    apt-get --yes install postgresql-client-12 && \
    apt-get --yes upgrade postgresql-client-12
RUN update-ca-certificates  --fresh
WORKDIR /app
EXPOSE 5000
COPY ${source:-publish/scheduler} .
ENTRYPOINT ["dotnet", "PTV.TaskScheduler.dll"]
