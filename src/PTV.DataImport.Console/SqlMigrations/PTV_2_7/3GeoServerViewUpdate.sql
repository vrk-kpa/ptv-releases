create schema if not exists geo;

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- DROPS ------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------
drop materialized view if exists geo."ChannelIds";
drop materialized view if exists geo."lc_ChannelMunicipalityNameLan";
drop materialized view if exists geo."lc_AddressMunicipalityGroup";
drop materialized view if exists geo."lc_LocationChannelLan";
drop materialized view if exists geo."lc_MunicipalityNameLan";
drop materialized view if exists geo."lc_ChannelServiceNameLan";
drop materialized view if exists geo."lc_ServiceNameLan";
drop materialized view if exists geo."lc_ChannelNameLan";
drop materialized view if exists geo."lc_OrganizationNameLan";


drop materialized view if exists geo."lc_ServiceName";
drop materialized view if exists geo."lc_LocationChannel";
drop materialized view if exists geo."lc_ChannelService";
drop materialized view if exists geo."lc_ChannelCurrentlyOpen";
--drop materialized view if exists geo."lc_StandardOpeningHoursGroup";
drop materialized view if exists geo."lc_StandardOpeningHours";
drop materialized view if exists geo."lc_ChannelCoordinateGroup";
drop materialized view if exists geo."lc_ChannelAddressCoordinateGroup";
drop materialized view if exists geo."lc_ChannelAddressCoordinate";
drop materialized view if exists geo."lc_Address";
drop materialized view if exists geo."lc_ChannelAddress";
drop materialized view if exists geo."lc_OrganizationName";
drop materialized view if exists geo."lc_ChannelName";
drop materialized view if exists geo."lc_MunicipalityName";
drop materialized view if exists geo."LocationChannel";

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel- ---------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."LocationChannel" as 
select 
	scv."Id" as "VersionedId",
	scv."UnificRootId" as "RootId",
	scv."OrganizationId" as "OrganizationId"
from "ServiceChannelVersioned" scv
where scv."PublishingStatusId" = (select "Id" from "PublishingStatusType" where "Code" = 'Published')
and scv."TypeId" = (select "Id" from "ServiceChannelType" where "Code" = 'ServiceLocation')
with data;
create unique index on geo."LocationChannel" ("VersionedId");
create unique index on geo."LocationChannel" ("RootId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Municipality names -----------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_MunicipalityName" as 
select 
	mun."MunicipalityId",
	lan."Code" as "LanguageCode",
	mun."Name"
from "MunicipalityName" mun
join "Language" lan on mun."LocalizationId" = lan."Id"
with data;
create index on geo."lc_MunicipalityName"("MunicipalityId");
create unique index on geo."lc_MunicipalityName"("MunicipalityId", "LanguageCode");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Name -----------------------------------------------------------
-- > geo."LocationChannel"
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_ChannelName" as 
select 
	"VersionedId", 
	"LanguageCode", 
	"Name"
from (
	select 
		"Name"."Id" as "VersionedId",
		"Name"."LanguageCode",
		case 
			when "DisplayType"."Type" = 'AlternateName' then "AlternateName"."Name"
			else "Name"."Name"
		end "Name"
	from
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
	left join 
	(	
		select 
			"ServiceChannelVersionedId" as "Id",
			"Name",
			"LocalizationId"
		from "ServiceChannelName" 
		where "TypeId" = (select "Id" from "NameType" where "Code" = 'AlternateName')
	) as "AlternateName" on "Name"."Id" = "AlternateName"."Id" and "Name"."LocalizationId" = "AlternateName"."LocalizationId"
	left join 
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
create index on geo."lc_ChannelName" ("VersionedId");
create unique index on geo."lc_ChannelName"("VersionedId", "LanguageCode");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Organization Name -----------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_OrganizationName" as 
select 
	"OrganizationId",
	"Name",
	"LanguageCode"
from (	
	select 
		org."RootId" as "OrganizationId",
		ona."Name",
		lan."Code" as "LanguageCode"
	from (
		select "UnificRootId" as "RootId", "Id" 
		from "OrganizationVersioned" 
		where "PublishingStatusId" = (select "Id" from "PublishingStatusType" where "Code" = 'Published')
	) org	
	join "OrganizationName" ona on org."Id" = ona."OrganizationVersionedId"
	join "OrganizationDisplayNameType" odt on org."Id" = odt."OrganizationVersionedId" and ona."LocalizationId" = odt."LocalizationId" and ona."TypeId" = odt."DisplayNameTypeId"
	join "Language" lan on ona."LocalizationId" = lan."Id"
) t
where exists (select 1 from geo."LocationChannel" glc where glc."OrganizationId" = t."OrganizationId")
with data;
create index on geo."lc_OrganizationName" ("OrganizationId");
create unique index on geo."lc_OrganizationName"("OrganizationId", "LanguageCode");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Address --------------------------------------------------------
-- -> geo."LocationChannel"
-- <- geo."lc_ChannelAddressCoordinate", geo."lc_Address"
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_ChannelAddress" as
select 
	"ServiceChannelVersionedId" as "VersionedId",
	"AddressId"
from "ServiceChannelAddress" sca
where 
	sca."CharacterId" = (select "Id" from "AddressCharacter" where "Code" = 'Visiting')
	and exists (select 1 from geo."LocationChannel" glc where glc."VersionedId" = sca."ServiceChannelVersionedId")
with data;
create index on geo."lc_ChannelAddress" ("VersionedId");
create index on geo."lc_ChannelAddress" ("AddressId");
create unique index on geo."lc_ChannelAddress" ("VersionedId", "AddressId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel loacation addresses --------------------------------------------
-- > geo."lc_ChannelAddress" 
-- > 
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_Address" as
select 
	ap."AddressId", 
	adr."OrderNumber",
	lan."Code" as "LanguageCode", 
	asn."Name" || ' ' || ap."StreetNumber" || ', ' ||  pc."Code" || ' ' || pcn."Name" as "StreetAddress",
	mu."Id" as "MunicipalityId",
	mu."Code" as "MunicipalityCode",
	mun."Name" as "MunicipalityName"
from "ClsAddressPoint" ap
join "ClsAddressStreetName" asn on ap."AddressStreetId" = asn."ClsAddressStreetId"
join "Language" lan on lan."Id" = asn."LocalizationId"
join "PostalCode" pc on pc."Id" = ap."PostalCodeId"
join "PostalCodeName" pcn on pcn."PostalCodeId" = ap."PostalCodeId" and pcn."LocalizationId" = asn."LocalizationId" 
join "Municipality" mu on mu."Id" = ap."MunicipalityId"
join "MunicipalityName" mun on mun."MunicipalityId" = ap."MunicipalityId" and mun."LocalizationId" = asn."LocalizationId"
join "Address" adr on adr."Id" = ap."AddressId"
where exists (select 1 from geo."lc_ChannelAddress" lc_cha where lc_cha."AddressId" = ap."AddressId")
with data;
create unique index on geo."lc_Address" ("AddressId", "LanguageCode");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Address Coordinate -----------------------------------------------------
-- -> geo."lc_ChannelAddress"
-- <- geo."lc_ChannelAddressCoordinateGroup"
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_ChannelAddressCoordinate" as
select
	aco."RelatedToId" as "AddressId",
	st_setsrid(st_point(aco."Longitude", aco."Latitude"), 3067)::geometry(Point,3067) AS "Location3067",
	cty."Code"
from "AddressCoordinate" aco
join "CoordinateType" cty on aco."TypeId" = cty."Id" 
where "CoordinateState" in ('Ok', 'EnteredByUser', 'EnteredByAR')
	and exists (select 1 from geo."lc_ChannelAddress" gca where gca."AddressId" = aco."RelatedToId")
with data;
create index on geo."lc_ChannelAddressCoordinate" ("AddressId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Address Coordinate Group -----------------------------------------------
-- > geo."lc_ChannelAddressCoordinate"
-- < 
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_ChannelAddressCoordinateGroup" as
select 
	"AddressId",
	ST_Collect("Location3067") as "Location3067",
	array_to_string(array_agg("Type"), ', ') as "Coordinates"
from (	
	select distinct
		"AddressId",
		"Location3067",
		"Code" || ': ' || ST_AsText("Location3067") as "Type"
	from geo."lc_ChannelAddressCoordinate" coo
) t
group by "AddressId"
with data;
create unique index on geo."lc_ChannelAddressCoordinateGroup" ("AddressId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Coordinate Group -----------------------------------------------
-- > geo."lc_ChannelAddress"
-- > geo."lc_ChannelAddressCoordinateGroup", geo."lc_LocationChannelLan"
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_ChannelCoordinateGroup" as
select 
	cha."VersionedId",
	ST_Multi(ST_Union(cag."Location3067")) as "Location3067"
from geo."lc_ChannelAddress" cha
join geo."lc_ChannelAddressCoordinateGroup" cag on cha."AddressId" = cag."AddressId"
group by cha."VersionedId"
with data;
create unique index on geo."lc_ChannelCoordinateGroup" ("VersionedId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- StandardOpeningHours ---------------------------------------------------
-- > geo."LocationChannel"
/*-- < geo."lc_StandardOpeningHoursGroup"*/
-- < geo."lc_ChannelCurrentlyOpen"
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_StandardOpeningHours" as
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
from (
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
	and exists (select 1 from geo."LocationChannel" glc where glc."VersionedId" = sch."ServiceChannelVersionedId")
) t
left join "DailyOpeningTime" dot on t."ServiceHoursId" = dot."OpeningHourId"
where dot."DayFrom" = date_part('isodow', current_date)-1  or t."IsNonStop" = true
with data;

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Standard Opening Hours Group -------------------------------------------
-- > geo."lc_StandardOpeningHours"
------------------------------------------------------------------------------------------------------------------------------
/*create materialized view geo."lc_StandardOpeningHoursGroup" as
select 
	"VersionedId",
	"ServiceHoursId",
	coalesce (min("ServiceHoursOrder"), 0) as "OrderNumber",
	case 
		when bool_or("IsNonStop") then '24/7'
		else string_agg(to_char("From", 'HH24:MI') || '-' || to_char("To", 'HH24:MI'), '; ' order by "From") 
	end as "OpeningTime"
from geo."StandardOpeningHours" 
group by "VersionedId", "ServiceHoursId"
with data;
*/

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Standard Opening Hours Group -------------------------------------------
-- > geo."lc_StandardOpeningHours"
-- < geo."lc_LocationChannelLan" 
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_ChannelCurrentlyOpen" as 
select 
	"VersionedId", 
	bool_or("CurrentlyOpen")::text as "CurrentlyOpen"
from (
	select 
		"VersionedId",
		"From"::time <= current_timestamp::time and "To"::time > current_timestamp::time as "CurrentlyOpen"
	from geo."lc_StandardOpeningHours" 
) t group by "VersionedId" 
with data;
create unique index on geo."lc_ChannelCurrentlyOpen" ("VersionedId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Service Channels -------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------
-- > geo."lc_LocationChannel"
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_ChannelService" as
select 
	ssc."ServiceChannelId"  as "ChannelId",
	ssc."ServiceId" as "ServiceId"
from "ServiceServiceChannel" ssc
where exists (select 1 from geo."LocationChannel" glc where glc."RootId" = ssc."ServiceChannelId")
with data;
create unique index on geo."lc_ChannelService" ("ChannelId", "ServiceId");
create index on geo."lc_ChannelService" ("ChannelId");
create index on geo."lc_ChannelService" ("ServiceId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Service Names ----------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------
-- > geo."lc_LocationChannel"
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_ServiceName" as
select
	sev."UnificRootId" as "ServiceId",
	lan."Code" as "LanguageCode",
	sen."Name"		
from "ServiceVersioned" sev
join (
	select 
		"ServiceVersionedId",
		"Name",
		"LocalizationId"
	from "ServiceName"
	where "TypeId" = (select "Id" from "NameType" where "Code" = 'Name')
	) sen on sev."Id" = sen."ServiceVersionedId"
join "Language" lan on lan."Id" = sen."LocalizationId"	
where sev."PublishingStatusId" = (select "Id" from "PublishingStatusType" where "Code" = 'Published')
	and exists (select 1 from geo."lc_ChannelService" where "ServiceId" = sev."UnificRootId")
with data;
create index on geo."lc_ServiceName"("ServiceId");
create unique index on geo."lc_ServiceName" ("ServiceId", "LanguageCode");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel (base for view) ------------------------------------------------
-- > geo."LocationChannel", geo."lc_ChannelCoordinateGroup", geo."lc_ChannelCurrentlyOpen"
-- > geo."ChannelIds"
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_LocationChannel" as
select 
	lc."VersionedId",
	lc."RootId",
	lc."OrganizationId",
	coalesce(cco."CurrentlyOpen", '-') as "CurrentlyOpen",
	ccg."Location3067"
from geo."LocationChannel" lc
join geo."lc_ChannelCoordinateGroup" ccg on lc."VersionedId" = ccg."VersionedId"
left join geo."lc_ChannelCurrentlyOpen" cco on lc."VersionedId" = cco."VersionedId"
with data;
create unique index on geo."lc_LocationChannel" ("VersionedId");
create unique index on geo."lc_LocationChannel" ("RootId");
create index on geo."lc_LocationChannel" ("OrganizationId");
create index on geo."lc_LocationChannel" using gist ("Location3067");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel names with languages -------------------------------------------
-- > geo."lc_ChannelName"
-- > geo."lc_LocationChannelLan"
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_ChannelNameLan" as
select 
	"VersionedId",
	min("Name") filter (where "LanguageCode"='fi') as "FI",
	min("Name") filter (where "LanguageCode"='sv') as "SV",
	min("Name") filter (where "LanguageCode"='en') as "EN"
from geo."lc_ChannelName"
group by "VersionedId"
with data;	
create index on geo."lc_ChannelNameLan"("VersionedId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Organization names with languages --------------------------------------
-- > geo."lc_OrganizationName"
-- > geo."lc_LocationChannelLan"
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_OrganizationNameLan" as
select 
	"OrganizationId",
	min("Name") filter (where "LanguageCode"='fi') as "FI",
	min("Name") filter (where "LanguageCode"='sv') as "SV",
	min("Name") filter (where "LanguageCode"='en') as "EN"
from geo."lc_OrganizationName"
group by "OrganizationId"
with data;	
create index on geo."lc_OrganizationName"("OrganizationId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Service names with languages -------------------------------------------
-- > geo."lc_ServiceName"
-- > geo."lc_ChannelServiceNameLan"
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_ServiceNameLan" as
select 
	"ServiceId",
	min("Name") filter (where "LanguageCode"='fi') as "FI",
	min("Name") filter (where "LanguageCode"='sv') as "SV",
	min("Name") filter (where "LanguageCode"='en') as "EN"
from geo."lc_ServiceName"
group by "ServiceId"
with data;
create unique index on geo."lc_ServiceNameLan" ("ServiceId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Municipality names with languages --------------------------------------
-- > geo."lc_MunicipalityName"
-- > 
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_MunicipalityNameLan" as 
select 
	"MunicipalityId",
	min("Name") filter (where "LanguageCode"='fi') as "FI",
	min("Name") filter (where "LanguageCode"='sv') as "SV",
	min("Name") filter (where "LanguageCode"='en') as "EN"
from geo."lc_MunicipalityName"
group by "MunicipalityId"
with data;
create unique index on geo."lc_MunicipalityNameLan" ("MunicipalityId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel with channel and organization names with languages -------------
-- > geo."LocationChannel", geo."lc_ChannelCoordinateGroup", geo."lc_ChannelNameLan", geo."lc_OrganizationNameLan"
-- > 
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_LocationChannelLan" as
select 
	lc."VersionedId",
	lc."RootId",
	lc."OrganizationId",
	coalesce(cco."CurrentlyOpen", '-') as "CurrentlyOpen",
	chn."FI" as "Name_FI",
	chn."SV" as "Name_SV",
	chn."EN" as "Name_EN",
	orn."FI" as "OrganizationName_FI",
	orn."SV" as "OrganizationName_SV",
	orn."EN" as "OrganizationName_EN",
	ccg."Location3067"
from geo."LocationChannel" lc
join geo."lc_ChannelCoordinateGroup" ccg on lc."VersionedId" = ccg."VersionedId"
join geo."lc_ChannelNameLan" chn on lc."VersionedId" = chn."VersionedId"
join geo."lc_OrganizationNameLan" orn on lc."OrganizationId" = orn."OrganizationId" 
left join geo."lc_ChannelCurrentlyOpen" cco on lc."VersionedId" = cco."VersionedId"
with data;
create unique index on geo."lc_LocationChannelLan" ("VersionedId");
create unique index on geo."lc_LocationChannelLan" ("RootId");
create index on geo."lc_LocationChannelLan" ("OrganizationId");
create index on geo."lc_LocationChannelLan" using gist ("Location3067");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel service names with languages -----------------------------------
-- > geo."lc_ChannelService", geo."lc_ServiceNameLan"
-- > 
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_ChannelServiceNameLan" as
select 
	chs."ChannelId",
	chs."ServiceId",
	sen."FI",
	sen."SV",
	sen."EN"
from geo."lc_ChannelService" chs
join geo."lc_ServiceNameLan" sen on chs."ServiceId" = sen."ServiceId"
with data;
create unique index on geo."lc_ChannelServiceNameLan" ("ChannelId", "ServiceId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Address Municipality Group ---------------------------------------------
--> geo."Address, geo."LocationChhannel", geo."lc_ChannelAddress"
--> geo."lc_ChannelMunicipalityNameLan"
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_AddressMunicipalityGroup" as
select distinct
	cha."RootId",
	agr."MunicipalityCode", 
	agr."MunicipalityId"
from (
	select 
		lc."RootId",
		cha."AddressId" 
	from geo."LocationChannel" lc
	join geo."lc_ChannelAddress" cha on lc."VersionedId" = cha."VersionedId"
) cha
join (
	select 
		"AddressId", 
		"MunicipalityCode", 
		"MunicipalityId"
	from geo."lc_Address"
	group by "AddressId", "MunicipalityCode", "MunicipalityId"
) agr on cha."AddressId" = agr."AddressId"
with data;
create unique index on geo."lc_AddressMunicipalityGroup" ("RootId", "MunicipalityId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel municipality names with languages ------------------------------
-- > geo."AddressMunicipalityGroup", geo."lc_ServiceNameLan"
-- > 
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."lc_ChannelMunicipalityNameLan" as
select 
	amg."RootId",
	amg."MunicipalityId",
	amg."MunicipalityCode",
	mun."FI",
	mun."SV",
	mun."EN"
from geo."lc_AddressMunicipalityGroup" amg
join geo."lc_MunicipalityNameLan" mun on amg."MunicipalityId" = mun."MunicipalityId"
with data;
create index on geo."lc_ChannelMunicipalityNameLan" ("RootId");
create unique index on geo."lc_ChannelMunicipalityNameLan" ("RootId", "MunicipalityId");

------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Channel Ids ------------------------------------------------------------
-- > geo."AddressMunicipalityGroup", geo."lc_ServiceNameLan"
-- > 
------------------------------------------------------------------------------------------------------------------------------
create materialized view geo."ChannelIds" as
select 
	lch."RootId",
	lch."VersionedId",
	lch."OrganizationId",
	lch."CurrentlyOpen",
	services."ServiceIds",
	addresses."AddressIds",
	lch."Location3067"
from geo."lc_LocationChannel" as lch 
left join 
(
	select 
		"ChannelId" as "VersionedId",  
		string_agg("ServiceId"::text, ', '::text) as "ServiceIds"
	from geo."lc_ChannelService"
	group by "ChannelId"
) as services on lch."VersionedId" = services."VersionedId"
left join 
(
	select 
		"VersionedId",  
		string_agg("AddressId"::text, ', '::text) as "AddressIds"
	from geo."lc_ChannelAddress"
	group by "VersionedId"
) addresses on lch."VersionedId" = addresses."VersionedId"
with data;
create index on geo."ChannelIds" ("VersionedId");
create index on geo."ChannelIds" ("RootId");


------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- Refresh views function -------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------
create or replace function geo."RefreshMaterializedViews"()
returns void
as $$ 
begin
	refresh materialized view concurrently geo."LocationChannel";
	refresh materialized view concurrently geo."lc_MunicipalityName";
	refresh materialized view concurrently geo."lc_ChannelName";
	refresh materialized view concurrently geo."lc_OrganizationName";
	refresh materialized view concurrently geo."lc_ChannelAddress";
	refresh materialized view concurrently geo."lc_Address";
	refresh materialized view geo."lc_ChannelAddressCoordinate";	
	refresh materialized view concurrently geo."lc_ChannelAddressCoordinateGroup";
	refresh materialized view concurrently geo."lc_ChannelCoordinateGroup";
	refresh materialized view geo."lc_StandardOpeningHours";
	--refresh materialized view geo."lc_StandardOpeningHoursGroup";
	refresh materialized view concurrently geo."lc_ChannelCurrentlyOpen";
	refresh materialized view concurrently geo."lc_ChannelService";
	refresh materialized view concurrently geo."lc_ServiceName";
	refresh materialized view concurrently geo."lc_LocationChannel";

	refresh materialized view concurrently geo."lc_ChannelNameLan";
	refresh materialized view concurrently geo."lc_OrganizationNameLan";
	refresh materialized view concurrently geo."lc_ServiceNameLan";
	refresh materialized view concurrently geo."lc_MunicipalityNameLan";
	refresh materialized view concurrently geo."lc_LocationChannelLan";
	refresh materialized view concurrently geo."lc_ChannelServiceNameLan";
	refresh materialized view concurrently geo."lc_AddressMunicipalityGroup";
	refresh materialized view concurrently geo."lc_ChannelMunicipalityNameLan";
	
	refresh materialized view concurrently geo."ChannelIds";
end;$$ language plpgsql;

