CREATE EXTENSION IF NOT EXISTS postgis SCHEMA "public";

CREATE OR REPLACE VIEW public."geoChannelArea" AS
SELECT sca."ServiceChannelVersionedId",
  a."Code",
  a."Id" AS "AreaId",
  at."Code" AS "AreaTypeCode"
FROM "ServiceChannelArea" sca
--     JOIN "ServiceLocationChannel" slc ON slc."ServiceChannelVersionedId" = sca."ServiceChannelVersionedId"
JOIN "Area" a ON a."Id" = sca."AreaId"
JOIN "AreaType" at ON at."Id" = a."AreaTypeId";

CREATE OR REPLACE VIEW public."geoChannelMunicipality" AS
SELECT scam."ServiceChannelVersionedId",
  m."Code",
  m."Id" AS "MunicipalityId"
FROM "ServiceChannelAreaMunicipality" scam
--     JOIN "ServiceLocationChannel" slc ON slc."ServiceChannelVersionedId" = scam."ServiceChannelVersionedId"
JOIN "Municipality" m ON m."Id" = scam."MunicipalityId";

CREATE OR REPLACE VIEW public."geoAreaName" AS
 SELECT an."Name",
    l."Code" AS "Language",
    an."AreaId"
   FROM "AreaName" an
     JOIN "Language" l ON an."LocalizationId" = l."Id";

CREATE OR REPLACE VIEW public."geoServiceChannelEmail" AS
 SELECT sce."ServiceChannelVersionedId" AS "ServiceChannelVersionedId",
    ema."Value",
    lan."Code" AS "Language"
   FROM "ServiceChannelEmail" sce
     JOIN "Email" ema ON sce."EmailId" = ema."Id"
     JOIN "Language" lan ON ema."LocalizationId" = lan."Id";

CREATE OR REPLACE VIEW public."geoForeignAddress" AS
 SELECT af."AddressId" AS "AddressId",
    lan."Code" AS "Language",
    aftn."Name" AS "Address"
   FROM "AddressForeign" af
     JOIN "AddressForeignTextName" aftn ON af."AddressId" = aftn."AddressForeignId"
     JOIN "Language" lan ON aftn."LocalizationId" = lan."Id";

CREATE OR REPLACE VIEW public."geoLocationChannel" AS
SELECT scv."Id" AS "ServiceChannelVersionedId",
    scv."UnificRootId",
    scv."OrganizationId",
    ait."Code" AS "AreaInformationType"
FROM "ServiceChannelVersioned" scv 
LEFT JOIN "AreaInformationType" ait ON scv."AreaInformationTypeId" = ait."Id"
WHERE 
	scv."PublishingStatusId" = ( SELECT "PublishingStatusType"."Id" FROM "PublishingStatusType" WHERE "PublishingStatusType"."Code" = 'Published'::text)
	AND scv."TypeId" = ( SELECT "ServiceChannelType"."Id" FROM "ServiceChannelType" WHERE "ServiceChannelType"."Code" = 'ServiceLocation'::text)
ORDER BY scv."Modified" DESC;

CREATE OR REPLACE VIEW public."geoChannelAreaPublished" AS
 SELECT gca."ServiceChannelVersionedId",
    gca."Code",
    gca."AreaId",
    gca."AreaTypeCode"
   FROM "geoChannelArea" gca
     JOIN "ServiceChannelVersioned" scv ON gca."ServiceChannelVersionedId" = scv."Id"
  WHERE scv."PublishingStatusId" = (( SELECT "PublishingStatusType"."Id"
           FROM "PublishingStatusType"
          WHERE "PublishingStatusType"."Code" = 'Published'::text));

CREATE OR REPLACE VIEW public."geoChannelMunicipalityPublished" AS
 SELECT gcm."ServiceChannelVersionedId",
    gcm."Code",
    gcm."MunicipalityId"
   FROM "geoChannelMunicipality" gcm
     JOIN "ServiceChannelVersioned" scv ON gcm."ServiceChannelVersionedId" = scv."Id"
  WHERE scv."PublishingStatusId" = (SELECT "PublishingStatusType"."Id"
           FROM "PublishingStatusType"
          WHERE "PublishingStatusType"."Code" = 'Published'::text);

CREATE OR REPLACE VIEW public."geoMunicipalityName" AS
 SELECT mn."Name",
    l."Code" AS "Language",
    mn."MunicipalityId"
   FROM "MunicipalityName" mn
     JOIN "Language" l ON mn."LocalizationId" = l."Id";

CREATE OR REPLACE VIEW public."geoOrganizationName" AS
 SELECT org."Id" AS "OrganizationId",
    ona."Name",
    lan."Code" AS "Language",
    nt."Code" AS "NameType"
   FROM "Organization" org
     JOIN "OrganizationVersioned" ov ON org."Id" = ov."UnificRootId"
     JOIN "OrganizationName" ona ON ov."Id" = ona."OrganizationVersionedId"
     JOIN "Language" lan ON ona."LocalizationId" = lan."Id"
     JOIN "NameType" nt ON ona."TypeId" = nt."Id"
  WHERE ov."PublishingStatusId" = (SELECT "PublishingStatusType"."Id"
           FROM "PublishingStatusType"
          WHERE "PublishingStatusType"."Code" = 'Published'::text);

CREATE OR REPLACE VIEW public."geoServiceChannelName" AS
 SELECT scn."ServiceChannelVersionedId" AS "ServiceChannelVersionedId",
    scn."Name",
    lan."Code" AS "Language",
    nt."Code" AS "NameType"
   FROM "ServiceChannelName" scn
     JOIN "Language" lan ON scn."LocalizationId" = lan."Id"
     JOIN "NameType" nt ON scn."TypeId" = nt."Id";

CREATE OR REPLACE VIEW public."geoServiceLocationChannelAddress" AS
 SELECT sca."ServiceChannelVersionedId",
    ads."Id" AS "AddressId",
    ac."Code" AS "CharacterCode",
    cnt."Code" AS "CountryCode",
    aty."Code" AS "AddressType",
    mu."Code" AS "MunicipalityCode",
    poc."Code" AS "PostalCode",
    ast."StreetNumber",
    ast."AddressId" AS "AddressStreetId"
   FROM "ServiceChannelAddress" sca
     JOIN "Address" ads ON sca."AddressId" = ads."Id"
     JOIN "AddressCharacter" ac ON sca."CharacterId" = ac."Id"
     LEFT JOIN "Country" cnt ON ads."CountryId" = cnt."Id"
     JOIN "AddressType" aty ON ads."TypeId" = aty."Id"
     LEFT JOIN "AddressStreet" ast ON ads."Id" = ast."AddressId"
     LEFT JOIN "Municipality" mu ON mu."Id" = ast."MunicipalityId"
     LEFT JOIN "PostalCode" poc ON poc."Id" = ast."PostalCodeId";

CREATE OR REPLACE VIEW public."geoStreetName" AS
 SELECT sn."AddressStreetId" AS "AddressStreetId",
    lan."Code" AS "Language",
    sn."Name"
   FROM "StreetName" sn
     JOIN "Language" lan ON sn."LocalizationId" = lan."Id";

CREATE OR REPLACE VIEW public."geoCoordinateWithSrid" AS
SELECT 
    c."AddressId",
    c."CoordinateState",
    ct."Code" AS "CoordinateType",
    st_setsrid(st_point(c."Longitude", c."Latitude"), 3067)::geometry(Point,3067) AS "Location"
FROM "Coordinate" c
JOIN "CoordinateType" ct ON c."TypeId" = ct."Id"
WHERE lower(c."CoordinateState") IN ('ok'::text, 'enteredbyuser'::text, 'enteredbyar'::text);

CREATE OR REPLACE VIEW public."geoServiceChannelPhone" AS
SELECT 
	scp."ServiceChannelVersionedId",
	pnt."Code" AS "PhoneType",
	dc."Code" AS "DialCode",
	p."Number",
	p."AdditionalInformation",
	sct."Code" AS "ChargeType",
	p."ChargeDescription",
    lan."Code" AS "Language"
FROM "ServiceChannelPhone" scp
JOIN "Phone" p ON scp."PhoneId" = p."Id"
JOIN "DialCode" dc ON dc."Id" = p."PrefixNumberId"  
JOIN "Language" lan ON p."LocalizationId" = lan."Id"
JOIN "PhoneNumberType" pnt on pnt."Id" = p."TypeId"
JOIN "ServiceChargeType" sct on sct."Id" = p."ChargeTypeId";

CREATE OR REPLACE VIEW public."geoServiceChannelWebPage" AS
SELECT 
	scwp."ServiceChannelVersionedId",
	wpt."Code" "WebPageType",
    wp."Name",
    wp."Url",
    wp."Description",
    lan."Code" AS "Language"
FROM "ServiceChannelWebPage" scwp
JOIN "WebPage" wp ON scwp."WebPageId" = wp."Id"
JOIN "WebPageType" wpt ON wpt."Id" = scwp."TypeId"
JOIN "Language" lan ON wp."LocalizationId" = lan."Id";

CREATE OR REPLACE VIEW public."geoAreaMunicipality" AS
SELECT 
    am."AreaId",
    am."MunicipalityId",
    m."Code"
FROM "AreaMunicipality" am
JOIN "Municipality" m ON am."MunicipalityId" = m."Id";


------------------------------------------------------------------------------------------------------------------------
create or replace view "geoLocationCoordinateWithSrid" as
select distinct 
	gcws."AddressId",
	scv."Id" as "VersionedId",
	scv."UnificRootId",
	gcws."CoordinateState",
	gcws."CoordinateType",
  gcws."Location"
from "geoCoordinateWithSrid" gcws 
join "ServiceChannelVersioned" scv on exists (select 1 from "ServiceChannelAddress" sca where sca."AddressId" = gcws."AddressId" and scv."Id" = sca."ServiceChannelVersionedId")
where scv."PublishingStatusId" = (select "Id" from "PublishingStatusType" pst where pst."Code" = 'Published')
;

create or replace view public."geoServiceChannelBase" as
select 
    scv."Id" as "VersionedId",
    scv."UnificRootId" as "RootId",
    scv."OrganizationId" as "OrganizationId", 
    sca."AddressId" as "AddressId",
    ach."Code" as "AddressCharacter",
    gcar."Code" as "AreaCode",
    gcar."AreaTypeCode" as "AreaType", 
    gcm."Code" as "MunicipalityCode",
    gcws."CoordinateType" as "CoordinateType",
    gcws."Location" as "Location"
from "ServiceChannelVersioned" scv
join "ServiceChannelAddress" sca on sca."ServiceChannelVersionedId" = scv."Id"
join "AddressCharacter" ach on ach."Id" = sca."CharacterId"
join "geoCoordinateWithSrid" gcws on sca."AddressId" = gcws."AddressId"
left join "geoChannelArea" gcar on scv."Id" = gcar."ServiceChannelVersionedId"
left join "geoChannelMunicipality" gcm on scv."Id" = gcm."ServiceChannelVersionedId"
where scv."PublishingStatusId" = (select "PublishingStatusType"."Id" from "PublishingStatusType" where "PublishingStatusType"."Code" = 'Published') 
    and scv."TypeId" = (select "ServiceChannelType"."Id" from "ServiceChannelType" where "ServiceChannelType"."Code" = 'ServiceLocation')
;

create or replace view "geoAddressStreetBase" as
select 
	ast."AddressId",
	lan."Code" as "LanguageCode",
	ast."StreetNumber",
	stn."Name" as "StreetName",
	pco."Code" as "PostalCode",
	pcn."Name" as "PostalName",
    coalesce(stn."Name", '') || ' ' || coalesce(ast."StreetNumber", '') || ', ' || coalesce(pco."Code", '') || ', ' || coalesce(pcn."Name", '') as "AddressText"
from "AddressStreet" ast
join "StreetName" stn on ast."AddressId" = stn."AddressStreetId"
join "Language" lan on stn."LocalizationId" = lan."Id"
join "PostalCode" pco on ast."PostalCodeId" = pco."Id"
left join "PostalCodeName" pcn on ast."PostalCodeId" = pcn."PostalCodeId" and lan."Id" = pcn."LocalizationId"
;

create or replace view "geoAddressStreetGrouped" as
select 
    "AddressId",
    max(case when "LanguageCode" = 'fi' then "AddressText" end) as "AddressFi",
    max(case when "LanguageCode" = 'sv' then "AddressText" end) as "AddressSv",
    max(case when "LanguageCode" = 'en' then "AddressText" end) as "AddressEn"
from "geoAddressStreetBase"
group by "AddressId", "StreetNumber", "PostalCode"
;

create or replace view "geoChannelNameBase" AS
select 
    scn."ServiceChannelVersionedId",
    lan."Code" as "LanguageCode",
    scn."Name"
from "ServiceChannelName" scn
join "Language" lan ON scn."LocalizationId" = lan."Id"
where scn."TypeId" = (select nat."Id" from "NameType" nat where nat."Code" = 'Name')
;

create or replace view "geoChannelNameGrouped" as
select 
    "ServiceChannelVersionedId",
    max(case when "LanguageCode" = 'fi' then "Name" end) as "NameFi",
    max(case when "LanguageCode" = 'en' then "Name" end) as "NameEn",
    max(case when "LanguageCode" = 'sv' then "Name" end) as "NameSv"
from  "geoChannelNameBase"
group by "ServiceChannelVersionedId"
;

create or replace view "geoOrganizationNameBase" as
select 
	ove."UnificRootId" as "OrganizationId",
    lan."Code" as "LanguageCode",
    ona."Name"
from "OrganizationVersioned" ove
join "OrganizationName" ona on ove."Id" = ona."OrganizationVersionedId"
join "Language" lan on ona."LocalizationId" = lan."Id"
where ove."PublishingStatusId" = (select "Id" from "PublishingStatusType" pst where "Code" = 'Published') 
	and ona."TypeId" = (select "Id" from "NameType" where "Code" = 'Name') 
;

create or replace view "geoOrganizationNameGrouped" as
select	
	"OrganizationId",
    max(case when "LanguageCode" = 'fi' then "Name" end) as "NameFi",
    max(case when "LanguageCode" = 'sv' then "Name" end) as "NameSv",
    max(case when "LanguageCode" = 'en' then "Name" end) as "NameEn"
from "geoOrganizationNameBase"
group by "OrganizationId"
;
