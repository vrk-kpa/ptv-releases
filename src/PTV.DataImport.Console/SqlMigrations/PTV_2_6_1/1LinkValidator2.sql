-- Services
UPDATE "ServiceWebPage" AS swp
SET "Name" = wp."Name",
    "Description" = wp."Description",
    "LocalizationId" = wp."LocalizationId",
    "OrderNumber" = CASE WHEN swp."OrderNumber" IS NULL THEN wp."OrderNumber" ELSE swp."OrderNumber" END
FROM "WebPage" as wp
WHERE swp."WebPageId" = wp."Id";

-- Channels
UPDATE "ServiceChannelWebPage" AS cwp
SET "Name" = wp."Name",
    "Description" = wp."Description",
    "LocalizationId" = wp."LocalizationId",
    "OrderNumber" = CASE WHEN cwp."OrderNumber" IS NULL THEN wp."OrderNumber" ELSE cwp."OrderNumber" END
FROM "WebPage" as wp
WHERE cwp."WebPageId" = wp."Id";

-- Connections
UPDATE "ServiceServiceChannelWebPage" AS jwp
SET "Name" = wp."Name",
    "Description" = wp."Description",
    "LocalizationId" = wp."LocalizationId",
    "OrderNumber" = CASE WHEN jwp."OrderNumber" IS NULL THEN wp."OrderNumber" ELSE jwp."OrderNumber" END
FROM "WebPage" as wp
WHERE jwp."WebPageId" = wp."Id";

-- Organizations
UPDATE "OrganizationWebPage" AS owp
SET "Name" = wp."Name",
    "Description" = wp."Description",
    "LocalizationId" = wp."LocalizationId",
    "OrderNumber" = CASE WHEN owp."OrderNumber" IS NULL THEN wp."OrderNumber" ELSE owp."OrderNumber" END
FROM "WebPage" as wp
WHERE owp."WebPageId" = wp."Id";

-- Laws
UPDATE "LawWebPage" AS lwp
SET "Name" = wp."Name",
    "Description" = wp."Description",
    "LocalizationId" = wp."LocalizationId",
    "OrderNumber" = CASE WHEN lwp."OrderNumber" IS NULL THEN wp."OrderNumber" ELSE lwp."OrderNumber" END
FROM "WebPage" as wp
WHERE lwp."WebPageId" = wp."Id";

-- Temporary insert any valid WebPage.Id to the following tables in order to be able to create FK reference.
UPDATE "WebpageChannelUrl"
SET "WebPageId" = (SELECT "Id" FROM "WebPage" LIMIT 1);

UPDATE "PrintableFormChannelUrl"
SET "WebPageId" = (SELECT "Id" FROM "WebPage" LIMIT 1);

UPDATE "ElectronicChannelUrl"
SET "WebPageId" = (SELECT "Id" FROM "WebPage" LIMIT 1);

-- Views
DROP VIEW IF EXISTS "geoServiceChannelWebPage";

-- Re-created in the next script
