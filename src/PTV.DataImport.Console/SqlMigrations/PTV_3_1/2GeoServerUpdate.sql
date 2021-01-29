------------------------------------------------------------------------------------------------------------------------
-- drop unused public geo views
drop view if exists public."geoAddressArea";
drop view if exists public."geoAddressMunicipality";
drop view if exists public."geoServiceChannelEmail";
drop view if exists public."geoAddressStreetGrouped";
drop view if exists public."geoAddressStreetBase";
drop view if exists public."geoAreaMunicipality";
drop view if exists public."geoChannelAreaPublished";
drop view if exists public."geoChannelMunicipalityPublished";
drop view if exists public."geoChannelNameGrouped";
drop view if exists public."geoChannelNameBase";
drop view if exists public."geoForeignAddress";
drop view if exists public."geoOrganizationChannelName";
drop view if exists public."geoOrganizationName";
drop view if exists public."geoOrganizationNameGrouped";
drop view if exists public."geoOrganizationNameBase";
drop view if exists public."geoServiceChannelBase";
drop view if exists public."geoServiceChannelName";
drop view if exists public."geoServiceChannelPhone";
drop view if exists public."geoServiceChannelWebPage";
------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
-- drop unused lc_* materialized views
------------------------------------------------------------------------------------------------------------------------
drop materialized view if exists geo."ChannelIds";
drop materialized view if exists geo."lc_LocationChannel";
drop materialized view if exists geo."lc_LocationChannelLan";
drop materialized view if exists geo."lc_ChannelCoordinateGroup";
drop materialized view if exists geo."lc_ChannelAddressCoordinateGroup";
drop materialized view if exists geo."lc_ChannelAddressCoordinate";
drop materialized view if exists geo."lc_ChannelNameLan";
drop materialized view if exists geo."lc_ChannelName";
drop materialized view if exists geo."lc_OrganizationNameLan";
drop materialized view if exists geo."lc_OrganizationName";
drop materialized view if exists geo."lc_ChannelMunicipalityNameLan";
drop materialized view if exists geo."lc_AddressMunicipalityGroup";
drop materialized view if exists geo."lc_Address";
drop materialized view if exists geo."lc_ChannelAddress";
drop materialized view if exists geo."lc_ChannelServiceNameLan";
drop materialized view if exists geo."lc_ServiceNameLan";
drop materialized view if exists geo."lc_ServiceName";
drop materialized view if exists geo."lc_ChannelService";
drop materialized view if exists geo."lc_ChannelCurrentlyOpen";
drop materialized view if exists geo."lc_StandardOpeningHours";
drop materialized view if exists geo."lc_ChannelService";
drop materialized view if exists geo."lc_MunicipalityNameLan";
drop materialized view if exists geo."lc_MunicipalityName";

drop materialized view if exists geo."mv_layer_serviceChannelBase";
drop materialized view if exists geo."mv_layer_serviceChannel";
drop materialized view if exists geo."mv_layer_serviceChannelLanguage";
drop materialized view if exists geo."mv_layer_locationChannelAddress";

drop materialized view if exists geo."mv_dataStore_municipalityName";
drop materialized view if exists geo."mv_dataStore_serviceLocationAddress";
drop materialized view if exists geo."mv_dataStore_streetAdditionalInformation";
drop materialized view if exists geo."mv_dataStore_streetAddress";
drop materialized view if exists geo."mv_dataStore_serviceLocationLan";
drop materialized view if exists geo."mv_dataStore_serviceLocationLanService";
drop materialized view if exists geo."mv_dataStore_serviceLocationLanAddress";

drop materialized view if exists geo."mv_ChannelPhoneGroupLan";
drop materialized view if exists geo."mv_ChannelPhoneGroup";
drop materialized view if exists geo."mv_ChannelPhoneLan";
drop materialized view if exists geo."mv_ChannelPhone";
drop materialized view if exists geo."mv_LocationChannel";
drop materialized view if exists geo."mv_LocationChannelLan";
drop materialized view if exists geo."mv_LocationChannelGroup";
drop materialized view if exists geo."mv_ChannelCoordinateGroup";
drop materialized view if exists geo."mv_AddressCoordinateGroup";
drop materialized view if exists geo."mv_AddressCoordinate";
drop materialized view if exists geo."mv_ChannelAddressLan";
drop materialized view if exists geo."mv_ChannelAddress";
drop materialized view if exists geo."mv_ChannelAddressBase";
drop materialized view if exists geo."mv_ChannelCurrentlyOpen";
drop materialized view if exists geo."mv_StandardOpeningHours";
drop materialized view if exists geo."mv_ChannelNameLan";
drop materialized view if exists geo."mv_ChannelName";
drop materialized view if exists geo."mv_OrganizationNameLan";
drop materialized view if exists geo."mv_OrganizationName";
drop materialized view if exists geo."mv_ServiceNameLan";
drop materialized view if exists geo."mv_ServiceName";
drop materialized view if exists geo."mv_ChannelService";
drop materialized view if exists geo."SupportedLanguages";

drop materialized view if exists geo."mv_ChannelEmailLan";
drop materialized view if exists geo."mv_ChannelEmail";

------------------------------------------------------------------------------------------------------------------------
-- refresh base view
refresh materialized view concurrently geo."LocationChannel";

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Supported Languages ----------------------------------------------
-- -> 
-- <- 
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."SupportedLanguages" as
select "Id", "Code" from "Language"
where "Code" in ('fi', 'sv', 'en')
with data;

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Service---------------------------------------------------
create materialized view geo."mv_ChannelService" as
select
    lc."RootId" as "ChannelId",
    ssc."ServiceId"
from geo."LocationChannel" lc
join "ServiceServiceChannel" ssc on lc."RootId" = ssc."ServiceChannelId" 
with data;
create unique index on geo."mv_ChannelService" ("ChannelId", "ServiceId");
create index on geo."mv_ChannelService" ("ChannelId");
create index on geo."mv_ChannelService" ("ServiceId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Name -----------------------------------------------------
-- > geo."LocationChannel"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelName" as
select
    "VersionedId",
    "LanguageCode",
    "Name"
from 
(
    select
        "Name"."Id" as "VersionedId",
        "Name"."LanguageCode",
        case when "DisplayType"."Type" = 'AlternateName' 
            then "AlternateName"."Name"
            else "Name"."Name"
        end "Name"
    from -- name
    (
        select
            "ServiceChannelVersionedId" as "Id",
            "Name",
            "LocalizationId",
            lan."Code" as "LanguageCode"
        from "ServiceChannelName" scn
        join "Language" lan on scn."LocalizationId" = lan."Id"
        where "TypeId" = (select "Id" from "NameType" where "Code" = 'Name')
    ) "Name"
    left join -- alternate name
    (
        select
            "ServiceChannelVersionedId" as "Id",
            "Name",
            "LocalizationId"
        from "ServiceChannelName"
        where "TypeId" = (select "Id" from "NameType" where "Code" = 'AlternateName')
    ) as "AlternateName" on "Name"."Id" = "AlternateName"."Id" and "Name"."LocalizationId" = "AlternateName"."LocalizationId"
    left join -- display type
    (
        select
            dnt."ServiceChannelVersionedId" as "Id",
            dnt."LocalizationId",
            nt."Code" as "Type"
        from "ServiceChannelDisplayNameType" dnt
        join "NameType" nt on dnt."DisplayNameTypeId" = nt."Id"
    ) "DisplayType" on "Name"."Id" = "DisplayType"."Id" and "Name"."LocalizationId" = "DisplayType"."LocalizationId"
) t
where exists (select 1 from geo."LocationChannel" glc where glc."VersionedId" = t."VersionedId")
with data;
create index on geo."mv_ChannelName" ("VersionedId");
create unique index on geo."mv_ChannelName"("VersionedId", "LanguageCode");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Name Language---------------------------------------------
-- > geo."mv_ChannelName"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelNameLan" as
select
    "VersionedId",
    min("Name"::text) filter (where "LanguageCode" = 'fi'::text) as "FI",
    min("Name"::text) filter (where "LanguageCode" = 'sv'::text) as "SV",
    min("Name"::text) filter (where "LanguageCode" = 'en'::text) as "EN"
from geo."mv_ChannelName"
group by "VersionedId"
with data;
create unique index on geo."mv_ChannelNameLan" ("VersionedId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Organization Name ------------------------------------------------
-- > geo."LocationChannel"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_OrganizationName" as
select 
    t."OrganizationId",
    t."Name",
    t."LanguageCode"
from 
(
    select 
        org."RootId" as "OrganizationId",
        ona."Name",
        lan."Code" as "LanguageCode"
    from 
    (
        select 
            "UnificRootId" as "RootId",
            "Id"
        from "OrganizationVersioned"
        where "PublishingStatusId" = (select "Id" from "PublishingStatusType" where "Code" = 'Published'::text)
    ) org
    join "OrganizationName" ona on org."Id" = ona."OrganizationVersionedId"
    join "OrganizationDisplayNameType" odt on org."Id" = odt."OrganizationVersionedId" and ona."LocalizationId" = odt."LocalizationId" and ona."TypeId" = odt."DisplayNameTypeId"
    join "Language" lan on ona."LocalizationId" = lan."Id"
) t
where ( exists ( select 1 from geo."LocationChannel" lc where lc."OrganizationId" = t."OrganizationId"))
with data;
create unique index on geo."mv_OrganizationName" ("OrganizationId", "LanguageCode");
create index on geo."mv_OrganizationName" ("OrganizationId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Organization Name Language ---------------------------------------
-- > geo."mv_OrganizationName"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_OrganizationNameLan" as
select 
    "OrganizationId",
    min("Name"::text) filter (where "LanguageCode" = 'fi'::text) as "FI",
    min("Name"::text) filter (where "LanguageCode" = 'sv'::text) as "SV",
    min("Name"::text) filter (where "LanguageCode" = 'en'::text) as "EN"
from geo."mv_OrganizationName"
group by "OrganizationId"
with data;
create unique index on geo."mv_OrganizationNameLan" ("OrganizationId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Service Name -----------------------------------------------------
-- > geo."LocationChannel", geo."mv_ChannelService"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceName" as
select 
    sev."UnificRootId" as "ServiceId",
    lan."Code" as "LanguageCode",
    sen."Name"
from "ServiceVersioned" sev
join 
( 
    select 
        "ServiceVersionedId",
        "Name",
        "LocalizationId"
    from "ServiceName"
    where "TypeId" = ( select "Id" from "NameType" where "Code" = 'Name'::text)
) sen on sev."Id" = sen."ServiceVersionedId"
join "Language" lan on lan."Id" = sen."LocalizationId"
where sev."PublishingStatusId" = ( select "Id" from "PublishingStatusType" where "Code" = 'Published'::text) 
  and ( exists ( select 1 from geo."mv_ChannelService" where "ServiceId" = sev."UnificRootId"))
with data;
create unique index on geo."mv_ServiceName" ("ServiceId", "LanguageCode");
create index on geo."mv_ServiceName" ("ServiceId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Service Name Language --------------------------------------------
-- > geo."mv_ServiceName"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceNameLan" as 
select 
    "ServiceId",
    min("Name"::text) filter (where "LanguageCode" = 'fi'::text) as "FI",
    min("Name"::text) filter (where "LanguageCode" = 'sv'::text) as "SV",
    min("Name"::text) filter (where "LanguageCode" = 'en'::text) as "EN"
from geo."mv_ServiceName"
group by "ServiceId"
with data;
create unique index on geo."mv_ServiceNameLan" ("ServiceId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- StandardOpeningHours ---------------------------------------------
-- > geo."LocationChannel"
-- < geo."mv_ChannelCurrentlyOpen"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_StandardOpeningHours" as
select
    t."VersionedId",
    t."ServiceHoursId",
    t."ServiceHoursOrder",
    dot."DayFrom",
    dot."DayTo",
    dot."From",
    dot."To",
    dot."OrderNumber",
    t."IsNonStop"
from 
(
     select
         sch."ServiceChannelVersionedId" as "VersionedId",
         sh."Id" as "ServiceHoursId",
         sh."OpeningHoursFrom",
         sh."OpeningHoursTo",
         sh."OrderNumber" as "ServiceHoursOrder",
         sh."IsNonStop"
     from "ServiceHours" sh
     join "ServiceChannelServiceHours" sch on sh."Id" = sch."ServiceHoursId"
     join geo."LocationChannel" cha on sch."ServiceChannelVersionedId" = cha."VersionedId"
     where "ServiceHourTypeId" = (select "Id" from "ServiceHourType" where "Code" = 'Standard')
       and ("OpeningHoursFrom" is null or "OpeningHoursFrom"::date <= current_date)
       and ("OpeningHoursTo"is null or "OpeningHoursTo"::date >= current_date)
) t
left join "DailyOpeningTime" dot on t."ServiceHoursId" = dot."OpeningHourId"
where dot."DayFrom" = date_part('isodow', current_date)-1  or t."IsNonStop" = true
with data;

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Standard Opening Hours Group -------------------------------------------
-- > geo."mv_StandardOpeningHours"
-- <  
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelCurrentlyOpen" as
select
    "VersionedId",
    bool_or("CurrentlyOpen")::text as "CurrentlyOpen"
from 
 (
     select
         "VersionedId",
         "From"::time <= current_timestamp::time and "To"::time > current_timestamp::time as "CurrentlyOpen"
     from geo."mv_StandardOpeningHours"
 ) t group by "VersionedId"
with data;
create unique index on geo."mv_ChannelCurrentlyOpen" ("VersionedId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Address Base ---------------------------------------------
-- -> 
-- <- geo."LocationChannel"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelAddressBase" as
select
    lc."VersionedId",
    lc."RootId",
    lc."OrganizationId",
    sca."AddressId",
    adt."Code" as "AddressType",
    adr."OrderNumber",
    case when sa."AddressId" is null
        then oa."PostalCodeId"
        else sa."PostalCodeId"
    end as "PostalCodeId",
    case when sa."AddressId" is null
        then oa."PostalCode"
        else sa."PostalCode"
    end as "PostalCode",
    case when sa."AddressId" is null
        then oa."MunicipalityId"
        else sa."MunicipalityId"
    end as "MunicipalityId",
    case when sa."AddressId" is null
        then oa."MunicipalityCode"
        else sa."MunicipalityCode"
    end as "MunicipalityCode"
from geo."LocationChannel" lc
join "ServiceChannelAddress" sca on lc."VersionedId" = sca."ServiceChannelVersionedId"
join "Address" adr on adr."Id" = sca."AddressId"
join "AddressType" adt on adr."TypeId" = adt."Id"
left join -- street address
(
    select
        ap."AddressId",
        ap."PostalCodeId",
        pc."Code" as "PostalCode",
        mun."Id" as "MunicipalityId",
        mun."Code" as "MunicipalityCode"
    from "ClsAddressPoint" ap
    join "PostalCode" pc on ap."PostalCodeId" = pc."Id"
    join "Municipality" mun on pc."MunicipalityId" = mun."Id"
) sa on adr."Id" = sa."AddressId"
left join -- other address
(
    select
        ao."AddressId",
        ao."PostalCodeId",
        pc."Code" as "PostalCode",
        mun."Id" as "MunicipalityId",
        mun."Code" as "MunicipalityCode"
    from "AddressOther" ao
    join "PostalCode" pc on ao."PostalCodeId" = pc."Id"
    join "Municipality" mun on pc."MunicipalityId" = mun."Id"
) oa on adr."Id" = oa."AddressId"
where sca."CharacterId" = (select "Id" from "AddressCharacter" where "Code" = 'Visiting')
with data;
create unique index on geo."mv_ChannelAddressBase" ("RootId", "AddressId");
create unique index on geo."mv_ChannelAddressBase" ("VersionedId", "AddressId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Address --------------------------------------------------
-- -> 
-- <- geo."mv_ChannelAddressBase", geo."SupportedLanguages"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelAddress" as
select
    al."VersionedId",
    al."RootId",
    al."OrganizationId",
    al."AddressId",
    al."AddressType",
    al."OrderNumber",
    al."LanguageCode",
    al."PostalCodeId",
    al."PostalCode",
    pcn."Name" as "PostalName",
    al."MunicipalityId",
    al."MunicipalityCode",
    mun."Name" as "MunicipalityName",
    aai."Text" as "AdditionalInformation",
    sa."StreetName",
    sa."StreetNumber",
    sa."StreetAddress",
    (sa."StreetAddress"::text || ', '::text || al."PostalCode"::text || ' '::text || pcn."Name") as "StreetAddressComplete"
from (select chab.*, sl."Code" as "LanguageCode", sl."Id" as "LanguageId" from geo."mv_ChannelAddressBase" chab, geo."SupportedLanguages" sl) al
left join "PostalCodeName" pcn on al."PostalCodeId" = pcn."PostalCodeId" and al."LanguageId" = pcn."LocalizationId"
left join "MunicipalityName" mun on al."MunicipalityId" = mun."MunicipalityId" and al."LanguageId" = mun."LocalizationId"
left join "AddressAdditionalInformation" aai on al."AddressId" = aai."AddressId" and al."LanguageId" = aai."LocalizationId"
left join 
(
    select 
        ap."AddressId" as "AddressId", 
        asn."LocalizationId" as "LocalizationId",
        asn."Name" as "StreetName",
        ap."StreetNumber" as "StreetNumber",
        (asn."Name"::text || ' '::text || ap."StreetNumber"::text) as "StreetAddress"
    from "ClsAddressPoint" ap
    join "ClsAddressStreetName" asn on ap."AddressStreetId" = asn."ClsAddressStreetId"
) sa on al."AddressId" = sa."AddressId" and al."LanguageId" = sa."LocalizationId"
with data;
create unique index on geo."mv_ChannelAddress" ("RootId", "AddressId", "LanguageCode");
create unique index on geo."mv_ChannelAddress" ("VersionedId", "AddressId", "LanguageCode");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Address with languages -----------------------------------
-- -> 
-- <- geo."mv_ChannelAddress"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelAddressLan" as
select
    "VersionedId",
    "RootId",
    "OrganizationId",
    "AddressId",
    "AddressType",
    "OrderNumber",
    "PostalCodeId",
    "PostalCode",
    min("PostalName"::text) filter (where "LanguageCode" = 'fi'::text) as "PostalName_FI",
    min("PostalName"::text) filter (where "LanguageCode" = 'sv'::text) as "PostalName_SV",
    min("PostalName"::text) filter (where "LanguageCode" = 'en'::text) as "PostalName_EN",
    "MunicipalityId",
    "MunicipalityCode",
    min("MunicipalityName"::text) filter (where "LanguageCode" = 'fi'::text) as "MunicipalityName_FI",
    min("MunicipalityName"::text) filter (where "LanguageCode" = 'sv'::text) as "MunicipalityName_SV",
    min("MunicipalityName"::text) filter (where "LanguageCode" = 'en'::text) as "MunicipalityName_EN",
    min("AdditionalInformation"::text) filter (where "LanguageCode" = 'fi'::text) as "AdditionalInformation_FI",
    min("AdditionalInformation"::text) filter (where "LanguageCode" = 'sv'::text) as "AdditionalInformation_SV",
    min("AdditionalInformation"::text) filter (where "LanguageCode" = 'en'::text) as "AdditionalInformation_EN",
    min("StreetAddress"::text) filter (where "LanguageCode" = 'fi'::text) as "StreetAddress_FI",
    min("StreetAddress"::text) filter (where "LanguageCode" = 'sv'::text) as "StreetAddress_SV",
    min("StreetAddress"::text) filter (where "LanguageCode" = 'en'::text) as "StreetAddress_EN",
    min("StreetAddressComplete"::text) filter (where "LanguageCode" = 'fi'::text) as "StreetAddressComplete_FI",
    min("StreetAddressComplete"::text) filter (where "LanguageCode" = 'sv'::text) as "StreetAddressComplete_SV",
    min("StreetAddressComplete"::text) filter (where "LanguageCode" = 'en'::text) as "StreetAddressComplete_EN"
from geo."mv_ChannelAddress"
group by "VersionedId", "RootId", "OrganizationId", "AddressId", "AddressType", "OrderNumber", "PostalCodeId", "PostalCode", "MunicipalityId", "MunicipalityCode"
with data;
create unique index on geo."mv_ChannelAddressLan" ("RootId", "AddressId");
create unique index on geo."mv_ChannelAddressLan" ("VersionedId", "AddressId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Address Coordinate -----------------------------------------------
-- -> geo."mv_ChannelAddress"
-- <- geo."mv_ChannelAddressCoordinateGroup"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_AddressCoordinate" as
select
    aco."RelatedToId" as "AddressId",
    cty."Code" as "Type",
    aco."CoordinateState" as "State",
    st_setsrid(st_point(aco."Longitude", aco."Latitude"), 3067)::geometry(Point,3067) as "Location3067"
from "AddressCoordinate" aco
join "CoordinateType" cty on aco."TypeId" = cty."Id"
where lower("CoordinateState") in ('ok', 'enteredbyuser', 'enteredbyar')
    and exists (select 1 from geo."mv_ChannelAddressBase" gca where gca."AddressId" = aco."RelatedToId")
with data;
create index on geo."mv_AddressCoordinate" ("AddressId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Address Coordinate Group -----------------------------------------
-- > geo."mv_ChannelAddressCoordinate"
-- < geo."mv_ChannelCoordinateGroup";
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_AddressCoordinateGroup" as
select
    "AddressId",
    ST_Collect("Location3067") as "Location3067",
    array_to_string(array_agg("Type"), ', ') as "Coordinates"
from 
(
    select distinct
        "AddressId",
        "Location3067",
        "Type" || ': ' || ST_AsText("Location3067") as "Type"
    from geo."mv_AddressCoordinate" coo
) t
group by "AddressId"
with data;
create unique index on geo."mv_AddressCoordinateGroup" ("AddressId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Coordinate Group -----------------------------------------
-- > geo."mv_ChannelAddressBase", geo."mv_AddressCoordinateGroup"
-- > geo."mv_ChannelAddressCoordinateGroup", geo."mv_LocationChannelLan"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelCoordinateGroup" as
select
    chab."VersionedId",
    ST_Multi(ST_Union(acg."Location3067")) as "Location3067"
from geo."mv_ChannelAddressBase" chab
join geo."mv_AddressCoordinateGroup" acg on chab."AddressId" = acg."AddressId"
group by chab."VersionedId"
with data;
create unique index on geo."mv_ChannelCoordinateGroup" ("VersionedId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel (base for view) ------------------------------------------
-- > geo."LocationChannel", geo."mv_ChannelCoordinateGroup", geo."lc_ChannelCurrentlyOpen"
-- > geo."ChannelIds"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_LocationChannelGroup" as
select
    lc."VersionedId",
    lc."RootId",
    lc."OrganizationId",
    coalesce(cco."CurrentlyOpen", '-') as "CurrentlyOpen",
    chcg."Location3067"
from geo."LocationChannel" lc
join geo."mv_ChannelCoordinateGroup" chcg on lc."VersionedId" = chcg."VersionedId"
left join geo."mv_ChannelCurrentlyOpen" cco on lc."VersionedId" = cco."VersionedId"
with data;
create unique index on geo."mv_LocationChannelGroup" ("VersionedId");
create unique index on geo."mv_LocationChannelGroup" ("RootId");
create index on geo."mv_LocationChannelGroup" ("OrganizationId");
create index on geo."mv_LocationChannelGroup" using gist ("Location3067");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Phones ---------------------------------------------------
-- > geo."LocationChannel"
-- > 
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelPhone" as 
select 
    lc."RootId",
    p."Id" as "PhoneId",
    lan."Code" as "LanguageCode",
    coalesce(scp."OrderNumber", 0) AS "OrderNumber",
    dc."Code" as "DialCode",
    p."Number",
    p."AdditionalInformation",
    case when p."AdditionalInformation" is null 
        then coalesce(dc."Code" || ' '::text, '') || p."Number"
        else coalesce(dc."Code" || ' '::text, '') || p."Number" || ', ' || p."AdditionalInformation" 
    end as "NumberComplete"
from geo."LocationChannel" lc
join "ServiceChannelPhone" scp on lc."VersionedId" = scp."ServiceChannelVersionedId" 
join "Phone" p on scp."PhoneId" = p."Id" 
left join "DialCode" dc on p."PrefixNumberId" = dc."Id" 
join geo."SupportedLanguages" lan on lan."Id" = p."LocalizationId"
where p."TypeId" = (select "Id" from "PhoneNumberType" where "Code" = 'Phone'::text)
with data;
create index on geo."mv_ChannelPhone" ("RootId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Phones Language-------------------------------------------
-- > geo."mv_ChannelPhone"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelPhoneLan" as 
select 
    "RootId",
    "PhoneId",
    min("NumberComplete"::text) filter (where "LanguageCode" = 'fi'::text) as "FI",
    min("NumberComplete"::text) filter (where "LanguageCode" = 'sv'::text) as "SV",
    min("NumberComplete"::text) filter (where "LanguageCode" = 'en'::text) as "EN"
from geo."mv_ChannelPhone"
group by "RootId", "PhoneId"
with data;
create unique index on geo."mv_ChannelPhoneLan" ("PhoneId");
create index on geo."mv_ChannelPhoneLan" ("RootId");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Phones Grouped -------------------------------------------
-- > geo."mv_ChannelPhone"
-- > 
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelPhoneGroup" as 
select 
	"RootId", 
	"LanguageCode", 
	string_agg("NumberComplete", E'\n' order by "OrderNumber") as "NumberComplete"
from geo."mv_ChannelPhone"
group by "RootId", "LanguageCode"
with data;
create unique index on geo."mv_ChannelPhoneGroup" ("RootId", "LanguageCode");

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Phones Language-------------------------------------------
-- > geo."mv_ChannelPhoneGroup"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelPhoneGroupLan" as 
select 
    "RootId",
    min("NumberComplete"::text) filter (where "LanguageCode" = 'fi'::text) as "FI",
    min("NumberComplete"::text) filter (where "LanguageCode" = 'sv'::text) as "SV",
    min("NumberComplete"::text) filter (where "LanguageCode" = 'en'::text) as "EN"
from geo."mv_ChannelPhoneGroup"
group by "RootId"
with data;
create unique index on geo."mv_ChannelPhoneGroupLan" ("RootId");

------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
-- FIX layer Channel Ids (Simple Feature) .. use small letters for attributes
------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Ids ------------------------------------------------------
-- > geo."AddressMunicipalityGroup"
-- > 
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."ChannelIds" as
select
    lch."RootId" as "rootId",
    lch."VersionedId" as "versionedId",
    lch."OrganizationId" as "organizationId",
    lch."CurrentlyOpen" as "currentlyOpen",
    services."ServiceIds" as "serviceIds",
    addresses."AddressIds" as "addressIds",
    lch."Location3067" as "location3067"
from geo."mv_LocationChannelGroup" as lch
left join
(
    select
        "ChannelId" as "VersionedId",
        string_agg("ServiceId"::text, ', '::text) as "ServiceIds"
    from geo."mv_ChannelService"
    group by "ChannelId"
) as services on lch."VersionedId" = services."VersionedId"
left join
(
    select
        "VersionedId",
        string_agg("AddressId"::text, ', '::text) as "AddressIds"
    from geo."mv_ChannelAddressBase"
    group by "VersionedId"
) addresses on lch."VersionedId" = addresses."VersionedId"
with data;
create index on geo."ChannelIds" ("versionedId");
create index on geo."ChannelIds" ("rootId");
create unique index on geo."ChannelIds"("rootId", "versionedId");
------------------------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------------------------
-- layer and datastore views
-----------------------------------------------------------------------------------------------------------------------
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
join geo."mv_AddressCoordinate" chac ON lchab."AddressId" = chac."AddressId"
with data;
create index on geo."mv_layer_serviceChannelBase"("rootId");
create index on geo."mv_layer_serviceChannelBase"("versionedId");
create index on geo."mv_layer_serviceChannelBase"("addressId");

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
    aco."State" as "coordinateState",
    aco."Type" as "coordinateType",
    aco."Location3067" as "location"
from geo."mv_ChannelAddressLan" chal
left join geo."mv_ChannelNameLan" chnl on chal."VersionedId" = chnl."VersionedId"
left join geo."mv_OrganizationNameLan" onl on chal."OrganizationId" = onl."OrganizationId"
join geo."mv_AddressCoordinate" aco on chal."AddressId" = aco."AddressId"
left join geo."mv_ChannelCurrentlyOpen" cco on chal."VersionedId" = cco."VersionedId"
left join geo."mv_ChannelPhoneGroupLan" chp on chal."RootId" = chp."RootId"
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
with data;
create index on geo."mv_layer_serviceChannelLanguage"("rootId");
create index on geo."mv_layer_serviceChannelLanguage"("versionedId");
create index on geo."mv_layer_serviceChannelLanguage"("addressId");
create index on geo."mv_layer_serviceChannelLanguage" using gist ("location");

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
join geo."mv_AddressCoordinate" aco on cha."AddressId" = aco."AddressId"
left join geo."mv_ChannelCurrentlyOpen" cco on cha."VersionedId" = cco."VersionedId"
with data;
create index on geo."mv_layer_locationChannelAddress"("rootId");
create index on geo."mv_layer_locationChannelAddress"("versionedId");
create index on geo."mv_layer_locationChannelAddress"("addressId");
create index on geo."mv_layer_locationChannelAddress" using gist ("location");

create materialized view geo."mv_dataStore_municipalityName" as
select distinct on (mn."Name")
	mn."MunicipalityId",
	mn."Name",
	lan."Code" as "LanguageCode"
from "MunicipalityName" mn
join "Language" lan on lan."Id" = mn."LocalizationId" 
order by mn."Name", lan."OrderNumber"
with data;
create index on geo."mv_dataStore_municipalityName" ("MunicipalityId");

create materialized view geo."mv_dataStore_serviceLocationAddress" as
select 
	"RootId",
	"AddressType",
	"MunicipalityCode",
	"MunicipalityId",
	"AddressId"
from "geo"."mv_ChannelAddressBase"
with data;
create index on geo."mv_dataStore_serviceLocationAddress" ("RootId");
create index on geo."mv_dataStore_serviceLocationAddress" ("AddressId");

create materialized view geo."mv_dataStore_streetAdditionalInformation" as
select 
	"AddressId",
	"LanguageCode",
	"AdditionalInformation"
from geo."mv_ChannelAddress" 
where "AdditionalInformation" is not null
with data;
create index on geo."mv_dataStore_streetAdditionalInformation" ("AddressId");

create materialized view geo."mv_dataStore_streetAddress" as
select 
	"AddressId",
	"LanguageCode",
	"StreetAddressComplete" as "StreetAddress"
from geo."mv_ChannelAddress" 
where "StreetAddressComplete" is not null
with data;
create index on geo."mv_dataStore_streetAddress" ("AddressId");

create materialized view geo."mv_dataStore_serviceLocationLan" as
select 
	lchg."RootId",
	lchg."VersionedId",
	lchg."OrganizationId",
	lchg."CurrentlyOpen",
	chnl."FI" as "ChannelNameFi",
    chnl."SV" as "ChannelNameSv",
    chnl."EN" as "ChannelNameEn",
    onl."FI" as "OrganizationNameFi",
    onl."SV" as "OrganizationNameSv",
    onl."EN" as "OrganizationNameEn",
	lchg."Location3067"
from geo."mv_LocationChannelGroup" lchg
left join geo."mv_ChannelNameLan" chnl on lchg."VersionedId" = chnl."VersionedId"
left join geo."mv_OrganizationNameLan" onl on lchg."OrganizationId" = onl."OrganizationId"
with data;
create unique index on geo."mv_dataStore_serviceLocationLan" ("RootId");
create unique index on geo."mv_dataStore_serviceLocationLan" ("VersionedId");

create materialized view geo."mv_dataStore_serviceLocationLanService" as
select 
	chs."ChannelId",
	chs."ServiceId",
	sen."FI",
	sen."SV",
	sen."EN"
from geo."mv_ChannelService" chs
join geo."mv_ServiceNameLan" sen on chs."ServiceId" = sen."ServiceId"
with data;
create unique index on geo."mv_dataStore_serviceLocationLanService" ("ChannelId", "ServiceId");

create materialized view geo."mv_dataStore_serviceLocationLanAddress" as
select 
	"RootId",
	"AddressId",
	"AddressType",
	"MunicipalityCode",
	"MunicipalityName_FI",
	"MunicipalityName_SV",
	"MunicipalityName_EN",
	"StreetAddressComplete_FI" as "StreetAddress_FI",
	"StreetAddressComplete_SV" as "StreetAddress_SV",
	"StreetAddressComplete_EN" as "StreetAddress_EN",
	"AdditionalInformation_FI",
	"AdditionalInformation_SV",
	"AdditionalInformation_EN"
from geo."mv_ChannelAddressLan" 
order by "RootId", "OrderNumber"
with data;
create index on geo."mv_dataStore_serviceLocationLanAddress" ("RootId");
create index on geo."mv_dataStore_serviceLocationLanAddress" ("AddressId");

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
		
	refresh materialized view concurrently geo."ChannelIds";
	refresh materialized view geo."mv_layer_serviceChannelBase";
	refresh materialized view geo."mv_layer_serviceChannel";
	refresh materialized view geo."mv_layer_serviceChannelLanguage";
	refresh materialized view geo."mv_layer_locationChannelAddress";
	refresh materialized view geo."mv_dataStore_municipalityName";
	refresh materialized view geo."mv_dataStore_serviceLocationAddress";
	refresh materialized view geo."mv_dataStore_streetAdditionalInformation";
	refresh materialized view geo."mv_dataStore_streetAddress";
	refresh materialized view geo."mv_dataStore_serviceLocationLan";
	refresh materialized view geo."mv_dataStore_serviceLocationLanService";
	refresh materialized view geo."mv_dataStore_serviceLocationLanAddress";
	
end;$$ language plpgsql;