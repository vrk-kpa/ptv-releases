WITH x AS (
    SELECT DISTINCT xStr."Id"
    FROM "ClsAddressStreet" AS xStr
        JOIN "ClsAddressStreetName" AS xNam ON xNam."ClsAddressStreetId" = xStr."Id"
        JOIN "ClsAddressStreetNumber" AS xNum ON xNum."ClsAddressStreetId" = xStr."Id"
        JOIN "PostalCode" AS xPC ON xPC."Id" = xNum."PostalCodeId"
    WHERE xNam."Name3" = 'kuu'
      AND (
          (xNum."NonCls" = FALSE AND xNam."NonCls" = FALSE)
        OR (EXISTS (SELECT 1 FROM "ClsStreetNumberCoordinate" AS xCoor WHERE xCoor."RelatedToId" = xNum."Id" )))
      AND LOWER(xNam."Name") LIKE CONCAT('kuusitie', '%')
    ORDER BY xNam."Name"
    LIMIT 100)
SELECT DISTINCT "ClsAddressStreet"."Id", "ClsAddressStreet"."MunicipalityId", "ClsAddressStreet"."IsValid", "ClsAddressStreet"."NonCls",
                "ClsAddressStreetName"."ClsAddressStreetId", "ClsAddressStreetName"."LocalizationId", "ClsAddressStreetName"."Name", "ClsAddressStreetName"."Name3",
                "ClsAddressStreetName"."NonCls", "ClsAddressStreetNumber"."Id", "ClsAddressStreetNumber"."ClsAddressStreetId", "ClsAddressStreetNumber"."PostalCodeId",
                "ClsAddressStreetNumber"."IsEven", "ClsAddressStreetNumber"."StartNumber", "ClsAddressStreetNumber"."EndNumber", "ClsAddressStreetNumber"."IsValid",
                "ClsAddressStreetNumber"."NonCls", "PostalCode"."Id", "PostalCode"."Code", "PostalCode"."MunicipalityId", "PostalCode"."IsValid",
                "PostalCodeName"."PostalCodeId", "PostalCodeName"."LocalizationId", "PostalCodeName"."Name", "Municipality"."Id", "Municipality"."Code", "Municipality"."IsValid"
FROM "ClsAddressStreet"
    JOIN x ON x."Id" = "ClsAddressStreet"."Id"
    JOIN "ClsAddressStreetName" ON "ClsAddressStreet"."Id" = "ClsAddressStreetName"."ClsAddressStreetId"
    JOIN "ClsAddressStreetNumber" ON "ClsAddressStreet"."Id" = "ClsAddressStreetNumber"."ClsAddressStreetId"
    JOIN "PostalCode" ON "ClsAddressStreetNumber"."PostalCodeId" = "PostalCode"."Id"
    JOIN "PostalCodeName" ON "PostalCode"."Id" = "PostalCodeName"."PostalCodeId"
    JOIN "Municipality" ON "ClsAddressStreet"."MunicipalityId" = "Municipality"."Id"
WHERE "ClsAddressStreet"."IsValid" = TRUE
  AND "ClsAddressStreetNumber"."IsValid" = TRUE
  AND "ClsAddressStreetName"."LocalizationId" = 'a09dd793-a60a-47ff-b43b-08d3c60ad4e0' -- just this language
ORDER BY "ClsAddressStreetName"."Name", "PostalCodeName"."Name";


-- ************************************************************************************

WITH x AS (
    SELECT DISTINCT xStr."Id"
    FROM "ClsAddressStreet" AS xStr
        INNER JOIN "ClsAddressStreetName" AS xNam ON xStr."Id" = xNam."ClsAddressStreetId"
        INNER JOIN "ClsAddressStreetNumber" AS xNum ON xStr."Id" = xNum."ClsAddressStreetId"
        INNER JOIN "PostalCode" AS xPC ON xNum."PostalCodeId" = xPC."Id"
    WHERE xNam."Name3"  = 'kuu'
      AND (
          (  xNam."NonCls"  = FALSE AND xNum."NonCls"  = FALSE )
              OR
          ( EXISTS (SELECT 1 FROM "ClsStreetNumberCoordinate" AS xCoord WHERE xCoord."RelatedToId" = xNum."Id" )) )
      AND ( LOWER(xNam."Name") LIKE CONCAT('kuusitie', '%') )
      --AND xPC."Id"  = @PostalCode
    )
SELECT DISTINCT "ClsAddressStreet"."Id" , "ClsAddressStreet"."MunicipalityId" , "ClsAddressStreet"."IsValid" ,
                "ClsAddressStreet"."NonCls" , "ClsAddressStreetName"."ClsAddressStreetId" ,
                "ClsAddressStreetName"."LocalizationId" , "ClsAddressStreetName"."Name" ,
                "ClsAddressStreetName"."Name3" , "ClsAddressStreetName"."NonCls" , "ClsAddressStreetNumber"."Id" ,
                "ClsAddressStreetNumber"."ClsAddressStreetId" , "ClsAddressStreetNumber"."PostalCodeId" ,
                "ClsAddressStreetNumber"."IsEven" , "ClsAddressStreetNumber"."StartNumber" ,
                "ClsAddressStreetNumber"."EndNumber" , "ClsAddressStreetNumber"."IsValid" ,
                "ClsAddressStreetNumber"."NonCls" , "PostalCode"."Id" , "PostalCode"."Code" ,
                "PostalCode"."MunicipalityId" , "PostalCode"."IsValid" , "PostalCodeName"."PostalCodeId" ,
                "PostalCodeName"."LocalizationId" , "PostalCodeName"."Name" , "Municipality"."Id" ,
                "Municipality"."Code" , "Municipality"."IsValid"
FROM "Municipality"
    INNER JOIN "ClsAddressStreet" ON "Municipality"."Id" = "ClsAddressStreet"."MunicipalityId"
    JOIN x ON x."Id" = "ClsAddressStreet"."Id"
    INNER JOIN "ClsAddressStreetName" ON "ClsAddressStreet"."Id" = "ClsAddressStreetName"."ClsAddressStreetId"
    INNER JOIN "ClsAddressStreetNumber" ON "ClsAddressStreet"."Id" = "ClsAddressStreetNumber"."ClsAddressStreetId"
    INNER JOIN "PostalCode" ON "ClsAddressStreetNumber"."PostalCodeId" = "PostalCode"."Id"
    INNER JOIN "PostalCodeName" ON "PostalCode"."Id" = "PostalCodeName"."PostalCodeId"
WHERE "ClsAddressStreet"."IsValid"  = TRUE
  AND "ClsAddressStreetNumber"."IsValid"  = TRUE
  AND "ClsAddressStreetName"."LocalizationId" = 'a09dd793-a60a-47ff-b43b-08d3c60ad4e0' -- just this language;


-- ***********************************************************

WITH x AS (
    SELECT DISTINCT xInner."Id"
    FROM (
        SELECT outNam."Name" , xStr."Id"
        FROM "ClsAddressStreet" AS xStr
            INNER JOIN "ClsAddressStreetName" AS inNam ON xStr."Id" = inNam."ClsAddressStreetId"
            INNER JOIN "ClsAddressStreetName" AS outNam ON xStr."Id" = outNam."ClsAddressStreetId"
            INNER JOIN "ClsAddressStreetNumber" AS xNum ON xStr."Id" = xNum."ClsAddressStreetId"
            INNER JOIN "PostalCode" AS xPC ON xNum."PostalCodeId" = xPC."Id"
        WHERE inNam."Name3"  = 'kuu'
          AND (
              (  inNam."NonCls"  = FALSE AND xNum."NonCls"  = FALSE )
                  OR
              ( EXISTS (SELECT 1 FROM "ClsStreetNumberCoordinate" AS xCoord WHERE xCoord."RelatedToId" = xNum."Id" )) )
          AND ( LOWER(inNam."Name") LIKE CONCAT('kuusitie', '%') )
          AND outNam."LocalizationId"  = 'a09dd793-a60a-47ff-b43b-08d3c60ad4e0'
        ORDER BY outNam."Name" ASC , xPC."Code" ASC )
        AS xInner
        LIMIT 50 OFFSET 0)
SELECT DISTINCT "ClsAddressStreet"."Id" , "ClsAddressStreet"."MunicipalityId" , "ClsAddressStreet"."IsValid" , "ClsAddressStreet"."NonCls" , "ClsAddressStreetName"."ClsAddressStreetId" ,
                "ClsAddressStreetName"."LocalizationId" , "ClsAddressStreetName"."Name" , "ClsAddressStreetName"."Name3" , "ClsAddressStreetName"."NonCls" , "ClsAddressStreetNumber"."Id" ,
                "ClsAddressStreetNumber"."ClsAddressStreetId" , "ClsAddressStreetNumber"."PostalCodeId" , "ClsAddressStreetNumber"."IsEven" , "ClsAddressStreetNumber"."StartNumber" ,
                "ClsAddressStreetNumber"."EndNumber" , "ClsAddressStreetNumber"."IsValid" , "ClsAddressStreetNumber"."NonCls" , "PostalCode"."Id" , "PostalCode"."Code" , "PostalCode"."MunicipalityId" ,
                "PostalCode"."IsValid" , "PostalCodeName"."PostalCodeId" , "PostalCodeName"."LocalizationId" , "PostalCodeName"."Name" , "Municipality"."Id" , "Municipality"."Code" ,
                "Municipality"."IsValid"
FROM "Municipality"
    INNER JOIN "ClsAddressStreet" ON "Municipality"."Id" = "ClsAddressStreet"."MunicipalityId"
    JOIN x ON x."Id" = "ClsAddressStreet"."Id"
    INNER JOIN "ClsAddressStreetName" ON "ClsAddressStreet"."Id" = "ClsAddressStreetName"."ClsAddressStreetId"
    INNER JOIN "ClsAddressStreetNumber" ON "ClsAddressStreet"."Id" = "ClsAddressStreetNumber"."ClsAddressStreetId"
    INNER JOIN "PostalCode" ON "ClsAddressStreetNumber"."PostalCodeId" = "PostalCode"."Id"
    INNER JOIN "PostalCodeName" ON "PostalCode"."Id" = "PostalCodeName"."PostalCodeId"
WHERE "ClsAddressStreet"."IsValid"  = TRUE
  AND "ClsAddressStreetNumber"."IsValid"  = TRUE
  AND "ClsAddressStreetName"."LocalizationId" = 'a09dd793-a60a-47ff-b43b-08d3c60ad4e0'
ORDER BY "ClsAddressStreetName"."Name" ASC , "PostalCodeName"."Name" ASC ;

SELECT distinct str."Id", ordering."Name", pcn."Name", concat(ordering."Name", pcn."Name")
FROM "ClsAddressStreet" as str
join "ClsAddressStreetName" as search on str."Id" = search."ClsAddressStreetId"
join "ClsAddressStreetName" as ordering on str."Id" = ordering."ClsAddressStreetId"
join "ClsAddressStreetNumber" as num on str."Id" = num."ClsAddressStreetId"
join "PostalCode" as pc on num."PostalCodeId" = pc."Id"
join "PostalCodeName" as pcn on pc."Id" = pcn."PostalCodeId"
where search."Name3" = 'kuu'
and lower(search."Name") like 'kuusitie%'
and ordering."LocalizationId" = (
    select abc."LocalizationId" from (select xNam."LocalizationId",
             case xNam."LocalizationId" when 'a09dd793-a60a-47ff-b43b-08d3c60ad4e0' then 0 else 1 end as "NameOrder"
    from "ClsAddressStreetName" as xNam
    where xNam."ClsAddressStreetId" = str."Id"
    order by "NameOrder"
    limit 1) as abc)
and pcn."LocalizationId" = (select abc."LocalizationId" from (select xPCN."LocalizationId",
             case xPCN."LocalizationId" when 'a09dd793-a60a-47ff-b43b-08d3c60ad4e0' then 0 else 1 end as "NameOrder"
    from "PostalCodeName" as xPCN
    where xPCN."PostalCodeId" = pc."Id"
    order by "NameOrder"
    limit 1) abc)
order by ordering."Name", pcn."Name";

-- ************************************************************************************************************

WITH x AS (
    SELECT DISTINCT xStr."Id" , xOrdering."Name" , xPCN."Name"
    FROM "PostalCodeName" AS xPCN
        INNER JOIN "PostalCode" AS xPC ON xPCN."PostalCodeId" = xPC."Id"
        INNER JOIN "ClsAddressStreetNumber" AS xNum ON xPC."Id" = xNum."PostalCodeId"
        INNER JOIN "ClsAddressStreetName" AS xSearch ON xNum."ClsAddressStreetId" = xSearch."ClsAddressStreetId"
        INNER JOIN "ClsAddressStreet" AS xStr ON xSearch."ClsAddressStreetId" = xStr."Id"
        INNER JOIN "ClsAddressStreetName" AS xOrdering ON xStr."Id" = xOrdering."ClsAddressStreetId"
    WHERE xSearch."Name3"  = 'kuu'
      AND ( LOWER(xSearch."Name") LIKE CONCAT('kuusitie', '%') )
      AND (
          (  xSearch."NonCls"  = FALSE AND xNum."NonCls"  = FALSE )
              OR
          ( EXISTS (SELECT 1 FROM "ClsStreetNumberCoordinate" AS xCoord WHERE xCoord."RelatedToId" = xNum."Id" )) )
      AND xOrdering."LocalizationId"  = (
          SELECT xSubquery."LocalizationId" FROM (
              SELECT innerOrdering."LocalizationId" ,
                     CASE innerOrdering."LocalizationId" WHEN 'a09dd793-a60a-47ff-b43b-08d3c60ad4e0' THEN 0 ELSE 1 END AS NameOrder
              FROM "ClsAddressStreetName" AS innerOrdering
              WHERE innerOrdering."ClsAddressStreetId"  =  xStr."Id"
              ORDER BY NameOrder ASC
              LIMIT 1
              ) AS xSubquery
          )
      AND xPCN."LocalizationId"  = (
          SELECT xSubquery."LocalizationId"
          FROM (
              SELECT innerPCN."LocalizationId" ,
                     CASE innerPCN."LocalizationId" WHEN 'a09dd793-a60a-47ff-b43b-08d3c60ad4e0' THEN 0 ELSE 1 END AS NameOrder
              FROM "PostalCodeName" AS innerPCN
              WHERE innerPCN."PostalCodeId"  =  xPC."Id"
              ORDER BY NameOrder ASC LIMIT 1
              ) AS xSubquery
          )
    ORDER BY xOrdering."Name" ASC , xPCN."Name" ASC
    LIMIT 50 OFFSET 0)
SELECT DISTINCT "ClsAddressStreet"."Id" , "ClsAddressStreet"."MunicipalityId" , "ClsAddressStreet"."IsValid" , "ClsAddressStreet"."NonCls" , "ClsAddressStreetName"."ClsAddressStreetId" ,
                "ClsAddressStreetName"."LocalizationId" , "ClsAddressStreetName"."Name" , "ClsAddressStreetName"."Name3" , "ClsAddressStreetName"."NonCls" , "ClsAddressStreetNumber"."Id" ,
                "ClsAddressStreetNumber"."ClsAddressStreetId" , "ClsAddressStreetNumber"."PostalCodeId" , "ClsAddressStreetNumber"."IsEven" , "ClsAddressStreetNumber"."StartNumber" ,
                "ClsAddressStreetNumber"."EndNumber" , "ClsAddressStreetNumber"."IsValid" , "ClsAddressStreetNumber"."NonCls" , "PostalCode"."Id" , "PostalCode"."Code" , "PostalCode"."MunicipalityId" ,
                "PostalCode"."IsValid" , "PostalCodeName"."PostalCodeId" , "PostalCodeName"."LocalizationId" , "PostalCodeName"."Name" , "Municipality"."Id" , "Municipality"."Code" ,
                "Municipality"."IsValid"
FROM "Municipality"
    INNER JOIN "ClsAddressStreet" ON "Municipality"."Id" = "ClsAddressStreet"."MunicipalityId"
    JOIN x ON x."Id" = "ClsAddressStreet"."Id"
    INNER JOIN "ClsAddressStreetName" ON "ClsAddressStreet"."Id" = "ClsAddressStreetName"."ClsAddressStreetId"
    INNER JOIN "ClsAddressStreetNumber" ON "ClsAddressStreet"."Id" = "ClsAddressStreetNumber"."ClsAddressStreetId"
    INNER JOIN "PostalCode" ON "ClsAddressStreetNumber"."PostalCodeId" = "PostalCode"."Id"
    INNER JOIN "PostalCodeName" ON "PostalCode"."Id" = "PostalCodeName"."PostalCodeId"
WHERE "ClsAddressStreet"."IsValid"  = TRUE AND "ClsAddressStreetNumber"."IsValid"  = TRUE
ORDER BY "PostalCode"."Code";