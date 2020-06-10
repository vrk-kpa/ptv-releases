#!/bin/bash
set -e

GS_INSTALL_PATH=/tmp/gs-install
SCHEMA_PATH=geoserver/www/schemas/ptv

host=$(echo ${GEOSERVER_BASE_SCHEMA_URI} | sed -e "s#[^/]*\/\/\([^@]*@\)\?\([^:/]*\).*#\2#")
echo 'host: '$host >> /tmp/install_info.txt
geoserverCachePath=$(echo $host | awk -F. '{for(i=NF;i>0;--i) printf "%s%s", (i<NF ? "/" : "") , $i, i}')
echo 'cachePath: '$geoserverCachePath >> /tmp/install_info.txt

# configure app-schema.properties
sed -i \
    -e "s#%DB_HOST%#${DB_HOST}#" \
    -e "s#%DB_PORT%#${DB_PORT}#" \
    -e "s#%DB_NAME%#${DB_NAME}#" \
    -e "s#%DB_USERNAME%#${DB_USERNAME}#" \
    -e "s#%DB_PASSWORD%#${DB_PASSWORD}#" \
    -e "s#%GEOSERVER_BASE_SCHEMA_URI%#${GEOSERVER_BASE_SCHEMA_URI}#" \
    ${GEOSERVER_DATA_DIR}/app-schema.properties

# configure environment.properties not needed, variables are taken from environment schema properties
# sed -i \
#     -e "s/%DB_HOST%/$DB_HOST/" \
#     -e "s/%DB_PORT%/$DB_PORT/" \
#     -e "s/%DB_NAME%/$DB_NAME/" \
#     -e "s/%DB_USERNAME%/$DB_USERNAME/" \
#     -e "s/%DB_PASSWORD%/$DB_PASSWORD/" \
#     -e "s/%GEOSERVER_BASE_SCHEMA_URI%/$GEOSERVER_BASE_SCHEMA_URI/" /
#     ${GEOSERVER_DATA_DIR}/environment.properties

# configure logging 
mkdir -p ${GEOSERVER_LOG_PATH}
sed -i \
    -e "s#%GEOSERVER_LOG_PATH%#${GEOSERVER_LOG_PATH}#" \
    ${GEOSERVER_DATA_DIR}/logging.xml

# configure schema files
schemaLocationUrl=${GEOSERVER_BASE_SCHEMA_URI}/$SCHEMA_PATH
echo 'schemaLocationUrl: '$schemaLocationUrl >> /tmp/install_info.txt

find $GS_INSTALL_PATH/schemas/* -type f -name "*.xsd" | xargs sed -i "s#%SCHEMA_LOCATION_URL%#$schemaLocationUrl#g"

# copy schemas to geoserver/www folder and to data_dir/app-schema-cache folder 
mkdir -p ${GEOSERVER_DATA_DIR}/app-schema-cache/$geoserverCachePath/$SCHEMA_PATH
mkdir -p ${CATALINA_HOME}/webapps/$SCHEMA_PATH/
cp $GS_INSTALL_PATH/schemas/* ${CATALINA_HOME}/webapps/$SCHEMA_PATH/
cp $GS_INSTALL_PATH/schemas/* ${GEOSERVER_DATA_DIR}/app-schema-cache/${geoserverCachePath}/$SCHEMA_PATH/

#set controlflow.properties
sed -i \
    -e "s#\${REQUEST_TIMEOUT}#${REQUEST_TIMEOUT}#" \
    -e "s#\${PARALELL_REQUEST}#${PARALELL_REQUEST}#" \
    -e "s#\${GETMAP}#${GETMAP}#" \
    -e "s#\${GETFEATURE}#${GETFEATURE}#" \
    -e "s#\${SINGLE_USER}#${SINGLE_USER}#" \
    ${GEOSERVER_DATA_DIR}/controlflow.properties

# set alias
echo "alias ls='ls -l --color'" >> ~/.bashrc

#set log path for auditing
mkdir -p ${GEOSERVER_LOG_PATH}
sed -i \
    -e "s#%GEOSERVER_LOG_PATH%#${GEOSERVER_LOG_PATH}#" \
    ${GEOSERVER_DATA_DIR}/monitoring/monitor.properties

export GEOSERVER_OPTS="\
    -DALLOW_ENV_PARAMETRIZATION=true \
    -Dapp-schema.properties=${GEOSERVER_DATA_DIR}/app-schema.properties \
    -DGEOSERVER_DATA_DIR=${GEOSERVER_DATA_DIR} \
    -Djava.awt.headless=true \
    -server \
    -Xms${INITIAL_MEMORY} \
    -Xmx${MAXIMUM_MEMORY} \
    -Xrs \
    -Dorg.geotools.referencing.forceXY=true \
    -D-XX:SoftRefLRUPolicyMSPerMB=36000 \
    -XX:SoftRefLRUPolicyMSPerMB=36000 \
    -XX:+UseParallelGC \
    -XX:NewRatio=2 \
    -XX:+CMSClassUnloadingEnabled \
    -XX:PerfDataSamplingInterval=500 \
    -XX:NewSize=300m \
    -Dfile.encoding=UTF8 \
    -Duser.timezone=GMT \
    -Djavax.servlet.request.encoding=UTF-8 \
    -Djavax.servlet.response.encoding=UTF-8 \
    -Dorg.geotools.shapefile.datetime=true \
    -Dorg.geotools.shapefile.datetime=true \
    -Dgeoserver.xframe.shouldSetPolicy=${XFRAME_OPTIONS} \
    -Ds3.properties.location=${GEOSERVER_DATA_DIR}/s3.properties \
    -Dsun.java2d.renderer.useThreadLocal=false \
    -Dsun.java2d.renderer.pixelsize=8192 \
    -Dsun.java2d.renderer=org.marlin.pisces.PiscesRenderingEngine 
    -Dlog4j.configuration=${CATALINA_HOME}/log4j.properties \
    --add-modules java.se \
    --add-exports java.base/jdk.internal.ref=ALL-UNNAMED \
    --add-opens java.base/java.lang=ALL-UNNAMED \
    --add-opens java.base/java.nio=ALL-UNNAMED \
    --add-opens java.base/sun.nio.ch=ALL-UNNAMED \
    --add-opens java.management/sun.management=ALL-UNNAMED \
    --add-opens jdk.management/com.sun.management.internal=ALL-UNNAMED \
    --patch-module java.desktop=${CATALINA_HOME}/marlin-0.9.4.2-Unsafe-OpenJDK9.jar"

## Preparare the JVM command line arguments
export JAVA_OPTS="${JAVA_OPTS} ${GEOSERVER_OPTS}"

exec /usr/local/tomcat/bin/catalina.sh run