drop materialized view if exists geo."mv_layer_serviceChannel";
drop materialized view if exists geo."mv_layer_serviceChannelServiceJson";
drop materialized view if exists geo."mv_layer_serviceChannelLanguage";
drop materialized view if exists geo."mv_dataStore_standardServiceHours";
drop materialized view if exists geo."mv_dataStore_standardServiceHoursLan";
drop materialized view if exists geo."mv_dataStore_exceptionalServiceHours";
drop materialized view if exists geo."mv_dataStore_exceptionalServiceHoursLan";
drop materialized view if exists geo."mv_dataStore_holidayServiceHours";
drop materialized view if exists geo."mv_dataStore_holidayServiceHoursLan";
drop materialized view if exists geo."mv_ServiceHoursGroupedTypeLan";
drop materialized view if exists geo."mv_ServiceHoursGroupedType";
drop materialized view if exists geo."mv_ServiceHoursGroupedLan";
drop materialized view if exists geo."mv_ServiceHoursGrouped";

----------------------------------------------------- Function for replacing of openingType-----------------------------
------------------------------------------------------------------------------------------------------------------------
create or replace function geo.replace_opening_type(openingType text)
    returns text language sql strict immutable parallel safe as $$
select
    case lower(openingType)
        when 'isnonstop' then 'alwaysOpen'
        when 'onreservation' then 'forReservation'
        when 'isclosed' then 'isClosed'
        else null
        end;
$$;

----------------------------------------------------- Service hours grouped --------------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceHoursGrouped" as
select
    "RootId",
    "Type",
    "LanguageCode",
    string_agg(case when "Type" = 'Holiday' then "HolidayName" else "AdditionalInformation" end || "Validity" || coalesce ("OpeningType", "OpeningHours"),  E'\n' order by "OrderNumber") as "ServiceHours"
from (
         select
             shbl."RootId",
             shbl."Type",
             shbl."OrderNumber",
             shbl."LanguageCode",
             case
                 when shbl."Type" = 'Special' then ''
                 when shbl."From" is not null and shbl."To" is null then '[' || shbl."From" || ']: '
                 when shbl."From" is not null and shbl."To" is not null then '[' || shbl."From" || ' - ' || shbl."To" || ']: '
                 else ''
                 end as "Validity",
--		coalesce(coalesce (ai."AdditionalInformation", ai_default."AdditionalInformation")  || ': ', '') as "AdditionalInformation",
             coalesce(ai."AdditionalInformation"  || ': ', '') as "AdditionalInformation",
             coalesce(coalesce(hn."Name", hn_default."Name") || ': ', '') as "HolidayName",
             geo.opening_hour_type(shbl."OpeningType", shbl."LanguageCode") as "OpeningType",
             coalesce(coalesce(ohg."DailyOpeningHours", ohg_default."DailyOpeningHours"), '') as "OpeningHours"
         from  (select shb.*, sl."Code" as "LanguageCode" from geo."mv_ServiceHoursBase" shb,  geo."SupportedLanguages" sl) shbl
                   left join geo."mv_ServiceHoursAdditionalInformation" ai on shbl."ServiceHoursId" = ai."ServiceHoursId" and shbl."LanguageCode" = ai."LanguageCode"
--	left join geo."mv_ServiceHoursAdditionalInformation" ai_default on shbl."ServiceHoursId" = ai_default."ServiceHoursId" and ai_default."LanguageCode" = 'fi'
                   left join geo."mv_HolidayName" hn on shbl."HolidayId" = hn."HolidayId" and shbl."LanguageCode" = hn."LanguageCode"
                   left join geo."mv_HolidayName" hn_default on shbl."HolidayId" = hn_default."HolidayId" and hn_default."LanguageCode" = 'fi'
                   left join geo."mv_OpeningHoursGrouped" ohg on shbl."ServiceHoursId" = ohg."OpeningHourId" and shbl."LanguageCode" = ohg."LanguageCode"
                   left join geo."mv_OpeningHoursGrouped" ohg_default on shbl."ServiceHoursId" = ohg_default."OpeningHourId" and ohg_default."LanguageCode" = 'fi'
     ) t
group by "RootId", "Type", "LanguageCode"
with data;
create unique index on geo."mv_ServiceHoursGrouped" ("RootId", "Type", "LanguageCode");
create index on geo."mv_ServiceHoursGrouped" ("RootId");

----------------------------------------------------- Service hours grouped type ---------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceHoursGroupedType" as
select
    "RootId",
    "LanguageCode",
    min("ServiceHours"::text) filter (where "Type" = 'Standard'::text) as "normalOpeningHours",
    min("ServiceHours"::text) filter (where "Type" = 'Exception'::text) as "exceptionalOpeningHours",
    min("ServiceHours"::text) filter (where "Type" = 'Holiday'::text) as "bankholidayOpeningHours",
    min("ServiceHours"::text) filter (where "Type" = 'Special'::text) as "overmidnightOpeningHours"
from geo."mv_ServiceHoursGrouped"
group by "RootId", "LanguageCode";
create unique index on geo."mv_ServiceHoursGroupedType"("RootId", "LanguageCode");


----------------------------------------------------- Service hours grouped Lan ----------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceHoursGroupedLan" as
select
    "RootId",
    "Type",
    min("ServiceHours"::text) filter (where "LanguageCode" = 'en'::text) as "EN",
    min("ServiceHours"::text) filter (where "LanguageCode" = 'fi'::text) as "FI",
    min("ServiceHours"::text) filter (where "LanguageCode" = 'sv'::text) as "SV"
from geo."mv_ServiceHoursGrouped"
group by "RootId", "Type";
create unique index on geo."mv_ServiceHoursGroupedLan" ("RootId", "Type");
create index on geo."mv_ServiceHoursGroupedLan" ("RootId");

----------------------------------------------------- Service hours grouped type Lan -----------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceHoursGroupedTypeLan" as
select
    "RootId",
    min("FI"::text) filter (where "Type" = 'Standard'::text) as "normalOpeningHoursFi",
    min("SV"::text) filter (where "Type" = 'Standard'::text) as "normalOpeningHoursSv",
    min("EN"::text) filter (where "Type" = 'Standard'::text) as "normalOpeningHoursEn",
    min("FI"::text) filter (where "Type" = 'Exception'::text) as "exceptionalOpeningHoursFi",
    min("SV"::text) filter (where "Type" = 'Exception'::text) as "exceptionalOpeningHoursSv",
    min("EN"::text) filter (where "Type" = 'Exception'::text) as "exceptionalOpeningHoursEn",
    min("FI"::text) filter (where "Type" = 'Holiday'::text) as "bankholidayOpeningHoursFi",
    min("SV"::text) filter (where "Type" = 'Holiday'::text) as "bankholidayOpeningHoursSv",
    min("EN"::text) filter (where "Type" = 'Holiday'::text) as "bankholidayOpeningHoursEn",
    min("FI"::text) filter (where "Type" = 'Special'::text) as "overmidnightOpeningHoursFi",
    min("SV"::text) filter (where "Type" = 'Special'::text) as "overmidnightOpeningHoursSv",
    min("EN"::text) filter (where "Type" = 'Special'::text) as "overmidnightOpeningHoursEn"
from geo."mv_ServiceHoursGroupedLan"
group by "RootId"
with data;
create unique index on geo."mv_ServiceHoursGroupedTypeLan"("RootId");

------------------------------------------------------------------------------------------------------------------------
-- layer and datastore views
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_layer_serviceChannelServiceJson" as
select
    chal."VersionedId" as "versionedId",
    chal."RootId" as "rootId",
    chal."OrganizationId" as "organizationId",
    chal."AddressId" as "addressId",
    chal."AddressType" as "addressType",
    coalesce(cco."CurrentlyOpen", '-') as "currentlyOpen",
    chnl."FI" as "channelNameFi",
    chnl."SV" as "channelNameSv",
    chnl."EN" as "channelNameEn",
    onl."FI" as "organizationNameFi",
    onl."SV" as "organizationNameSv",
    onl."EN" as "organizationNameEn",
    chal."StreetAddressComplete_FI" as "addressFi",
    chal."StreetAddressComplete_SV" as "addressSv",
    chal."StreetAddressComplete_EN" as "addressEn",
    chal."AdditionalInformation_FI" as "addressAdditionalInformationFi",
    chal."AdditionalInformation_SV" as "addressAdditionalInformationSv",
    chal."AdditionalInformation_EN" as "addressAdditionalInformationEn",
    chal."MunicipalityCode" as "municipalityCode",
    chal."MunicipalityName_FI" as "municipalityNameFi",
    chal."MunicipalityName_SV" as "municipalityNameSv",
    chal."MunicipalityName_EN" as "municipalityNameEn",
    chp."FI" as "phoneNumbersFi",
    chp."SV" as "phoneNumbersSv",
    chp."EN" as "phoneNumbersEn",
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
    js."Services" as "services",
    aco."State" as "coordinateState",
    aco."Type" as "coordinateType",
    aco."Location3067" as "location"
from geo."mv_ChannelAddressLan" chal
         left join geo."mv_ChannelNameLan" chnl on chal."VersionedId" = chnl."VersionedId"
         left join geo."mv_OrganizationNameLan" onl on chal."OrganizationId" = onl."OrganizationId"
         join geo."mv_AddressCoordinate" aco on chal."AddressId" = aco."AddressId"
         left join geo."mv_ChannelCurrentlyOpen" cco on chal."VersionedId" = cco."VersionedId"
         left join geo."mv_ChannelPhoneGroupLan" chp on chal."RootId" = chp."RootId"
         left join geo."mv_ServiceHoursGroupedTypeLan" shgt on chal."RootId" = shgt."RootId"
         left join geo."mv_ChannelServiceJson" as js on chal."RootId" = js."RootId"
with data;
create index on geo."mv_layer_serviceChannelServiceJson"("rootId");
create index on geo."mv_layer_serviceChannelServiceJson"("versionedId");
create index on geo."mv_layer_serviceChannelServiceJson"("addressId");
create index on geo."mv_layer_serviceChannelServiceJson" using gist ("location");

create materialized view geo."mv_layer_serviceChannel" as
select
    chal."VersionedId" as "versionedId",
    chal."RootId" as "rootId",
    chal."OrganizationId" as "organizationId",
    chal."AddressId" as "addressId",
    chal."AddressType" as "addressType",
    coalesce(cco."CurrentlyOpen", '-') as "currentlyOpen",
    chnl."FI" as "channelNameFi",
    chnl."SV" as "channelNameSv",
    chnl."EN" as "channelNameEn",
    onl."FI" as "organizationNameFi",
    onl."SV" as "organizationNameSv",
    onl."EN" as "organizationNameEn",
    chal."StreetAddressComplete_FI" as "addressFi",
    chal."StreetAddressComplete_SV" as "addressSv",
    chal."StreetAddressComplete_EN" as "addressEn",
    chal."AdditionalInformation_FI" as "addressAdditionalInformationFi",
    chal."AdditionalInformation_SV" as "addressAdditionalInformationSv",
    chal."AdditionalInformation_EN" as "addressAdditionalInformationEn",
    chal."MunicipalityCode" as "municipalityCode",
    chal."MunicipalityName_FI" as "municipalityNameFi",
    chal."MunicipalityName_SV" as "municipalityNameSv",
    chal."MunicipalityName_EN" as "municipalityNameEn",
    chp."FI" as "phoneNumbersFi",
    chp."SV" as "phoneNumbersSv",
    chp."EN" as "phoneNumbersEn",
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
    chs."ServiceId" as "serviceId",
    snl."FI" as "serviceNameFi",
    snl."SV" as "serviceNameSv",
    snl."EN" as "serviceNameEn",
    aco."State" as "coordinateState",
    aco."Type" as "coordinateType",
    aco."Location3067" as "location"
from geo."mv_ChannelAddressLan" chal
         left join geo."mv_ChannelNameLan" chnl on chal."VersionedId" = chnl."VersionedId"
         left join geo."mv_OrganizationNameLan" onl on chal."OrganizationId" = onl."OrganizationId"
         join geo."mv_AddressCoordinate" aco on chal."AddressId" = aco."AddressId"
         left join geo."mv_ChannelCurrentlyOpen" cco on chal."VersionedId" = cco."VersionedId"
         left join geo."mv_ChannelPhoneGroupLan" chp on chal."RootId" = chp."RootId"
         left join geo."mv_ServiceHoursGroupedTypeLan" shgt on chal."RootId" = shgt."RootId"
         left join geo."mv_ChannelService" chs on chal."RootId" = chs."ChannelId"
         left join geo."mv_ServiceNameLan" snl on chs."ServiceId" = snl."ServiceId"
with data;
create index on geo."mv_layer_serviceChannel"("rootId");
create index on geo."mv_layer_serviceChannel"("versionedId");
create index on geo."mv_layer_serviceChannel"("addressId");
create index on geo."mv_layer_serviceChannel" using gist ("location");

create materialized view geo."mv_layer_serviceChannelLanguage" as
select
    cha."VersionedId" as "versionedId",
    cha."RootId" as "rootId",
    cha."OrganizationId" as "organizationId",
    cha."AddressId" as "addressId",
    cha."AddressType" as "addressType",
    coalesce(cco."CurrentlyOpen", '-') as "currentlyOpen",
    cha."LanguageCode" as "languageCode",
    coalesce (chn."Name" , chn_default."Name") as "channelName",
    coalesce (ona."Name" , ona_default."Name") as "organizationName",
    coalesce (cha."StreetAddressComplete", cha_default."StreetAddressComplete") as "streetAddress",
    coalesce (cha."AdditionalInformation", cha_default."AdditionalInformation") as "addressAdditionalInformation",
    cha."MunicipalityCode" as "municipalityCode",
    coalesce (cha."MunicipalityName", cha_default."MunicipalityName") as "municipalityName",
    coalesce (chp."NumberComplete", chp_default."NumberComplete") as "phoneNumbers",
    chs."ServiceId" as "serviceId",
    coalesce (sen."Name" , sen_default."Name") as "serviceName",
    sh."normalOpeningHours",
    sh."overmidnightOpeningHours",
    sh."bankholidayOpeningHours",
    sh."exceptionalOpeningHours",
    aco."State" as "coordinateState",
    aco."Type" as "coordinateType",
    aco."Location3067" as "location"
from geo."mv_ChannelAddress" cha
         join geo."mv_ChannelAddress" cha_default on cha."AddressId" = cha_default."AddressId" and cha_default."LanguageCode" = 'fi'
         join geo."mv_AddressCoordinate" aco on cha."AddressId" = aco."AddressId"
         left join geo."mv_ChannelCurrentlyOpen" cco on cha."VersionedId" = cco."VersionedId"
         left join geo."mv_ChannelName" chn on cha."VersionedId" = chn."VersionedId" and cha."LanguageCode" = chn."LanguageCode"
         left join geo."mv_ChannelName" chn_default on cha."VersionedId" = chn_default."VersionedId" and chn_default."LanguageCode" = 'fi'
         left join geo."mv_OrganizationName" ona on cha."OrganizationId" = ona."OrganizationId" and cha."LanguageCode" = ona."LanguageCode"
         left join geo."mv_OrganizationName" ona_default on cha."OrganizationId" = ona_default."OrganizationId" and ona_default."LanguageCode" = 'fi'
         left join geo."mv_ChannelPhoneGroup" chp on cha."RootId" = chp."RootId" and cha."LanguageCode" = chp."LanguageCode"
         left join geo."mv_ChannelPhoneGroup" chp_default on cha."RootId" = chp_default."RootId" and chp_default."LanguageCode" = 'fi'
         left join geo."mv_ChannelService" chs on cha."RootId" = chs."ChannelId"
         left join geo."mv_ServiceName" sen on chs."ServiceId" = sen."ServiceId" and cha."LanguageCode" = sen."LanguageCode"
         left join geo."mv_ServiceName" sen_default on chs."ServiceId" = sen_default."ServiceId" and sen_default."LanguageCode" = 'fi'
         left join geo."mv_ServiceHoursGroupedType" sh on cha."RootId" = sh."RootId" and cha."LanguageCode" = sh."LanguageCode"
with data;
create index on geo."mv_layer_serviceChannelLanguage"("rootId");
create index on geo."mv_layer_serviceChannelLanguage"("versionedId");
create index on geo."mv_layer_serviceChannelLanguage"("addressId");
create index on geo."mv_layer_serviceChannelLanguage" using gist ("location");

create materialized view geo."mv_dataStore_standardServiceHours" as
select
    "RootId",
    "ServiceHoursId",
    "From",
    "To",
    geo.replace_opening_type("OpeningType") as "OpeningType"
from  geo."mv_ServiceHoursBase"
where "Type" = 'Standard'
with data;
create unique index on geo."mv_dataStore_standardServiceHours"("ServiceHoursId");
create index on geo."mv_dataStore_standardServiceHours"("RootId");

create materialized view geo."mv_dataStore_standardServiceHoursLan" as
select
    shb."RootId",
    shb."ServiceHoursId",
    shb."From",
    shb."To",
    geo.replace_opening_type(shb."OpeningType") as "OpeningType",
    ai."FI" as "AdditionalInformation_FI",
    ai."SV" as "AdditionalInformation_SV",
    ai."EN" as "AdditionalInformation_EN",
    opd."Monday",
    opd."Tuesday",
    opd."Wednesday",
    opd."Thursday",
    opd."Friday",
    opd."Saturday",
    opd."Sunday"
from  geo."mv_ServiceHoursBase" shb
          left join geo."mv_ServiceHoursAdditionalInformationLan" ai on shb."ServiceHoursId" = ai."ServiceHoursId"
          left join geo."mv_OpeningHoursDay" opd on shb."ServiceHoursId" = opd."OpeningHourId"
where "Type" = 'Standard'
with data;
create unique index on geo."mv_dataStore_standardServiceHoursLan"("ServiceHoursId");
create index on geo."mv_dataStore_standardServiceHoursLan"("RootId");

create materialized view geo."mv_dataStore_exceptionalServiceHours" as
select
    "RootId",
    "ServiceHoursId",
    "From",
    "To",
    geo.replace_opening_type(shb."OpeningType") as "OpeningType",
    "OpeningHours"
from  geo."mv_ServiceHoursBase" shb
          left join geo."mv_OpeningHours" oh on shb."ServiceHoursId" = oh."OpeningHourId"
where "Type" = 'Exception'
with data;
create unique index on geo."mv_dataStore_exceptionalServiceHours"("ServiceHoursId");
create index on geo."mv_dataStore_exceptionalServiceHours"("RootId");

create materialized view geo."mv_dataStore_exceptionalServiceHoursLan" as
select
    shb."RootId",
    shb."ServiceHoursId",
    shb."From",
    shb."To",
    geo.replace_opening_type(shb."OpeningType") as "OpeningType",
    oh."OpeningHours",
    ai."FI" as "AdditionalInformation_FI",
    ai."SV" as "AdditionalInformation_SV",
    ai."EN" as "AdditionalInformation_EN"
from  geo."mv_ServiceHoursBase" shb
          left join geo."mv_OpeningHours" oh on shb."ServiceHoursId" = oh."OpeningHourId"
          left join geo."mv_ServiceHoursAdditionalInformationLan" ai on shb."ServiceHoursId" = ai."ServiceHoursId"
where "Type" = 'Exception'
with data;
create unique index on geo."mv_dataStore_exceptionalServiceHoursLan"("ServiceHoursId");
create index on geo."mv_dataStore_exceptionalServiceHoursLan"("RootId");

create materialized view geo."mv_dataStore_holidayServiceHours" as
select
    shb."RootId",
    shb."ServiceHoursId",
    shb."HolidayId",
    shb."HolidayDate",
    geo.replace_opening_type(shb."OpeningType") as "OpeningType",
    oh."OpeningHours"
from  geo."mv_ServiceHoursBase" shb
          left join geo."mv_OpeningHours" oh on shb."ServiceHoursId" = oh."OpeningHourId"
where shb."HolidayId" is not null
with data;
create unique index on geo."mv_dataStore_holidayServiceHours"("ServiceHoursId");
create index on geo."mv_dataStore_holidayServiceHours"("RootId");

create materialized view geo."mv_dataStore_holidayServiceHoursLan" as
select
    shb."RootId",
    shb."ServiceHoursId",
    shb."HolidayId",
    shb."HolidayDate",
    geo.replace_opening_type(shb."OpeningType") as "OpeningType",
    oh."OpeningHours",
    hn."FI" as "HolidayName_FI",
    hn."SV" as "HolidayName_SV",
    hn."EN" as "HolidayName_EN"
from  geo."mv_ServiceHoursBase" shb
          left join geo."mv_OpeningHours" oh on shb."ServiceHoursId" = oh."OpeningHourId"
          left join geo."mv_HolidayNameLan" hn on shb."HolidayId" = hn."HolidayId"
where shb."HolidayId" is not null
with data;
create unique index on geo."mv_dataStore_holidayServiceHoursLan"("ServiceHoursId");
create index on geo."mv_dataStore_holidayServiceHoursLan"("RootId");

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

end;$$ language plpgsql;