drop materialized view geo."mv_layer_serviceChannelBase";
drop materialized view geo."mv_layer_serviceChannel";
drop materialized view geo."mv_layer_serviceChannelLanguage";
drop materialized view geo."mv_layer_serviceChannelServiceJson";
drop materialized view geo."mv_layer_locationChannelAddress";

drop materialized view if exists geo."mv_AddressCoordinateDistinct";
create materialized view geo."mv_AddressCoordinateDistinct" as
select distinct on (1)
    aco."RelatedToId" as "AddressId",
    cty."Code" as "Type",
    aco."CoordinateState" as "State",
    st_setsrid(st_point(aco."Longitude", aco."Latitude"), 3067)::geometry(Point,3067) AS "Location3067"
from "AddressCoordinate" aco
         join "CoordinateType" cty on aco."TypeId" = cty."Id"
where (lower(aco."CoordinateState") = any (array['ok'::text, 'enteredbyuser'::text, 'enteredbyar'::text]))
  and (exists ( select 1 from geo."mv_ChannelAddressBase" gca where gca."AddressId" = aco."RelatedToId"))
order by "AddressId", aco."Modified" desc
with data;
create unique index on geo."mv_AddressCoordinateDistinct"("AddressId");

create materialized view geo."mv_layer_serviceChannelBase" as
select
    lchab."VersionedId" as "versionedId",
    lchab."RootId" as "rootId",
    lchab."OrganizationId" as "organizationId",
    lchab."AddressId" as "addressId",
    lchab."AddressType" as "addressType",
    lchab."PostalCode" as "postalCode",
    lchab."MunicipalityCode" as "municipalityCode",
    chac."Type" as "coordinateType",
    chac."State" as "coordinateState",
    chac."Location3067" as "location"
from geo."mv_ChannelAddressBase" lchab
         join geo."mv_AddressCoordinateDistinct" chac ON lchab."AddressId" = chac."AddressId"
with data;
create index on geo."mv_layer_serviceChannelBase"("rootId");
create index on geo."mv_layer_serviceChannelBase"("versionedId");
create index on geo."mv_layer_serviceChannelBase"("addressId");

create materialized view geo."mv_layer_serviceChannel" as
select
    row_number() over() as "id",
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
         join geo."mv_AddressCoordinateDistinct" aco on chal."AddressId" = aco."AddressId"
         left join geo."mv_ChannelCurrentlyOpen" cco on chal."VersionedId" = cco."VersionedId"
         left join geo."mv_ChannelPhoneGroupLan" chp on chal."RootId" = chp."RootId"
         left join geo."mv_ServiceHoursGroupedTypeLan" shgt on chal."RootId" = shgt."RootId"
         left join geo."mv_ChannelService" chs on chal."RootId" = chs."ChannelId"
         left join geo."mv_ServiceNameLan" snl on chs."ServiceId" = snl."ServiceId"
with data;
create unique index on geo."mv_layer_serviceChannel"("id");
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
         join geo."mv_AddressCoordinateDistinct" aco on cha."AddressId" = aco."AddressId"
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
         join geo."mv_AddressCoordinateDistinct" aco on chal."AddressId" = aco."AddressId"
         left join geo."mv_ChannelCurrentlyOpen" cco on chal."VersionedId" = cco."VersionedId"
         left join geo."mv_ChannelPhoneGroupLan" chp on chal."RootId" = chp."RootId"
         left join geo."mv_ServiceHoursGroupedTypeLan" shgt on chal."RootId" = shgt."RootId"
         left join geo."mv_ChannelServiceJson" as js on chal."RootId" = js."RootId"
with data;
create index on geo."mv_layer_serviceChannelServiceJson"("rootId");
create index on geo."mv_layer_serviceChannelServiceJson"("versionedId");
create index on geo."mv_layer_serviceChannelServiceJson"("addressId");
create index on geo."mv_layer_serviceChannelServiceJson" using gist ("location");

create materialized view geo."mv_layer_locationChannelAddress" as
select
    cha."RootId" as "rootId",
    cha."VersionedId" as "versionedId",
    cha."AddressId" as "addressId",
    cha."AddressType" as "addressType",
    coalesce(cco."CurrentlyOpen", '-') as "currentlyOpen",
    cha."LanguageCode" as "languageCode",
    cha."StreetNumber" as "streetNumber",
    cha."StreetName" as "streetName",
    cha."AdditionalInformation" as "additionalInformation",
    cha."PostalCode" as "postalCode",
    cha."PostalName" as "postalName",
    cha."MunicipalityCode" as "municipalityCode",
    cha."MunicipalityName" as "municipalityName",
    aco."State" as "coordinateState",
    aco."Type" as "coordinateType",
    aco."Location3067" as "location"
from geo."mv_ChannelAddress" cha
         join geo."mv_AddressCoordinateDistinct" aco on cha."AddressId" = aco."AddressId"
         left join geo."mv_ChannelCurrentlyOpen" cco on cha."VersionedId" = cco."VersionedId"
with data;
create index on geo."mv_layer_locationChannelAddress"("rootId");
create index on geo."mv_layer_locationChannelAddress"("versionedId");
create index on geo."mv_layer_locationChannelAddress"("addressId");
create index on geo."mv_layer_locationChannelAddress" using gist ("location");

grant select on geo."mv_AddressCoordinateDistinct" to geoserver_user;
grant select on geo."mv_layer_serviceChannelBase" to geoserver_user;
grant select on geo."mv_layer_serviceChannel" to geoserver_user;
grant select on geo."mv_layer_serviceChannelLanguage" to geoserver_user;
grant select on geo."mv_layer_serviceChannelServiceJson" to geoserver_user;
grant select on geo."mv_layer_locationChannelAddress" to geoserver_user;

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

end;$$ language plpgsql;