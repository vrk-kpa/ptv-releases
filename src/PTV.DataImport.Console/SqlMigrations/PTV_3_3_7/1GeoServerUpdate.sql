------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- ADD NEW MATERIALIZED VIEW  ---------------------------------------
------------------------------------------------------------------------------------------------------------------------

CREATE MATERIALIZED VIEW IF NOT EXISTS geo."mv_layer_serviceChannel_grouped"
    TABLESPACE pg_default
AS
SELECT
            row_number() OVER () AS id,
            "rootId",
            location,
            string_agg(CAST("versionedId" as varchar), ';') as "versionedId",
            string_agg(CAST("organizationId" as varchar), ';') as "organizationId",
            string_agg(CAST("addressId" as varchar), ';') as "addressId",
            string_agg(CAST("addressType" as varchar), ';') as "addressType",
            string_agg(CAST("currentlyOpen" as varchar), ';') as "currentlyOpen",
            string_agg(CAST("channelNameFi" as varchar), ';') as "channelNameFi",
            string_agg(CAST("channelNameSv" as varchar), ';') as "channelNameSv",
            string_agg(CAST("channelNameEn" as varchar), ';') as "channelNameEn",
            string_agg(CAST("organizationNameFi" as varchar), ';') as "organizationNameFi",
            string_agg(CAST("organizationNameSv" as varchar), ';') as "organizationNameSv",
            string_agg(CAST("organizationNameEn" as varchar), ';') as "organizationNameEn",
            string_agg(CAST("addressFi" as varchar), ';') as "addressFi",
            string_agg(CAST("addressSv" as varchar), ';') as "addressSv",
            string_agg(CAST("addressEn" as varchar), ';') as "addressEn",
            string_agg(CAST("addressAdditionalInformationFi" as varchar), ';') as "addressAdditionalInformationFi",
            string_agg(CAST("addressAdditionalInformationSv" as varchar), ';') as "addressAdditionalInformationSv",
            string_agg(CAST("addressAdditionalInformationEn" as varchar), ';') as "addressAdditionalInformationEn",
            string_agg(CAST("municipalityCode" as varchar), ';') as "municipalityCode",
            string_agg(CAST("municipalityNameFi" as varchar), ';') as "municipalityNameFi",
            string_agg(CAST("municipalityNameSv" as varchar), ';') as "municipalityNameSv",
            string_agg(CAST("municipalityNameEn" as varchar), ';') as "municipalityNameEn",
            string_agg(CAST("phoneNumbersFi" as varchar), ';') as "phoneNumbersFi",
            string_agg(CAST("phoneNumbersSv" as varchar), ';') as "phoneNumbersSv",
            string_agg(CAST("phoneNumbersEn" as varchar), ';') as "phoneNumbersEn",
            string_agg(CAST("normalOpeningHoursFi" as varchar), ';') as "normalOpeningHoursFi",
            string_agg(CAST("normalOpeningHoursSv" as varchar), ';') as "normalOpeningHoursSv",
            string_agg(CAST("normalOpeningHoursEn" as varchar), ';') as "normalOpeningHoursEn",
            string_agg(CAST("exceptionalOpeningHoursFi" as varchar), ';') as "exceptionalOpeningHoursFi",
            string_agg(CAST("exceptionalOpeningHoursSv" as varchar), ';') as "exceptionalOpeningHoursSv",
            string_agg(CAST("exceptionalOpeningHoursEn" as varchar), ';') as "exceptionalOpeningHoursEn",
            string_agg(CAST("bankholidayOpeningHoursFi" as varchar), ';') as "bankholidayOpeningHoursFi",
            string_agg(CAST("bankholidayOpeningHoursSv" as varchar), ';') as "bankholidayOpeningHoursSv",
            string_agg(CAST("bankholidayOpeningHoursEn" as varchar), ';') as "bankholidayOpeningHoursEn",
            string_agg(CAST("overmidnightOpeningHoursFi" as varchar), ';') as "overmidnightOpeningHoursFi",
            string_agg(CAST("overmidnightOpeningHoursSv" as varchar), ';') as "overmidnightOpeningHoursSv",
            string_agg(CAST("overmidnightOpeningHoursEn" as varchar), ';') as "overmidnightOpeningHoursEn",
            string_agg(CAST("serviceId" as varchar), ';') as "serviceId",
            string_agg(CAST("serviceNameFi" as varchar), ';') as "serviceNameFi",
            string_agg(CAST("serviceNameSv" as varchar), ';') as "serviceNameSv",
            string_agg(CAST("serviceNameEn" as varchar), ';') as "serviceNameEn",
            string_agg(CAST("coordinateState" as varchar), ';') as "coordinateState",
            string_agg(CAST("coordinateType" as varchar), ';') as "coordinateType"
FROM geo."mv_layer_serviceChannel"
group by "rootId", location
WITH DATA;

CREATE INDEX "mv_layer_serviceChannel_grouped_addressId_idx"
    ON geo."mv_layer_serviceChannel_grouped" USING btree
        ("addressId")
    TABLESPACE pg_default;
CREATE UNIQUE INDEX "mv_layer_serviceChannel_grouped_id_idx"
    ON geo."mv_layer_serviceChannel_grouped" USING btree
        (id)
    TABLESPACE pg_default;
CREATE INDEX "mv_layer_serviceChannel_grouped_location_idx"
    ON geo."mv_layer_serviceChannel_grouped" USING gist
    (location)
    TABLESPACE pg_default;
CREATE INDEX "mv_layer_serviceChannel_grouped_rootId_idx"
    ON geo."mv_layer_serviceChannel_grouped" USING btree
        ("rootId")
    TABLESPACE pg_default;
CREATE INDEX "mv_layer_serviceChannel_grouped_versionedId_idx"
    ON geo."mv_layer_serviceChannel_grouped" USING btree
        ("versionedId")
    TABLESPACE pg_default;

grant select on geo."mv_layer_serviceChannel_grouped" to geoserver_user;

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Refresh views function -------------------------------------------
------------------------------------------------------------------------------------------------------------------------
create or replace function geo."RefreshMaterializedViews"()
    returns void
as $$
begin

    refresh materialized view concurrently geo."LocationChannel";

    refresh materialized view concurrently geo."mv_ChannelService";
    refresh materialized view concurrently geo."mv_ChannelName";
    refresh materialized view concurrently geo."mv_ChannelNameLan";
    refresh materialized view concurrently geo."mv_OrganizationName";
    refresh materialized view concurrently geo."mv_OrganizationNameLan";
    refresh materialized view concurrently geo."mv_ServiceName";
    refresh materialized view concurrently geo."mv_ServiceNameLan";
    refresh materialized view geo."mv_ChannelPhone";
    refresh materialized view concurrently geo."mv_ChannelPhoneLan";
    refresh materialized view concurrently geo."mv_ChannelPhoneGroup";
    refresh materialized view concurrently geo."mv_ChannelPhoneGroupLan";

    refresh materialized view geo."mv_StandardOpeningHours";
    refresh materialized view concurrently geo."mv_ChannelCurrentlyOpen";

    refresh materialized view concurrently geo."mv_ChannelAddressBase";
    refresh materialized view concurrently geo."mv_ChannelAddress";
    refresh materialized view concurrently geo."mv_ChannelAddressLan";

    refresh materialized view geo."mv_AddressCoordinate";
    refresh materialized view concurrently geo."mv_AddressCoordinateDistinct";
    refresh materialized view concurrently geo."mv_AddressCoordinateGroup";
    refresh materialized view concurrently geo."mv_ChannelCoordinateGroup";
    refresh materialized view concurrently geo."mv_LocationChannelGroup";

    refresh materialized view concurrently geo."mv_ServiceHoursBase";
    refresh materialized view geo."mv_HolidayName";
    refresh materialized view concurrently geo."mv_HolidayNameLan";
    refresh materialized view geo."mv_ServiceHoursAdditionalInformation";
    refresh materialized view concurrently geo."mv_ServiceHoursAdditionalInformationLan";
    refresh materialized view concurrently geo."mv_OpeningHours";
    refresh materialized view concurrently geo."mv_OpeningHoursSpecial";
    refresh materialized view concurrently geo."mv_ServiceHoursSpecial";
    refresh materialized view concurrently geo."mv_OpeningHoursDay";
    refresh materialized view concurrently geo."mv_OpeningHoursGrouped";
    refresh materialized view concurrently geo."mv_OpeningHoursGroupedLan";
    refresh materialized view concurrently geo."mv_ServiceHoursGrouped";
    refresh materialized view concurrently geo."mv_ServiceHoursGroupedLan";
    refresh materialized view concurrently geo."mv_ServiceHoursGroupedType";
    refresh materialized view concurrently geo."mv_ServiceHoursGroupedTypeLan";
    refresh materialized view concurrently geo."mv_ServiceClass";
    refresh materialized view concurrently geo."mv_ChannelServiceJson";

    refresh materialized view concurrently geo."ChannelIds";
    refresh materialized view geo."mv_layer_serviceChannelBase";
    refresh materialized view geo."mv_layer_serviceChannel";
    refresh materialized view geo."mv_layer_serviceChannelServiceJson";
    refresh materialized view geo."mv_layer_serviceChannelLanguage";
    refresh materialized view geo."mv_layer_locationChannelAddress";
    refresh materialized view geo."mv_layer_serviceClass";
    refresh materialized view geo."mv_dataStore_municipalityName";
    refresh materialized view geo."mv_dataStore_serviceLocationAddress";
    refresh materialized view geo."mv_dataStore_streetAdditionalInformation";
    refresh materialized view geo."mv_dataStore_streetAddress";
    refresh materialized view geo."mv_dataStore_serviceLocationLan";
    refresh materialized view geo."mv_dataStore_serviceLocationLanService";
    refresh materialized view geo."mv_dataStore_serviceLocationLanAddress";
    refresh materialized view concurrently geo."mv_dataStore_standardServiceHours";
    refresh materialized view concurrently geo."mv_dataStore_standardServiceHoursLan";
    refresh materialized view concurrently geo."mv_dataStore_exceptionalServiceHours";
    refresh materialized view concurrently geo."mv_dataStore_exceptionalServiceHoursLan";
    refresh materialized view concurrently geo."mv_dataStore_specialServiceHours";
    refresh materialized view concurrently geo."mv_dataStore_specialServiceHoursLan";
    refresh materialized view concurrently geo."mv_dataStore_holidayServiceHours";
    refresh materialized view concurrently geo."mv_dataStore_holidayServiceHoursLan";
    refresh materialized view geo."mv_layer_serviceChannel_grouped";

end;$$ language plpgsql;