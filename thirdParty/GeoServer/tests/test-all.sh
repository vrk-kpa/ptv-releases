#!/bin/bash
set -e

if [ -z "$1" ]; then
    echo "test-all.sh prints checksums of the most important GeoServer WFS requests to be compared before and after changes."
    echo "Usage: test-all.sh [geoserver url]"
    echo "geoserver url example: https://ws.palvelutietovaranto.qa.suomi.fi/geoserver"
    exit
fi

GEOSERVER_URL=$1

TOTAL=""

echo_checksum () {
    local response=`curl -sf "$2"`
    # Ignore timestamp attributes when computing the checksum
    local normalized=`echo "$response" | sed -e 's/timeStamp=\"[^\"]*\"/timestamp=\"\"/g' | sed -e 's/fid=\"[^\"]*\"/fid=\"\"/g' | sed -e 's!'"$GEOSERVER_URL"'!!g'`
    local checksum=`echo "$normalized" | md5sum`

    if [ -z "$response" ]; then
        echo "Failed to fetch $1 from $2"
        curl -f "$2"
        exit 1
    else
        echo "$1 $checksum"
        TOTAL="$TOTAL $checksum"
    fi
}

echo_checksum "ptv:channelIds" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.1.0&request=GetFeature&typeName=ptv%3AchannelIds&outputFormat=gml3&maxFeatures=50"
echo_checksum "ptv:locationChannelAddress" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.0.0&request=GetFeature&typeName=ptv%3AlocationChannelAddress&maxFeatures=50"
echo_checksum "ptv:locationChannelName" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.0.0&request=GetFeature&typeName=ptv%3AlocationChannelName&maxFeatures=50"
echo_checksum "ptv:locationCoordinate" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.0.0&request=GetFeature&typeName=ptv%3AlocationCoordinate&maxFeatures=50"
echo_checksum "ptv:locationOrganizationName" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.0.0&request=GetFeature&typeName=ptv%3AlocationOrganizationName&maxFeatures=50"
echo_checksum "ptv:mv_layer_serviceChannel_grouped" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.0.0&request=GetFeature&typeName=ptv%3Amv_layer_serviceChannel_grouped&maxFeatures=50"
echo_checksum "ptv:serviceChannel" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.0.0&request=GetFeature&typeName=ptv%3AserviceChannel&maxFeatures=50"
echo_checksum "ptv:serviceChannelBase" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.0.0&request=GetFeature&typeName=ptv%3AserviceChannelBase&maxFeatures=50"
echo_checksum "ptv:serviceChannelLanguage" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.0.0&request=GetFeature&typeName=ptv%3AserviceChannelLanguage&maxFeatures=50"
echo_checksum "ptv:serviceChannelServiceJson" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.0.0&request=GetFeature&typeName=ptv%3AserviceChannelServiceJson&maxFeatures=50"
echo_checksum "ptv:serviceClass" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.0.0&request=GetFeature&typeName=ptv%3AserviceClass&maxFeatures=50"
echo_checksum "ptv:serviceLocation" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.1.0&request=GetFeature&typeName=ptv%3AserviceLocation&outputFormat=gml3&maxFeatures=50"
echo_checksum "ptv:serviceLocationLan" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.1.0&request=GetFeature&typeName=ptv%3AsfChannelIds&maxFeatures=50"
echo_checksum "ptv:sfChannelIds" "$GEOSERVER_URL/ptv/ows?service=WFS&version=1.0.0&request=GetFeature&typeName=ptv%3AsfChannelIds&maxFeatures=50"

total_sum=`echo "$TOTAL" | md5sum`
echo "All: $total_sum"
