
------------------------------------------------------------------------------------------------------------------------
-- refresh views
create unique index on geo."SupportedLanguages" ("Id");
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

------------------------------------------------------------------------------------------------------------------------
-- drop views
drop materialized view if exists geo."mv_dataStore_standardServiceHours";
drop materialized view if exists geo."mv_dataStore_standardServiceHoursLan";
drop materialized view if exists geo."mv_dataStore_exceptionalServiceHours";
drop materialized view if exists geo."mv_dataStore_exceptionalServiceHoursLan";
drop materialized view if exists geo."mv_dataStore_specialServiceHours";
drop materialized view if exists geo."mv_dataStore_specialServiceHoursLan";
drop materialized view if exists geo."mv_dataStore_holidayServiceHours";
drop materialized view if exists geo."mv_dataStore_holidayServiceHoursLan";
drop materialized view if exists geo."mv_layer_serviceChannel";
drop materialized view if exists geo."mv_layer_serviceClass";

drop materialized view if exists geo."mv_ServiceClass";
drop materialized view if exists geo."mv_ChannelServiceJson";
drop materialized view if exists geo."mv_ServiceHoursGroupedType";
drop materialized view if exists geo."mv_ServiceHoursGrouped";
drop materialized view if exists geo."mv_OpeningHoursGroupedLan";
drop materialized view if exists geo."mv_OpeningHoursGrouped";
drop materialized view if exists geo."mv_OpeningHoursDay";
drop materialized view if exists geo."mv_ServiceHoursSpecial";
drop materialized view if exists geo."mv_OpeningHoursSpecial";
drop materialized view if exists geo."mv_OpeningHours";
drop materialized view if exists geo."mv_ServiceHoursAdditionalInformationLan";
drop materialized view if exists geo."mv_ServiceHoursAdditionalInformation";
drop materialized view if exists geo."mv_HolidayNameLan";
drop materialized view if exists geo."mv_HolidayName";
drop materialized view if exists geo."mv_ServiceHoursBase";

------------------------------------------------------------------------------------------------------------------------
-- create functions
create or replace function geo.day_name_short(day_index integer, languageCode text)
returns text language sql strict immutable parallel safe as $$
	  select 
	  	case 
	  		when day_index = 0 and languageCode = 'en' then 'Mon'
	  		when day_index = 1 and languageCode = 'en' then 'Tue'
	  		when day_index = 2 and languageCode = 'en' then 'Wed'
	  		when day_index = 3 and languageCode = 'en' then 'Thu'
	  		when day_index = 4 and languageCode = 'en' then 'Fri'
	  		when day_index = 5 and languageCode = 'en' then 'Sat'
	  		when day_index = 6 and languageCode = 'en' then 'Sun'
	  		else null
    	end;
$$;

create or replace function geo.day_name_long(day_index integer, languageCode text)
returns text language sql strict immutable parallel safe as $$
  select 
	  	case 
	  		when day_index = 0 and languageCode = 'en' then 'Monday'
	  		when day_index = 1 and languageCode = 'en' then 'Tuesday'
	  		when day_index = 2 and languageCode = 'en' then 'Wednesday'
	  		when day_index = 3 and languageCode = 'en' then 'Thursday'
	  		when day_index = 4 and languageCode = 'en' then 'Friday'
	  		when day_index = 5 and languageCode = 'en' then 'Saturday'
	  		when day_index = 6 and languageCode = 'en' then 'Sunday'
	  		when day_index = 0 and languageCode = 'fi' then 'Maanantai'
	  		when day_index = 1 and languageCode = 'fi' then 'Tiistai'
	  		when day_index = 2 and languageCode = 'fi' then 'Keskiviikko'
	  		when day_index = 3 and languageCode = 'fi' then 'Torstai'
	  		when day_index = 4 and languageCode = 'fi' then 'Perjantai'
	  		when day_index = 5 and languageCode = 'fi' then 'Lauantai'
	  		when day_index = 6 and languageCode = 'fi' then 'Sunnuntai'
	  		when day_index = 0 and languageCode = 'sv' then 'Måndag'
	  		when day_index = 1 and languageCode = 'sv' then 'Tisdag'
	  		when day_index = 2 and languageCode = 'sv' then 'Onsdag'
	  		when day_index = 3 and languageCode = 'sv' then 'Torsdag'
	  		when day_index = 4 and languageCode = 'sv' then 'Fredag'
	  		when day_index = 5 and languageCode = 'sv' then 'Lördag'
	  		when day_index = 6 and languageCode = 'sv' then 'Söndag'
	  		else null
    	end;
$$;

create or replace function geo.opening_hour_type(code text, languageCode text)
returns text language sql strict immutable parallel safe as $$
	select
		case 
			when lower(code) = 'isclosed' and languageCode = 'fi' then 'Suljettu koko päivän'
			when lower(code) = 'isclosed' and languageCode = 'sv' then 'Stängt hela dagen'
			when lower(code) = 'isclosed' and languageCode = 'en' then 'Closed all day'
			when lower(code) = 'isnonstop' and languageCode = 'fi' then 'Aina avoinna (24/7)'
			when lower(code) = 'isnonstop' and languageCode = 'sv' then 'Alltid öppet (24/7)'
			when lower(code) = 'isnonstop' and languageCode = 'en' then 'Always open (24/7)'
			when lower(code) = 'onreservation' and languageCode = 'fi' then 'Avoinna ajanvarauksella'
			when lower(code) = 'onreservation' and languageCode = 'sv' then 'Öppet med tidbestallning'
			when lower(code) = 'onreservation' and languageCode = 'en' then 'Open with reservation only'
			else null
		end;
$$;

------------------------------------------------------------------------------------------------------------------------
-- drop layer and dataStore views
drop materialized view if exists geo."mv_layer_serviceChannelBase";
drop materialized view if exists geo."mv_layer_serviceChannelLanguage";
drop materialized view if exists geo."mv_layer_serviceChannel";
drop materialized view if exists geo."mv_layer_locationChannelAddress";
drop materialized view if exists geo."mv_dataStore_serviceLocationAddress";
drop materialized view if exists geo."mv_dataStore_serviceLocationLanAddress";
drop materialized view if exists geo."mv_dataStore_serviceLocationLan";
drop materialized view if exists geo."mv_dataStore_streetAdditionalInformation";
drop materialized view if exists geo."mv_dataStore_streetAddress";

-- drop views
drop materialized view geo."ChannelIds";
drop materialized view geo."mv_LocationChannelGroup";
drop materialized view geo."mv_ChannelCoordinateGroup";
drop materialized view geo."mv_AddressCoordinateGroup";
drop materialized view geo."mv_AddressCoordinate";
drop materialized view geo."mv_ChannelAddressLan";
drop materialized view geo."mv_ChannelAddress";
drop materialized view geo."mv_ChannelAddressBase";


------------------------------------------------------------------------------------------------------------------------
-- re-create views

------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Address Base ---------------------------------------------
-- -> geo."mv_ChannelAddress", geo."mv_AddressCoordinate", geo."mv_ChannelCoordinateGroup", geo."ChannelIds"
-- <- geo."LocationChannel"
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelAddressBase" as
select
    lc."VersionedId",
    lc."RootId",
    lc."OrganizationId",
    sca."AddressId",
    case adt."Code" 
    	when 'Street' then 'Single'
	    else adt."Code" 
	end as "AddressType",
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
	and exists (select 1 from "AddressCoordinate" ac where sca."AddressId" = ac."RelatedToId")
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

----------------------------------------------------- Service hours base -----------------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceHoursBase" as
select 
	scsh."ServiceChannelVersionedId" as "VersionedId",
	lc."RootId",
	scsh."ServiceHoursId" ,
	hsh."HolidayId",
	sh."OrderNumber" ,
	case when hsh."HolidayId" is null
		then false
		else true
	end as "IsHoliday",
	to_char(hd."Date", 'YYYY-MM-DD') as "HolidayDate",
	case when hsh."HolidayId" is null
		then sht."Code"
		else 'Holiday'
	end as "Type",
	to_char(sh."OpeningHoursFrom", 'YYYY-MM-DD') as "From",
	to_char(sh."OpeningHoursTo", 'YYYY-MM-DD') as "To",
	sh."IsClosed",
	sh."IsReservation",
	sh."IsNonStop",
	case 
		when "IsNonStop" = true then 'IsNonStop'
		when "IsClosed" = true then 'IsClosed'
		when "IsReservation" = true then 'OnReservation'
		else null
	end as "OpeningType"
from "ServiceChannelServiceHours" scsh
join geo."LocationChannel" lc on scsh."ServiceChannelVersionedId" = lc."VersionedId"
join "ServiceHours" sh on scsh."ServiceHoursId" = sh."Id" 
join "ServiceHourType" sht on sh."ServiceHourTypeId" = sht."Id" 
left join "HolidayServiceHours" hsh on scsh."ServiceHoursId" = hsh."ServiceHoursId" 
left join -- holiday date
(
	select "HolidayId", min("Date") as "Date"
	from "HolidayDate"
	where "Date" >= now()
	group by "HolidayId"
) hd on hsh."HolidayId" = hd."HolidayId"
order by "Type", sh."OrderNumber" 
with data;
create unique index on geo."mv_ServiceHoursBase"("VersionedId", "ServiceHoursId");
create index on geo."mv_ServiceHoursBase"("VersionedId");
create index on geo."mv_ServiceHoursBase"("ServiceHoursId");

----------------------------------------------------- Holidya name -----------------------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_HolidayName" as
select 
	h."Id" as "HolidayId",
	h."Code" ,
	sl."Code" as "LanguageCode",
	hn."Name" as "Name"
from "Holiday" h
join "HolidayName" hn on h."Id" = hn."TypeId" 
join geo."SupportedLanguages" sl on sl."Id" = hn."LocalizationId" 
where exists (select 1 from geo."mv_ServiceHoursBase" shb where shb."HolidayId" = h."Id")
with data;
create index on geo."mv_HolidayName"("HolidayId");

----------------------------------------------------- Holidya name lan -------------------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_HolidayNameLan" as
select 
	"HolidayId",
	min("Name") filter (where "LanguageCode"='fi') as "FI",
	min("Name") filter (where "LanguageCode"='sv') as "SV",
	min("Name") filter (where "LanguageCode"='en') as "EN"
from geo."mv_HolidayName"
group by "HolidayId"
with data;
create unique index on geo."mv_HolidayNameLan"("HolidayId");

----------------------------------------------------- Service hours additional information -----------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceHoursAdditionalInformation" as
select 
	shai."ServiceHoursId",
	shai."Text" as "AdditionalInformation",
	sl."Code" as "LanguageCode"
from "ServiceHoursAdditionalInformation" shai
join geo."SupportedLanguages" sl on shai."LocalizationId" = sl."Id" 
where exists (select 1 from geo."mv_ServiceHoursBase" shb where shai."ServiceHoursId" = shb."ServiceHoursId")
with data;
create index on geo."mv_ServiceHoursAdditionalInformation"("ServiceHoursId");

----------------------------------------------------- Service hours additional information lan -------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceHoursAdditionalInformationLan" as
select 
	"ServiceHoursId",
	min("AdditionalInformation") filter (where "LanguageCode"='fi') as "FI",
	min("AdditionalInformation") filter (where "LanguageCode"='sv') as "SV",
	min("AdditionalInformation") filter (where "LanguageCode"='en') as "EN"
from geo."mv_ServiceHoursAdditionalInformation"
group by "ServiceHoursId"
with data;
create unique index on geo."mv_ServiceHoursAdditionalInformationLan"("ServiceHoursId");

----------------------------------------------------- Opening hours ----------------------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_OpeningHours" as
select 
	"OpeningHourId",
	"DayFrom" as "Day",
	"DayTo" as "DayTo",
	geo.day_name_long("DayFrom", 'en') as "DayLong",
	geo.day_name_short("DayFrom", 'en') as "DayShort",
	string_agg(to_char("From", 'HH24:MI') || '-' || to_char("To", 'HH24:MI'), ', ' order by "OrderNumber") as "OpeningHours"
from "DailyOpeningTime" dot
where exists (select 1 from geo."mv_ServiceHoursBase" shb where dot."OpeningHourId" = shb."ServiceHoursId")
group by "OpeningHourId", "DayFrom", "DayTo"
order by "Day"
with data;
create unique index on geo."mv_OpeningHours"("OpeningHourId", "Day");
create index on geo."mv_OpeningHours"("OpeningHourId");

----------------------------------------------------- Opening hours special --------------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_OpeningHoursSpecial" as
select 
	dotl."OpeningHourId",
	dotl."LanguageCode",
	geo.day_name_long(dotl."DayFrom", dotl."LanguageCode") || ' ' || to_char(dotl."From", 'HH24:MI') as "From",
	coalesce(geo.day_name_long(dotl."DayTo",  dotl."LanguageCode"), '') || ' ' || to_char(dotl."To", 'HH24:MI') as "To"
from (select dot."OpeningHourId", dot."DayFrom", dot."DayTo", dot."From", dot."To", sl."Code" as "LanguageCode" from "DailyOpeningTime" dot, geo."SupportedLanguages" sl) dotl
join geo."mv_ServiceHoursBase" shb on dotl."OpeningHourId" = shb."ServiceHoursId" and shb."Type" = 'Special'
with data;
create unique index on geo."mv_OpeningHoursSpecial"("OpeningHourId", "LanguageCode");
create index on geo."mv_OpeningHoursSpecial"("OpeningHourId");

----------------------------------------------------- Service hours special --------------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceHoursSpecial" as
select 
	shb."ServiceHoursId",
	oh."LanguageCode",
	case when shb."From" is null 
		then oh."From"
		else shb."From" || ' ' || oh."From"
	end as "From",
	case when shb."To" is null
		then oh."To"
		else shb."To" || ' ' || oh."To"
	end as "To"
from  geo."mv_ServiceHoursBase" shb
join geo."mv_OpeningHoursSpecial" oh on shb."ServiceHoursId" = oh."OpeningHourId" 
with data;
create unique index on geo."mv_ServiceHoursSpecial"("ServiceHoursId", "LanguageCode");
create index on geo."mv_ServiceHoursSpecial"("ServiceHoursId");

----------------------------------------------------- Opening hours day-------------------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_OpeningHoursDay" as
select 
	"OpeningHourId",
	min("OpeningHours") filter (where "Day" = 0) as "Monday",
	min("OpeningHours") filter (where "Day" = 1) as "Tuesday",
	min("OpeningHours") filter (where "Day" = 2) as "Wednesday",
	min("OpeningHours") filter (where "Day" = 3) as "Thursday",
	min("OpeningHours") filter (where "Day" = 4) as "Friday",
	min("OpeningHours") filter (where "Day" = 5) as "Saturday",
	min("OpeningHours") filter (where "Day" = 6) as "Sunday"
from geo."mv_OpeningHours"
group by "OpeningHourId"
with data;
create unique index on geo."mv_OpeningHoursDay"("OpeningHourId");

----------------------------------------------------- Opening hours grouped --------------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_OpeningHoursGrouped" as
select 
	shb."Type",
	ohl."OpeningHourId",
	ohl."LanguageCode",
	string_agg(
		case shb."Type" 
			when 'Exception' then ohl."OpeningHours"
			when 'Holiday' then ohl."OpeningHours"
			when 'Special' then shs."From" || (case when shs."To" is null then '' else ' - ' || shs."To" end)
			when 'Standard' then (geo.day_name_long(ohl."Day", ohl."LanguageCode") || ': ' || ohl."OpeningHours") 
			else null 
		end, '; ' order by ohl."Day") as "DailyOpeningHours"
from (select oh."OpeningHourId", oh."Day", oh."OpeningHours", sl."Code" as "LanguageCode" from geo."mv_OpeningHours" oh, geo."SupportedLanguages" sl) ohl
join geo."mv_ServiceHoursBase" shb on shb."ServiceHoursId" = ohl."OpeningHourId" 
left join geo."mv_ServiceHoursSpecial" shs on ohl."OpeningHourId" = shs."ServiceHoursId" and ohl."LanguageCode" = shs."LanguageCode"
group by shb."Type", ohl."OpeningHourId", ohl."LanguageCode" 
with data;
create unique index on geo."mv_OpeningHoursGrouped"("OpeningHourId", "LanguageCode");

----------------------------------------------------- Opening hours grouped lan ----------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_OpeningHoursGroupedLan" as
select 
	"OpeningHourId",
	min("DailyOpeningHours") filter (where "LanguageCode"='fi') as "FI",
	min("DailyOpeningHours") filter (where "LanguageCode"='sv') as "SV",
	min("DailyOpeningHours") filter (where "LanguageCode"='en') as "EN"
from geo."mv_OpeningHoursGrouped"
group by "OpeningHourId"
with data;
create unique index on geo."mv_OpeningHoursGroupedLan"("OpeningHourId");

----------------------------------------------------- Service hours grouped --------------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceHoursGrouped" as
select 
	"RootId",
	"Type",
	string_agg(case when "Type" = 'Holiday' then "HolidayName_EN" else "AdditionalInformation_EN" end || "Validity" || coalesce ("OpeningType_EN", "OpeningHours_EN"),  E'\n' order by "OrderNumber") as "EN",
	string_agg(case when "Type" = 'Holiday' then "HolidayName_FI" else "AdditionalInformation_FI" end || "Validity" || coalesce ("OpeningType_FI", "OpeningHours_FI"),  E'\n' order by "OrderNumber") as "FI",
	string_agg(case when "Type" = 'Holiday' then "HolidayName_SV" else "AdditionalInformation_SV" end || "Validity" || coalesce ("OpeningType_SV", "OpeningHours_SV"),  E'\n' order by "OrderNumber") as "SV"
from (
	select 
		shb."RootId",
		shb."Type",
		shb."OrderNumber",
		geo.opening_hour_type("OpeningType", 'fi') as "OpeningType_FI",
		geo.opening_hour_type("OpeningType", 'sv') as "OpeningType_SV",
		geo.opening_hour_type("OpeningType", 'en') as "OpeningType_EN",
		coalesce (ai."EN" || ': ', '') as "AdditionalInformation_EN",
		coalesce (ai."FI" || ': ', '') as "AdditionalInformation_FI",
		coalesce (ai."SV" || ': ', '') as "AdditionalInformation_SV",
		coalesce (hnl."EN" || ': ', '') as "HolidayName_EN",
		coalesce (hnl."FI" || ': ', '') as "HolidayName_FI",
		coalesce (hnl."SV" || ': ', '') as "HolidayName_SV",
		case
			when "Type" = 'Special' then ''
			when shb."From" is not null and shb."To" is null then '[' || shb."From" || ']: '
			when shb."From" is not null and shb."To" is not null then '[' || shb."From" || ' - ' || shb."To" || ']: '
			else ''
		end as "Validity",
		coalesce (ohgl."EN", '') as "OpeningHours_EN",
		coalesce (ohgl."FI", '') as "OpeningHours_FI",
		coalesce (ohgl."SV", '') as "OpeningHours_SV"
	from  geo."mv_ServiceHoursBase" shb
	left join geo."mv_ServiceHoursAdditionalInformationLan" ai on shb."ServiceHoursId" = ai."ServiceHoursId"
	left join geo."mv_HolidayNameLan" hnl on shb."HolidayId" = hnl."HolidayId"
	left join geo."mv_OpeningHoursGroupedLan" ohgl on shb."ServiceHoursId" = ohgl."OpeningHourId" 
) t
group by "RootId", "Type"
with data;
create unique index on geo."mv_ServiceHoursGrouped" ("RootId", "Type");
create index on geo."mv_ServiceHoursGrouped" ("RootId");
;

----------------------------------------------------- Service hours grouped type ---------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceHoursGroupedType" as 
select 
    "RootId",
    min("FI"::text) filter (where "Type" = 'Standard'::text) as "standardOpeningHoursFi",
    min("SV"::text) filter (where "Type" = 'Standard'::text) as "standardOpeningHoursSv",
    min("EN"::text) filter (where "Type" = 'Standard'::text) as "standardOpeningHoursEn",
    min("FI"::text) filter (where "Type" = 'Exception'::text) as "exceptionalOpeningHoursFi",
    min("SV"::text) filter (where "Type" = 'Exception'::text) as "exceptionalOpeningHoursSv",
    min("EN"::text) filter (where "Type" = 'Exception'::text) as "exceptionalOpeningHoursEn",
    min("FI"::text) filter (where "Type" = 'Holiday'::text) as "holidayOpeningHoursFi",
    min("SV"::text) filter (where "Type" = 'Holiday'::text) as "holidayOpeningHoursSv",
    min("EN"::text) filter (where "Type" = 'Holiday'::text) as "holidayOpeningHoursEn",
    min("FI"::text) filter (where "Type" = 'Special'::text) as "specialOpeningHoursFi",
    min("SV"::text) filter (where "Type" = 'Special'::text) as "specialOpeningHoursSv",
    min("EN"::text) filter (where "Type" = 'Special'::text) as "specialOpeningHoursEn"
from geo."mv_ServiceHoursGrouped"
group by "RootId"
with data;
create unique index on geo."mv_ServiceHoursGroupedType"("RootId");

----------------------------------------------------- Channel services in JSON format ----------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ChannelServiceJson" as
select 
    chs."ChannelId" as "RootId",
    json_agg(js."JsonService")::text as "Services"
from geo."mv_ChannelService" chs
join (
    select 
        sn."ServiceId", 
        json_build_object('serviceId', sn."ServiceId", 'names', json_agg(json_build_object('languageCode', sn."LanguageCode", 'name', sn."Name" ))) as "JsonService"
    from geo."mv_ServiceName" sn
    group by "ServiceId"
) js on js."ServiceId" = chs."ServiceId" 
group by "RootId"
with data;
create unique index on geo."mv_ChannelServiceJson" ("RootId");

----------------------------------------------------- Service classes---------------------------------------------------
------------------------------------------------------------------------------------------------------------------------
create materialized view geo."mv_ServiceClass" as
select distinct
    chs."ServiceId",
    ssc."ServiceClassId",
    sc."Code",
    scd."Description_FI",
    scd."Description_SV",
    scd."Description_EN"
from geo."mv_ChannelService" chs
join "ServiceVersioned" sv on chs."ServiceId" = sv."UnificRootId" and sv."PublishingStatusId" = (select "Id" from "PublishingStatusType" where "Code" = 'Published')
join "ServiceServiceClass" ssc on sv."Id" = ssc."ServiceVersionedId" 
join "ServiceClass" sc on ssc."ServiceClassId" = sc."Id" 
join 
(
    select 	
        scd."ServiceClassId",
        min(scd."Description"::text) filter (where sl."Code" = 'fi'::text) as "Description_FI",
        min(scd."Description"::text) filter (where sl."Code" = 'sv'::text) as "Description_SV",
        min(scd."Description"::text) filter (where sl."Code" = 'en'::text) as "Description_EN"
    from "ServiceClassDescription" scd
    join geo."SupportedLanguages" sl on sl."Id" = scd."LocalizationId" 
    group by scd."ServiceClassId"
) scd on scd."ServiceClassId" = ssc."ServiceClassId" 
where sc."IsValid" = true
with data;
create unique index on geo."mv_ServiceClass" ("ServiceId", "ServiceClassId");
create index on geo."mv_ServiceClass" ("ServiceId");

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
    shgt."standardOpeningHoursFi",
    shgt."standardOpeningHoursSv",
    shgt."standardOpeningHoursEn",
    shgt."exceptionalOpeningHoursFi",
    shgt."exceptionalOpeningHoursSv",
    shgt."exceptionalOpeningHoursEn",
    shgt."holidayOpeningHoursFi",
    shgt."holidayOpeningHoursSv",
    shgt."holidayOpeningHoursEn",
    shgt."specialOpeningHoursFi",
    shgt."specialOpeningHoursSv",
    shgt."specialOpeningHoursEn",
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
left join geo."mv_ServiceHoursGroupedType" shgt on chal."RootId" = shgt."RootId"
left join geo."mv_ChannelServiceJson" as js on chal."RootId" = js."RootId"
with data;
create index on geo."mv_layer_serviceChannel"("rootId");
create index on geo."mv_layer_serviceChannel"("versionedId");
create index on geo."mv_layer_serviceChannel"("addressId");
create index on geo."mv_layer_serviceChannel" using gist ("location");


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

create materialized view geo."mv_dataStore_standardServiceHours" as
select 
	"RootId",
	"ServiceHoursId",
	"From",
	"To",
	"OpeningType"
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
	shb."OpeningType",
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
	"OpeningType",
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
	shb."OpeningType",
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

create materialized view geo."mv_dataStore_specialServiceHours" as
select 
	shb."RootId",
	shb."ServiceHoursId",
	sh."From",
	sh."To"
from  geo."mv_ServiceHoursBase" shb
join geo."mv_ServiceHoursSpecial" sh on shb."ServiceHoursId" = sh."ServiceHoursId" and "LanguageCode" = 'en' 
with data;
create unique index on geo."mv_dataStore_specialServiceHours"("ServiceHoursId");
create index on geo."mv_dataStore_specialServiceHours"("RootId");

create materialized view geo."mv_dataStore_specialServiceHoursLan" as
select 
	shb."RootId",
	shb."ServiceHoursId",
	sh."From",
	sh."To",
	ai."FI" as "AdditionalInformation_FI",
	ai."SV" as "AdditionalInformation_SV",
	ai."EN" as "AdditionalInformation_EN"
from  geo."mv_ServiceHoursBase" shb
join geo."mv_ServiceHoursSpecial" sh on shb."ServiceHoursId" = sh."ServiceHoursId" and "LanguageCode" = 'en'
left join geo."mv_ServiceHoursAdditionalInformationLan" ai on shb."ServiceHoursId" = ai."ServiceHoursId" 
with data;
create unique index on geo."mv_dataStore_specialServiceHoursLan"("ServiceHoursId");
create index on geo."mv_dataStore_specialServiceHoursLan"("RootId");

create materialized view geo."mv_dataStore_holidayServiceHours" as
select 
	shb."RootId",
	shb."ServiceHoursId",
	shb."HolidayId",
	shb."HolidayDate",
	shb."OpeningType",
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
	shb."OpeningType",
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

create materialized view geo."mv_layer_serviceClass" as 
select  
    lc."RootId" as "channelId",
    chs."ServiceId" as "serviceId",
    chn."FI" as "channelNameFi",
    chn."SV" as "channelNameSv",
    chn."EN" as "channelNameEn",
    sen."FI" as "serviceNameFi",
    sen."SV" as "serviceNameSv",
    sen."EN" as "serviceNameEn",
    sc."Code" as "serviceClassCode",
    sc."Description_FI" as "serviceClassDescriptionFi",
    sc."Description_SV" as "serviceClassDescriptionSv",
    sc."Description_EN" as "serviceClassDescriptionEn",
    chcg."Location3067" as "location"
from geo."LocationChannel" lc
join geo."mv_ChannelNameLan" chn on lc."VersionedId" = chn."VersionedId" 
join geo."mv_ChannelCoordinateGroup" chcg on lc."VersionedId" = chcg."VersionedId" 
join geo."mv_ChannelService" chs on lc."RootId" = chs."ChannelId" 
join geo."mv_ServiceNameLan" sen on chs."ServiceId" = sen."ServiceId" 
join geo."mv_ServiceClass" sc on chs."ServiceId" = sc."ServiceId" 
with data;
create unique index on geo."mv_layer_serviceClass" ("channelId", "serviceId", "serviceClassCode");

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
    refresh materialized view concurrently geo."mv_ServiceHoursGroupedType";
    refresh materialized view concurrently geo."mv_ServiceClass";

    refresh materialized view concurrently geo."ChannelIds";
    refresh materialized view geo."mv_layer_serviceChannelBase";
    refresh materialized view geo."mv_layer_serviceChannel";
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
