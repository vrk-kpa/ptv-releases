-- Step 1.: Drop views which depend on the column
drop materialized view if exists geo."mv_dataStore_exceptionalServiceHoursLan";
drop materialized view if exists geo."mv_dataStore_specialServiceHoursLan";
drop materialized view if exists geo."mv_dataStore_standardServiceHoursLan";
drop materialized view if exists geo."mv_layer_serviceChannelServiceJson";
drop materialized view if exists geo."mv_layer_serviceChannel";
drop materialized view if exists geo."mv_ServiceHoursGroupedTypeLan";
drop materialized view if exists geo."mv_layer_serviceChannelLanguage";
drop materialized view if exists geo."mv_ServiceHoursGroupedType";
drop materialized view if exists geo."mv_ServiceHoursGroupedLan";
drop materialized view if exists geo."mv_ServiceHoursGrouped";
drop materialized view if exists geo."mv_ServiceHoursAdditionalInformationLan";
drop materialized view if exists geo."mv_ServiceHoursAdditionalInformation";

-- Step 2.: Change the column
alter table public."ServiceHoursAdditionalInformation" alter column "Text" type character varying(150);

-- Step 3.: Recreate the views
create materialized view if not exists geo."mv_ServiceHoursAdditionalInformation" as
SELECT shai."ServiceHoursId",
       shai."Text" AS "AdditionalInformation",
       sl."Code"   AS "LanguageCode"
FROM "ServiceHoursAdditionalInformation" shai
         JOIN geo."SupportedLanguages" sl ON shai."LocalizationId" = sl."Id"
WHERE (EXISTS(SELECT 1
              FROM geo."mv_ServiceHoursBase" shb
              WHERE shai."ServiceHoursId" = shb."ServiceHoursId"));

alter materialized view geo."mv_ServiceHoursAdditionalInformation" owner to postgres;

create index if not exists "mv_ServiceHoursAdditionalInformation_ServiceHoursId_idx"
    on geo."mv_ServiceHoursAdditionalInformation" ("ServiceHoursId");

create materialized view if not exists geo."mv_ServiceHoursAdditionalInformationLan" as
SELECT "mv_ServiceHoursAdditionalInformation"."ServiceHoursId",
       min("mv_ServiceHoursAdditionalInformation"."AdditionalInformation"::text)
           FILTER (WHERE "mv_ServiceHoursAdditionalInformation"."LanguageCode" = 'fi'::text) AS "FI",
       min("mv_ServiceHoursAdditionalInformation"."AdditionalInformation"::text)
           FILTER (WHERE "mv_ServiceHoursAdditionalInformation"."LanguageCode" = 'sv'::text) AS "SV",
       min("mv_ServiceHoursAdditionalInformation"."AdditionalInformation"::text)
           FILTER (WHERE "mv_ServiceHoursAdditionalInformation"."LanguageCode" = 'en'::text) AS "EN"
FROM geo."mv_ServiceHoursAdditionalInformation"
GROUP BY "mv_ServiceHoursAdditionalInformation"."ServiceHoursId";

alter materialized view geo."mv_ServiceHoursAdditionalInformationLan" owner to postgres;

create unique index if not exists "mv_ServiceHoursAdditionalInformationLan_ServiceHoursId_idx"
    on geo."mv_ServiceHoursAdditionalInformationLan" ("ServiceHoursId");

create materialized view if not exists geo."mv_ServiceHoursGrouped" as
SELECT t."RootId",
       t."Type",
       t."LanguageCode",
       string_agg((
                          CASE
                              WHEN t."Type" = 'Holiday'::text THEN t."HolidayName"
                              ELSE t."AdditionalInformation"
                              END || t."Validity") || COALESCE(t."OpeningType", t."OpeningHours"), '
'::text ORDER BY t."OrderNumber") AS "ServiceHours"
FROM (SELECT shbl."RootId",
             shbl."Type",
             shbl."OrderNumber",
             shbl."LanguageCode",
             CASE
                 WHEN shbl."Type" = 'Special'::text THEN ''::text
                 WHEN shbl."From" IS NOT NULL AND shbl."To" IS NULL THEN ('['::text || shbl."From") || ']: '::text
                 WHEN shbl."From" IS NOT NULL AND shbl."To" IS NOT NULL THEN
                     ((('['::text || shbl."From") || ' - '::text) || shbl."To") || ']: '::text
                 ELSE ''::text
END                                                                                AS "Validity",
             COALESCE(ai."AdditionalInformation"::text || ': '::text, ''::text)                     AS "AdditionalInformation",
             COALESCE(COALESCE(hn."Name", hn_default."Name") || ': '::text, ''::text)               AS "HolidayName",
             geo.opening_hour_type(shbl."OpeningType", shbl."LanguageCode")                         AS "OpeningType",
             COALESCE(COALESCE(ohg."DailyOpeningHours", ohg_default."DailyOpeningHours"), ''::text) AS "OpeningHours"
      FROM (SELECT shb."VersionedId",
                   shb."RootId",
                   shb."ServiceHoursId",
                   shb."HolidayId",
                   shb."OrderNumber",
                   shb."IsHoliday",
                   shb."HolidayDate",
                   shb."Type",
                   shb."From",
                   shb."To",
                   shb."IsClosed",
                   shb."IsReservation",
                   shb."IsNonStop",
                   shb."OpeningType",
                   sl."Code" AS "LanguageCode"
            FROM geo."mv_ServiceHoursBase" shb,
                 geo."SupportedLanguages" sl) shbl
               LEFT JOIN geo."mv_ServiceHoursAdditionalInformation" ai
                         ON shbl."ServiceHoursId" = ai."ServiceHoursId" AND shbl."LanguageCode" = ai."LanguageCode"
               LEFT JOIN geo."mv_HolidayName" hn
                         ON shbl."HolidayId" = hn."HolidayId" AND shbl."LanguageCode" = hn."LanguageCode"
               LEFT JOIN geo."mv_HolidayName" hn_default
                         ON shbl."HolidayId" = hn_default."HolidayId" AND hn_default."LanguageCode" = 'fi'::text
               LEFT JOIN geo."mv_OpeningHoursGrouped" ohg
                         ON shbl."ServiceHoursId" = ohg."OpeningHourId" AND shbl."LanguageCode" = ohg."LanguageCode"
               LEFT JOIN geo."mv_OpeningHoursGrouped" ohg_default
                         ON shbl."ServiceHoursId" = ohg_default."OpeningHourId" AND
                            ohg_default."LanguageCode" = 'fi'::text) t
GROUP BY t."RootId", t."Type", t."LanguageCode";

alter materialized view geo."mv_ServiceHoursGrouped" owner to postgres;

create unique index if not exists "mv_ServiceHoursGrouped_RootId_Type_LanguageCode_idx"
    on geo."mv_ServiceHoursGrouped" ("RootId", "Type", "LanguageCode");

create index if not exists "mv_ServiceHoursGrouped_RootId_idx"
    on geo."mv_ServiceHoursGrouped" ("RootId");

create materialized view if not exists geo."mv_ServiceHoursGroupedLan" as
SELECT "mv_ServiceHoursGrouped"."RootId",
       "mv_ServiceHoursGrouped"."Type",
       min("mv_ServiceHoursGrouped"."ServiceHours")
           FILTER (WHERE "mv_ServiceHoursGrouped"."LanguageCode" = 'en'::text) AS "EN",
       min("mv_ServiceHoursGrouped"."ServiceHours")
           FILTER (WHERE "mv_ServiceHoursGrouped"."LanguageCode" = 'fi'::text) AS "FI",
       min("mv_ServiceHoursGrouped"."ServiceHours")
           FILTER (WHERE "mv_ServiceHoursGrouped"."LanguageCode" = 'sv'::text) AS "SV"
FROM geo."mv_ServiceHoursGrouped"
GROUP BY "mv_ServiceHoursGrouped"."RootId", "mv_ServiceHoursGrouped"."Type";

alter materialized view geo."mv_ServiceHoursGroupedLan" owner to postgres;

create unique index if not exists "mv_ServiceHoursGroupedLan_RootId_Type_idx"
    on geo."mv_ServiceHoursGroupedLan" ("RootId", "Type");

create index if not exists "mv_ServiceHoursGroupedLan_RootId_idx"
    on geo."mv_ServiceHoursGroupedLan" ("RootId");

create materialized view if not exists geo."mv_ServiceHoursGroupedType" as
SELECT "mv_ServiceHoursGrouped"."RootId",
       "mv_ServiceHoursGrouped"."LanguageCode",
       min("mv_ServiceHoursGrouped"."ServiceHours")
           FILTER (WHERE "mv_ServiceHoursGrouped"."Type" = 'Standard'::text)  AS "normalOpeningHours",
       min("mv_ServiceHoursGrouped"."ServiceHours")
           FILTER (WHERE "mv_ServiceHoursGrouped"."Type" = 'Exception'::text) AS "exceptionalOpeningHours",
       min("mv_ServiceHoursGrouped"."ServiceHours")
           FILTER (WHERE "mv_ServiceHoursGrouped"."Type" = 'Holiday'::text)   AS "bankholidayOpeningHours",
       min("mv_ServiceHoursGrouped"."ServiceHours")
           FILTER (WHERE "mv_ServiceHoursGrouped"."Type" = 'Special'::text)   AS "overmidnightOpeningHours"
FROM geo."mv_ServiceHoursGrouped"
GROUP BY "mv_ServiceHoursGrouped"."RootId", "mv_ServiceHoursGrouped"."LanguageCode";

alter materialized view geo."mv_ServiceHoursGroupedType" owner to postgres;

create unique index if not exists "mv_ServiceHoursGroupedType_RootId_LanguageCode_idx"
    on geo."mv_ServiceHoursGroupedType" ("RootId", "LanguageCode");

create materialized view if not exists geo."mv_layer_serviceChannelLanguage" as
SELECT cha."VersionedId"                                                          AS "versionedId",
       cha."RootId"                                                               AS "rootId",
       cha."OrganizationId"                                                       AS "organizationId",
       cha."AddressId"                                                            AS "addressId",
       cha."AddressType"                                                          AS "addressType",
       COALESCE(cco."CurrentlyOpen", '-'::text)                                   AS "currentlyOpen",
       cha."LanguageCode"                                                         AS "languageCode",
       COALESCE(chn."Name", chn_default."Name")                                   AS "channelName",
       COALESCE(ona."Name", ona_default."Name")                                   AS "organizationName",
       COALESCE(cha."StreetAddressComplete", cha_default."StreetAddressComplete") AS "streetAddress",
       COALESCE(cha."AdditionalInformation", cha_default."AdditionalInformation") AS "addressAdditionalInformation",
       cha."MunicipalityCode"                                                     AS "municipalityCode",
       COALESCE(cha."MunicipalityName", cha_default."MunicipalityName")           AS "municipalityName",
       COALESCE(chp."NumberComplete", chp_default."NumberComplete")               AS "phoneNumbers",
       chs."ServiceId"                                                            AS "serviceId",
       COALESCE(sen."Name", sen_default."Name")                                   AS "serviceName",
       sh."normalOpeningHours",
       sh."overmidnightOpeningHours",
       sh."bankholidayOpeningHours",
       sh."exceptionalOpeningHours",
       aco."State"                                                                AS "coordinateState",
       aco."Type"                                                                 AS "coordinateType",
       aco."Location3067"                                                         AS location
FROM geo."mv_ChannelAddress" cha
         JOIN geo."mv_ChannelAddress" cha_default
              ON cha."AddressId" = cha_default."AddressId" AND cha_default."LanguageCode" = 'fi'::text
         JOIN geo."mv_AddressCoordinate" aco ON cha."AddressId" = aco."AddressId"
    LEFT JOIN geo."mv_ChannelCurrentlyOpen" cco ON cha."VersionedId" = cco."VersionedId"
    LEFT JOIN geo."mv_ChannelName" chn
    ON cha."VersionedId" = chn."VersionedId" AND cha."LanguageCode" = chn."LanguageCode"
    LEFT JOIN geo."mv_ChannelName" chn_default
    ON cha."VersionedId" = chn_default."VersionedId" AND chn_default."LanguageCode" = 'fi'::text
    LEFT JOIN geo."mv_OrganizationName" ona
    ON cha."OrganizationId" = ona."OrganizationId" AND cha."LanguageCode" = ona."LanguageCode"
    LEFT JOIN geo."mv_OrganizationName" ona_default
    ON cha."OrganizationId" = ona_default."OrganizationId" AND ona_default."LanguageCode" = 'fi'::text
    LEFT JOIN geo."mv_ChannelPhoneGroup" chp
    ON cha."RootId" = chp."RootId" AND cha."LanguageCode" = chp."LanguageCode"
    LEFT JOIN geo."mv_ChannelPhoneGroup" chp_default
    ON cha."RootId" = chp_default."RootId" AND chp_default."LanguageCode" = 'fi'::text
    LEFT JOIN geo."mv_ChannelService" chs ON cha."RootId" = chs."ChannelId"
    LEFT JOIN geo."mv_ServiceName" sen
    ON chs."ServiceId" = sen."ServiceId" AND cha."LanguageCode" = sen."LanguageCode"
    LEFT JOIN geo."mv_ServiceName" sen_default
    ON chs."ServiceId" = sen_default."ServiceId" AND sen_default."LanguageCode" = 'fi'::text
    LEFT JOIN geo."mv_ServiceHoursGroupedType" sh
    ON cha."RootId" = sh."RootId" AND cha."LanguageCode" = sh."LanguageCode";

alter materialized view geo."mv_layer_serviceChannelLanguage" owner to postgres;

create index if not exists "mv_layer_serviceChannelLanguage_addressId_idx"
    on geo."mv_layer_serviceChannelLanguage" ("addressId");

create index if not exists "mv_layer_serviceChannelLanguage_location_idx"
    on geo."mv_layer_serviceChannelLanguage" using gist (location);

create index if not exists "mv_layer_serviceChannelLanguage_rootId_idx"
    on geo."mv_layer_serviceChannelLanguage" ("rootId");

create index if not exists "mv_layer_serviceChannelLanguage_versionedId_idx"
    on geo."mv_layer_serviceChannelLanguage" ("versionedId");

create materialized view if not exists geo."mv_ServiceHoursGroupedTypeLan" as
SELECT "mv_ServiceHoursGroupedLan"."RootId",
       min("mv_ServiceHoursGroupedLan"."FI")
           FILTER (WHERE "mv_ServiceHoursGroupedLan"."Type" = 'Standard'::text)  AS "normalOpeningHoursFi",
       min("mv_ServiceHoursGroupedLan"."SV")
           FILTER (WHERE "mv_ServiceHoursGroupedLan"."Type" = 'Standard'::text)  AS "normalOpeningHoursSv",
       min("mv_ServiceHoursGroupedLan"."EN")
           FILTER (WHERE "mv_ServiceHoursGroupedLan"."Type" = 'Standard'::text)  AS "normalOpeningHoursEn",
       min("mv_ServiceHoursGroupedLan"."FI")
           FILTER (WHERE "mv_ServiceHoursGroupedLan"."Type" = 'Exception'::text) AS "exceptionalOpeningHoursFi",
       min("mv_ServiceHoursGroupedLan"."SV")
           FILTER (WHERE "mv_ServiceHoursGroupedLan"."Type" = 'Exception'::text) AS "exceptionalOpeningHoursSv",
       min("mv_ServiceHoursGroupedLan"."EN")
           FILTER (WHERE "mv_ServiceHoursGroupedLan"."Type" = 'Exception'::text) AS "exceptionalOpeningHoursEn",
       min("mv_ServiceHoursGroupedLan"."FI")
           FILTER (WHERE "mv_ServiceHoursGroupedLan"."Type" = 'Holiday'::text)   AS "bankholidayOpeningHoursFi",
       min("mv_ServiceHoursGroupedLan"."SV")
           FILTER (WHERE "mv_ServiceHoursGroupedLan"."Type" = 'Holiday'::text)   AS "bankholidayOpeningHoursSv",
       min("mv_ServiceHoursGroupedLan"."EN")
           FILTER (WHERE "mv_ServiceHoursGroupedLan"."Type" = 'Holiday'::text)   AS "bankholidayOpeningHoursEn",
       min("mv_ServiceHoursGroupedLan"."FI")
           FILTER (WHERE "mv_ServiceHoursGroupedLan"."Type" = 'Special'::text)   AS "overmidnightOpeningHoursFi",
       min("mv_ServiceHoursGroupedLan"."SV")
           FILTER (WHERE "mv_ServiceHoursGroupedLan"."Type" = 'Special'::text)   AS "overmidnightOpeningHoursSv",
       min("mv_ServiceHoursGroupedLan"."EN")
           FILTER (WHERE "mv_ServiceHoursGroupedLan"."Type" = 'Special'::text)   AS "overmidnightOpeningHoursEn"
FROM geo."mv_ServiceHoursGroupedLan"
GROUP BY "mv_ServiceHoursGroupedLan"."RootId";

alter materialized view geo."mv_ServiceHoursGroupedTypeLan" owner to postgres;

create unique index if not exists "mv_ServiceHoursGroupedTypeLan_RootId_idx"
    on geo."mv_ServiceHoursGroupedTypeLan" ("RootId");

create materialized view if not exists geo."mv_layer_serviceChannel" as
SELECT chal."VersionedId"                       AS "versionedId",
       chal."RootId"                            AS "rootId",
       chal."OrganizationId"                    AS "organizationId",
       chal."AddressId"                         AS "addressId",
       chal."AddressType"                       AS "addressType",
       COALESCE(cco."CurrentlyOpen", '-'::text) AS "currentlyOpen",
       chnl."FI"                                AS "channelNameFi",
       chnl."SV"                                AS "channelNameSv",
       chnl."EN"                                AS "channelNameEn",
       onl."FI"                                 AS "organizationNameFi",
       onl."SV"                                 AS "organizationNameSv",
       onl."EN"                                 AS "organizationNameEn",
       chal."StreetAddressComplete_FI"          AS "addressFi",
       chal."StreetAddressComplete_SV"          AS "addressSv",
       chal."StreetAddressComplete_EN"          AS "addressEn",
       chal."AdditionalInformation_FI"          AS "addressAdditionalInformationFi",
       chal."AdditionalInformation_SV"          AS "addressAdditionalInformationSv",
       chal."AdditionalInformation_EN"          AS "addressAdditionalInformationEn",
       chal."MunicipalityCode"                  AS "municipalityCode",
       chal."MunicipalityName_FI"               AS "municipalityNameFi",
       chal."MunicipalityName_SV"               AS "municipalityNameSv",
       chal."MunicipalityName_EN"               AS "municipalityNameEn",
       chp."FI"                                 AS "phoneNumbersFi",
       chp."SV"                                 AS "phoneNumbersSv",
       chp."EN"                                 AS "phoneNumbersEn",
       shgt."normalOpeningHoursFi",
       shgt."normalOpeningHoursSv",
       shgt."normalOpeningHoursEn",
       shgt."exceptionalOpeningHoursFi",
       shgt."exceptionalOpeningHoursSv",
       shgt."exceptionalOpeningHoursEn",
       shgt."bankholidayOpeningHoursFi",
       shgt."bankholidayOpeningHoursSv",
       shgt."bankholidayOpeningHoursEn",
       shgt."overmidnightOpeningHoursFi",
       shgt."overmidnightOpeningHoursSv",
       shgt."overmidnightOpeningHoursEn",
       chs."ServiceId"                          AS "serviceId",
       snl."FI"                                 AS "serviceNameFi",
       snl."SV"                                 AS "serviceNameSv",
       snl."EN"                                 AS "serviceNameEn",
       aco."State"                              AS "coordinateState",
       aco."Type"                               AS "coordinateType",
       aco."Location3067"                       AS location
FROM geo."mv_ChannelAddressLan" chal
         LEFT JOIN geo."mv_ChannelNameLan" chnl ON chal."VersionedId" = chnl."VersionedId"
         LEFT JOIN geo."mv_OrganizationNameLan" onl ON chal."OrganizationId" = onl."OrganizationId"
         JOIN geo."mv_AddressCoordinate" aco ON chal."AddressId" = aco."AddressId"
         LEFT JOIN geo."mv_ChannelCurrentlyOpen" cco ON chal."VersionedId" = cco."VersionedId"
         LEFT JOIN geo."mv_ChannelPhoneGroupLan" chp ON chal."RootId" = chp."RootId"
         LEFT JOIN geo."mv_ServiceHoursGroupedTypeLan" shgt ON chal."RootId" = shgt."RootId"
         LEFT JOIN geo."mv_ChannelService" chs ON chal."RootId" = chs."ChannelId"
         LEFT JOIN geo."mv_ServiceNameLan" snl ON chs."ServiceId" = snl."ServiceId";

alter materialized view geo."mv_layer_serviceChannel" owner to postgres;

create index if not exists "mv_layer_serviceChannel_addressId_idx"
    on geo."mv_layer_serviceChannel" ("addressId");

create index if not exists "mv_layer_serviceChannel_location_idx"
    on geo."mv_layer_serviceChannel" using gist (location);

create index if not exists "mv_layer_serviceChannel_rootId_idx"
    on geo."mv_layer_serviceChannel" ("rootId");

create index if not exists "mv_layer_serviceChannel_versionedId_idx"
    on geo."mv_layer_serviceChannel" ("versionedId");

create materialized view if not exists geo."mv_layer_serviceChannelServiceJson" as
SELECT chal."VersionedId"                       AS "versionedId",
       chal."RootId"                            AS "rootId",
       chal."OrganizationId"                    AS "organizationId",
       chal."AddressId"                         AS "addressId",
       chal."AddressType"                       AS "addressType",
       COALESCE(cco."CurrentlyOpen", '-'::text) AS "currentlyOpen",
       chnl."FI"                                AS "channelNameFi",
       chnl."SV"                                AS "channelNameSv",
       chnl."EN"                                AS "channelNameEn",
       onl."FI"                                 AS "organizationNameFi",
       onl."SV"                                 AS "organizationNameSv",
       onl."EN"                                 AS "organizationNameEn",
       chal."StreetAddressComplete_FI"          AS "addressFi",
       chal."StreetAddressComplete_SV"          AS "addressSv",
       chal."StreetAddressComplete_EN"          AS "addressEn",
       chal."AdditionalInformation_FI"          AS "addressAdditionalInformationFi",
       chal."AdditionalInformation_SV"          AS "addressAdditionalInformationSv",
       chal."AdditionalInformation_EN"          AS "addressAdditionalInformationEn",
       chal."MunicipalityCode"                  AS "municipalityCode",
       chal."MunicipalityName_FI"               AS "municipalityNameFi",
       chal."MunicipalityName_SV"               AS "municipalityNameSv",
       chal."MunicipalityName_EN"               AS "municipalityNameEn",
       chp."FI"                                 AS "phoneNumbersFi",
       chp."SV"                                 AS "phoneNumbersSv",
       chp."EN"                                 AS "phoneNumbersEn",
       shgt."normalOpeningHoursFi",
       shgt."normalOpeningHoursSv",
       shgt."normalOpeningHoursEn",
       shgt."exceptionalOpeningHoursFi",
       shgt."exceptionalOpeningHoursSv",
       shgt."exceptionalOpeningHoursEn",
       shgt."bankholidayOpeningHoursFi",
       shgt."bankholidayOpeningHoursSv",
       shgt."bankholidayOpeningHoursEn",
       shgt."overmidnightOpeningHoursFi",
       shgt."overmidnightOpeningHoursSv",
       shgt."overmidnightOpeningHoursEn",
       js."Services"                            AS services,
       aco."State"                              AS "coordinateState",
       aco."Type"                               AS "coordinateType",
       aco."Location3067"                       AS location
FROM geo."mv_ChannelAddressLan" chal
         LEFT JOIN geo."mv_ChannelNameLan" chnl ON chal."VersionedId" = chnl."VersionedId"
         LEFT JOIN geo."mv_OrganizationNameLan" onl ON chal."OrganizationId" = onl."OrganizationId"
         JOIN geo."mv_AddressCoordinate" aco ON chal."AddressId" = aco."AddressId"
         LEFT JOIN geo."mv_ChannelCurrentlyOpen" cco ON chal."VersionedId" = cco."VersionedId"
         LEFT JOIN geo."mv_ChannelPhoneGroupLan" chp ON chal."RootId" = chp."RootId"
         LEFT JOIN geo."mv_ServiceHoursGroupedTypeLan" shgt ON chal."RootId" = shgt."RootId"
         LEFT JOIN geo."mv_ChannelServiceJson" js ON chal."RootId" = js."RootId";

alter materialized view geo."mv_layer_serviceChannelServiceJson" owner to postgres;

create index if not exists "mv_layer_serviceChannelServiceJson_addressId_idx"
    on geo."mv_layer_serviceChannelServiceJson" ("addressId");

create index if not exists "mv_layer_serviceChannelServiceJson_location_idx"
    on geo."mv_layer_serviceChannelServiceJson" using gist (location);

create index if not exists "mv_layer_serviceChannelServiceJson_rootId_idx"
    on geo."mv_layer_serviceChannelServiceJson" ("rootId");

create index if not exists "mv_layer_serviceChannelServiceJson_versionedId_idx"
    on geo."mv_layer_serviceChannelServiceJson" ("versionedId");

create materialized view if not exists geo."mv_dataStore_standardServiceHoursLan" as
SELECT shb."RootId",
       shb."ServiceHoursId",
       shb."From",
       shb."To",
       geo.replace_opening_type(shb."OpeningType") AS "OpeningType",
       ai."FI"                                     AS "AdditionalInformation_FI",
       ai."SV"                                     AS "AdditionalInformation_SV",
       ai."EN"                                     AS "AdditionalInformation_EN",
       opd."Monday",
       opd."Tuesday",
       opd."Wednesday",
       opd."Thursday",
       opd."Friday",
       opd."Saturday",
       opd."Sunday"
FROM geo."mv_ServiceHoursBase" shb
         LEFT JOIN geo."mv_ServiceHoursAdditionalInformationLan" ai ON shb."ServiceHoursId" = ai."ServiceHoursId"
         LEFT JOIN geo."mv_OpeningHoursDay" opd ON shb."ServiceHoursId" = opd."OpeningHourId"
WHERE shb."Type" = 'Standard'::text;

alter materialized view geo."mv_dataStore_standardServiceHoursLan" owner to postgres;

create index if not exists "mv_dataStore_standardServiceHoursLan_RootId_idx"
    on geo."mv_dataStore_standardServiceHoursLan" ("RootId");

create unique index if not exists "mv_dataStore_standardServiceHoursLan_ServiceHoursId_idx"
    on geo."mv_dataStore_standardServiceHoursLan" ("ServiceHoursId");

create materialized view if not exists geo."mv_dataStore_specialServiceHoursLan" as
SELECT shb."RootId",
       shb."ServiceHoursId",
       sh."From",
       sh."To",
       ai."FI" AS "AdditionalInformation_FI",
       ai."SV" AS "AdditionalInformation_SV",
       ai."EN" AS "AdditionalInformation_EN"
FROM geo."mv_ServiceHoursBase" shb
         JOIN geo."mv_ServiceHoursSpecial" sh ON shb."ServiceHoursId" = sh."ServiceHoursId" AND sh."LanguageCode" = 'en'::text
         LEFT JOIN geo."mv_ServiceHoursAdditionalInformationLan" ai ON shb."ServiceHoursId" = ai."ServiceHoursId";

alter materialized view geo."mv_dataStore_specialServiceHoursLan" owner to postgres;

create index if not exists "mv_dataStore_specialServiceHoursLan_RootId_idx"
    on geo."mv_dataStore_specialServiceHoursLan" ("RootId");

create unique index if not exists "mv_dataStore_specialServiceHoursLan_ServiceHoursId_idx"
    on geo."mv_dataStore_specialServiceHoursLan" ("ServiceHoursId");

create materialized view if not exists geo."mv_dataStore_exceptionalServiceHoursLan" as
SELECT shb."RootId",
       shb."ServiceHoursId",
       shb."From",
       shb."To",
       geo.replace_opening_type(shb."OpeningType") AS "OpeningType",
       oh."OpeningHours",
       ai."FI"                                     AS "AdditionalInformation_FI",
       ai."SV"                                     AS "AdditionalInformation_SV",
       ai."EN"                                     AS "AdditionalInformation_EN"
FROM geo."mv_ServiceHoursBase" shb
         LEFT JOIN geo."mv_OpeningHours" oh ON shb."ServiceHoursId" = oh."OpeningHourId"
         LEFT JOIN geo."mv_ServiceHoursAdditionalInformationLan" ai ON shb."ServiceHoursId" = ai."ServiceHoursId"
WHERE shb."Type" = 'Exception'::text;

alter materialized view geo."mv_dataStore_exceptionalServiceHoursLan" owner to postgres;

create index if not exists "mv_dataStore_exceptionalServiceHoursLan_RootId_idx"
    on geo."mv_dataStore_exceptionalServiceHoursLan" ("RootId");

create unique index if not exists "mv_dataStore_exceptionalServiceHoursLan_ServiceHoursId_idx"
    on geo."mv_dataStore_exceptionalServiceHoursLan" ("ServiceHoursId");
