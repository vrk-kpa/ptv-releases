CREATE OR REPLACE VIEW "coordinate_geo" AS
    SELECT f."AddressId",
           ST_Multi(ST_Collect(f.the_geom)) AS singlegeom
	FROM ( SELECT c."AddressId",
            (ST_Dump(ST_MakePoint(c."Longtitude", c."Latitude"))).geom AS the_geom
           FROM "Coordinate" c
		   WHERE "CoordinateState" = 'Ok') f
    GROUP BY f."AddressId"
;


CREATE OR REPLACE VIEW "address_ptv" AS
 SELECT 
	a."Id"::text AS "AddressId",
    a."StreetNumber" AS "StreetNumber",
    m."Code" AS "MunicipalityCode",
    c."Code" AS "CountryCode",
    p."Code" AS "PostalCode",
    sn."Text" AS "StreetName",
    l."Code" AS "LanguageCode",
        CASE l."Code"
            WHEN 'fi'::text THEN sn."Text"
            ELSE NULL::character varying
        END AS "StreetNameFI",
        CASE l."Code"
            WHEN 'sv'::text THEN sn."Text"
            ELSE NULL::character varying
        END AS "StreetNameSV",
    cg.singlegeom AS "Geometry"
   FROM "Address" a
     LEFT JOIN "Municipality" m ON a."MunicipalityId" = m."Id"
     LEFT JOIN "Country" c ON a."CountryId" = c."Id"
     LEFT JOIN "PostalCode" p ON a."PostalCodeId" = p."Id"
     LEFT JOIN "StreetName" sn ON a."Id" = sn."AddressId"
     LEFT JOIN "Language" l ON sn."LocalizationId" = l."Id"
     LEFT JOIN coordinate_geo cg ON a."Id" = cg."AddressId"
;