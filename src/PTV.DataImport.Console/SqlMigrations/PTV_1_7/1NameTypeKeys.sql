DELETE FROM "LifeEventName"
WHERE "Id" IN (SELECT "Id"
              FROM (SELECT "Id",
                             ROW_NUMBER() OVER (partition BY "LifeEventId", "LocalizationId" ORDER BY "Id") AS rnum
                     FROM "LifeEventName") t
              WHERE t.rnum > 1);

DELETE FROM "IndustrialClassName"
WHERE "Id" IN (SELECT "Id"
              FROM (SELECT "Id",
                             ROW_NUMBER() OVER (partition BY "IndustrialClassId", "LocalizationId" ORDER BY "Id") AS rnum
                     FROM "IndustrialClassName") t
              WHERE t.rnum > 1);