FROM mcr.microsoft.com/dotnet/sdk:3.1@sha256:8e4a55f6349d9f04f1a02f9e1187ea350a3c77d0300624c5f6f70d5152b2abc1 AS runtime
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
