FROM ubuntu:20.04@sha256:703218c0465075f4425e58fac086e09e1de5c340b12976ab9eb8ad26615c3715

ARG INSTALL_PATH=/tmp/gs-install
ARG GEOSERVER_DATA_DIR=/opt/geoserver/data_dir
ARG JAVA_HOME=/usr/local/jvm/

ENV CATALINA_HOME=/usr/local/tomcat
ENV DEBIAN_FRONTEND=noninteractive

RUN mkdir -p ${GEOSERVER_DATA_DIR} ${INSTALL_PATH} ${CATALINA_HOME} ${JAVA_HOME}

COPY scripts/ /scripts/
COPY schemas/ ${INSTALL_PATH}/schemas/
COPY install/ ${INSTALL_PATH}/
COPY data_dir/ ${GEOSERVER_DATA_DIR}/

RUN apt-get update -y               && \ 
    apt-get install -y apt-utils    && \
    apt-get install unzip           && \
    apt-get update -y

RUN \
    # install java
    mkdir ${JAVA_HOME}/openjdk-11.0.7+10-jre \
    && unzip ${INSTALL_PATH}/openjdk-11.0.7+10-jre.zip -d ${JAVA_HOME}/openjdk-11.0.7+10-jre/ \
    && ln -s ${JAVA_HOME}/openjdk-11.0.7+10-jre ${JAVA_HOME}/default-java \
    \
    # install tomcat
    && unzip ${INSTALL_PATH}/tomcat9.zip -d ${CATALINA_HOME} \
    # redirect homepage
    && cp -r ${INSTALL_PATH}/index.jsp ${CATALINA_HOME}/webapps/ROOT/index.jsp \
    \
    # install fonts
    && apt-get install -y \
        fonts-cantarell \
        lmodern \
        ttf-aenigma \
        ttf-georgewilliams \
        ttf-bitstream-vera \
        ttf-sjfonts \
        tv-fonts \
        #
        # Informational list of build-essential packages
        # build-essential \ 
        #
        # Apache Portable Runtime Library - Development Headers
        # libapr1-dev \
        #
        # Secure Sockets Layer toolkit - development files
        # libssl-dev \
        #
        # install Geospatial Data Abstraction Library - Utility programs
        gdal-bin \
        #
        # install Java bindings to the Geospatial Data Abstraction Library
        libgdal-java \
    #
    # install libjpeg-turbo
    && dpkg -i ${INSTALL_PATH}/libjpeg-turbo-official_2.0.4_amd64.deb


# unzip geoserver and pluggins
RUN unzip ${INSTALL_PATH}/geoserver.war -d ${CATALINA_HOME}/webapps/geoserver \
    && cp -r ${CATALINA_HOME}/webapps/geoserver/data/user_projections ${GEOSERVER_DATA_DIR} \
    && rm -rf ${CATALINA_HOME}/webapps/geoserver/data \
    #
    && unzip -o ${INSTALL_PATH}/geoserver-2.17.0-plugins.zip -d ${CATALINA_HOME}/webapps/geoserver/WEB-INF/lib \
    #
    && mkdir /usr/local/gdal_data \
    && unzip -o ${INSTALL_PATH}/gdal-data.zip -d /usr/local/gdal_data \
    && mkdir /usr/local/gdal_native_libs \
    && unzip -o ${INSTALL_PATH}/gdal192-Ubuntu12-gcc4.6.3-x86_64.zip -d /usr/local/gdal_native_libs \
    #
    && cp ${INSTALL_PATH}/marlin-0.9.4.2-Unsafe-OpenJDK9.jar ${CATALINA_HOME}/webapps/geoserver/WEB-INF/lib \
    #
    && cp ${INSTALL_PATH}/jetty-servlets-9.4.21.v20190926.jar ${CATALINA_HOME}/webapps/geoserver/WEB-INF/lib/jetty-servlets.jar \
    && cp ${INSTALL_PATH}/jetty-util-9.4.21.v20190926.jar ${CATALINA_HOME}/webapps/geoserver/WEB-INF/lib/jetty-util.jar \
    #
    #&& rm ${CATALINA_HOME}/webapps/geoserver/WEB-INF/lib/jai* 
    && cp -r ${INSTALL_PATH}/web.xml ${CATALINA_HOME}/webapps/geoserver/WEB-INF/web.xml \
    && mkdir -p ${CATALINA_HOME}/webapps/geoserver/www \
    && cp -r ${INSTALL_PATH}/permissionDenied.jsp ${CATALINA_HOME}/webapps/geoserver/www/permissionDenied.jsp
    
# clean
RUN dpkg-divert --local --rename --add /sbin/initctl                    && \
    (echo "Yes, do as I say!" | apt-get autoremove --force-yes login)   && \
    apt-get clean                                                       && \
    rm -rf /var/lib/apt/lists/*                                         

ENV \
    JAVA_HOME=${JAVA_HOME}/default-java \
    GEOSERVER_DATA_DIR=$GEOSERVER_DATA_DIR \
    GDAL_DATA=/usr/local/gdal_data \
    #LGE LD_LIBRARY_PATH="$LD_LIBRARY_PATH:/usr/local/apr/lib:/opt/libjpeg-turbo/lib64:/usr/lib:/usr/lib/x86_64-linux-gnu" \
    #LGE FOOTPRINTS_DATA_DIR=/opt/footprints_dir \
    GEOWEBCACHE_CACHE_DIR=$GEOSERVER_DATA_DIR/gwc \
    ENABLE_JSONP=true \
    MAX_FILTER_RULES=20 \
    OPTIMIZE_LINE_WIDTH=false \
    SSL=false \
    HTTP_PORT=8080 \
    HTTP_PROXY_NAME= \
    HTTP_PROXY_PORT= \
    HTTP_REDIRECT_PORT= \
    HTTP_CONNECTION_TIMEOUT=20000 \
    # HTTPS_PORT=8443 \
    # HTTPS_MAX_THREADS=150 \
    # HTTPS_CLIENT_AUTH= \
    # HTTPS_PROXY_NAME= \
    # HTTPS_PROXY_PORT= \
    #LGE JKS_FILE=letsencrypt.jks \
    #LGE JKS_KEY_PASSWORD='geoserver' \
    #LGE KEY_ALIAS=letsencrypt \
    #LGE JKS_STORE_PASSWORD='geoserver' \
    #LGE P12_FILE=letsencrypt.p12 \
    #LGE PKCS12_PASSWORD='geoserver' \
    #LGE LETSENCRYPT_CERT_DIR=/etc/letsencrypt \
    #LGE RANDFILE=${LETSENCRYPT_CERT_DIR}/.rnd \
    GEOSERVER_CSRF_DISABLED=true

ENV \
    INITIAL_MEMORY="1G" \
    MAXIMUM_MEMORY="2G" \
    XFRAME_OPTIONS="true" \
    REQUEST_TIMEOUT=60 \
    PARALELL_REQUEST=20 \
    GETMAP=10 \
    GETFEATURE=10 \
    #REQUEST_EXCEL=4 \
    SINGLE_USER=6 \
    GWC_REQUEST=16 \
    #WPS_REQUEST=1000/d;30s \
    #S3_SERVER_URL='' \
    #S3_USERNAME='' \
    #S3_PASSWORD='' \
    SAMPLE_DATA='FALSE'


RUN groupadd -r geoserverusers -g 10001 \
    && useradd -M -u 10000 -g geoserverusers geoserveruser

WORKDIR ${GEOSERVER_DATA_DIR}
EXPOSE ${HTTP_PORT}

CMD ["/bin/sh", "/scripts/start.sh"]