FROM kartoza/geoserver:2.13.0
COPY /opt /opt
COPY /encrypter/ /app/encrypter
COPY /plugins/ $CATALINA_HOME/webapps/geoserver/WEB-INF/lib
COPY /webconfig/ $CATALINA_HOME/webapps/geoserver/WEB-INF
COPY /EnvironmentsSettings/logging.*.xml /opt/environmentsettings/
ENV JAVA_OPTS "$JAVA_OPTS -DALLOW_ENV_PARAMETRIZATION=true"
RUN apt-get update && apt-get install -y --no-install-recommends apt-utils && apt-get install libunwind8 && \
chmod +x /app/encrypter/u/PTV.AppSettings.Encrypter /app/encrypter/r/PTV.AppSettings.Encrypter && \
chgrp -R 0 /usr/local/tomcat && chmod -R g=u /usr/local/tomcat 