CREATE EXTENSION IF NOT EXISTS postgis SCHEMA "public";

CREATE OR REPLACE VIEW public."geoChannelArea" AS
 SELECT sca."ServiceChannelVersionedId",
    a."Code",
    a."Id" AS "AreaId",
    at."Code" AS "AreaTypeCode"
   FROM "ServiceChannelArea" sca
     JOIN "ServiceLocationChannel" slc ON slc."ServiceChannelVersionedId" = sca."ServiceChannelVersionedId"
     JOIN "Area" a ON a."Id" = sca."AreaId"
     JOIN "AreaType" at ON at."Id" = a."AreaTypeId";

CREATE OR REPLACE VIEW public."geoChannelMunicipality" AS
 SELECT scam."ServiceChannelVersionedId",
    m."Code",
    m."Id" AS "MunicipalityId"
   FROM "ServiceChannelAreaMunicipality" scam
     JOIN "ServiceLocationChannel" slc ON slc."ServiceChannelVersionedId" = scam."ServiceChannelVersionedId"
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
    slc."Id" AS "ServiceLocationChannelId",
    ait."Code" AS "AreaInformationType"
   FROM "ServiceLocationChannel" slc
     JOIN "ServiceChannelVersioned" scv ON slc."ServiceChannelVersionedId" = scv."Id"
     LEFT JOIN "AreaInformationType" ait ON scv."AreaInformationTypeId" = ait."Id"
  WHERE scv."PublishingStatusId" = (( SELECT "PublishingStatusType"."Id"
           FROM "PublishingStatusType"
           WHERE "PublishingStatusType"."Code" = 'Published'::text))
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

CREATE OR REPLACE VIEW public."geoChannelMunicipality" AS
 SELECT scam."ServiceChannelVersionedId",
    m."Code",
    m."Id" AS "MunicipalityId"
   FROM "ServiceChannelAreaMunicipality" scam
     JOIN "ServiceLocationChannel" slc ON slc."ServiceChannelVersionedId" = scam."ServiceChannelVersionedId"
     JOIN "Municipality" m ON m."Id" = scam."MunicipalityId";

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
 SELECT slca."ServiceLocationChannelId",
    ads."Id" AS "AddressId",
    ac."Code" AS "CharacterCode",
    cnt."Code" AS "CountryCode",
    aty."Code" AS "AddressType",
    mu."Code" AS "MunicipalityCode",
    poc."Code" AS "PostalCode",
    ast."StreetNumber",
    ast."AddressId" AS "AddressStreetId"
   FROM "ServiceLocationChannelAddress" slca
     JOIN "Address" ads ON slca."AddressId" = ads."Id"
     JOIN "AddressCharacter" ac ON slca."CharacterId" = ac."Id"
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
    st_setsrid(st_point(c."Longitude", c."Latitude"), 3067)::geometry(Point,3067) AS singlegeom
FROM "Coordinate" c
JOIN "CoordinateType" ct ON c."TypeId" = ct."Id"
WHERE lower(c."CoordinateState") IN ('ok'::text, 'enteredbyuser'::text);

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