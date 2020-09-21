CREATE OR REPLACE VIEW public."VBrokenLink" ("WebPageId", "Url", "OrganizationId", "IsException", "ExceptionComment", "ValidationDate", "EntityType", "SubEntityType", "EntityId", "ConnectedChannelId")
AS
-- Services
SELECT DISTINCT e."WebPageId", wp."Url", ver."OrganizationId", wp."IsException", wp."ExceptionComment", wp."ValidationDate", 'Service' AS "EntityType", typ."Code" as "SubEntityType", ver."Id", NULL::uuid
FROM "ServiceWebPage" AS e
         JOIN "WebPage" AS wp ON e."WebPageId" = wp."Id"
         JOIN "ServiceVersioned" AS ver ON e."ServiceVersionedId" = ver."Id"
         JOIN "PublishingStatusType" AS pst ON ver."PublishingStatusId" = pst."Id"
         JOIN "ServiceType" AS typ ON ver."TypeId" = typ."Id"
WHERE wp."IsBroken" = TRUE
  AND (pst."Code" = 'Published' OR pst."Code" = 'Draft' OR pst."Code" = 'Modified')
UNION
-- Channels
SELECT DISTINCT e."WebPageId", wp."Url", ver."OrganizationId", wp."IsException", wp."ExceptionComment", wp."ValidationDate", 'Channel' AS "EntityType", typ."Code" AS "SubEntityType", ver."Id", NULL::uuid
FROM "ServiceChannelWebPage" AS e
         JOIN "WebPage" AS wp ON e."WebPageId" = wp."Id"
         JOIN "ServiceChannelVersioned" AS ver ON e."ServiceChannelVersionedId" = ver."Id"
         JOIN "PublishingStatusType" AS pst ON ver."PublishingStatusId" = pst."Id"
         JOIN "ServiceChannelType" AS typ ON ver."TypeId" = typ."Id"
WHERE wp."IsBroken" = TRUE
  AND (pst."Code" = 'Published' OR pst."Code" = 'Draft' OR pst."Code" = 'Modified')
UNION
-- Connections
SELECT DISTINCT e."WebPageId", wp."Url", ver."OrganizationId", wp."IsException", wp."ExceptionComment", wp."ValidationDate", 'Connection' AS "EntityType", 'Connection' AS "SubEntityType", ver."Id", chanv."Id"
FROM "ServiceServiceChannelWebPage" AS e
         JOIN "WebPage" AS wp ON e."WebPageId" = wp."Id"
         JOIN "ServiceVersioned" AS ver ON e."ServiceId" = ver."UnificRootId"
         JOIN "ServiceChannelVersioned" AS chanv ON e."ServiceChannelId" = chanv."UnificRootId"
         JOIN "PublishingStatusType" AS pst ON ver."PublishingStatusId" = pst."Id"
WHERE wp."IsBroken" = TRUE
  AND chanv."PublishingStatusId" = ver."PublishingStatusId"
  AND pst."Code" = 'Published'
UNION
-- Organizations
SELECT DISTINCT e."WebPageId", wp."Url", ver."UnificRootId", wp."IsException", wp."ExceptionComment", wp."ValidationDate", 'Organization' AS "EntityType", 'Organization' AS "SubEntityType", ver."Id", NULL::uuid
FROM "OrganizationWebPage" AS e
         JOIN "WebPage" AS wp ON e."WebPageId" = wp."Id"
         JOIN "OrganizationVersioned" AS ver ON e."OrganizationVersionedId" = ver."Id"
         JOIN "PublishingStatusType" AS pst ON ver."PublishingStatusId" = pst."Id"
WHERE wp."IsBroken" = TRUE
  AND (pst."Code" = 'Published' OR pst."Code" = 'Draft' OR pst."Code" = 'Modified')
UNION
-- Laws
SELECT DISTINCT e."WebPageId", wp."Url", ver."OrganizationId", wp."IsException", wp."ExceptionComment", wp."ValidationDate", 'Service' AS "EntityType", typ."Code" AS "SubEntityType", ver."Id", NULL::uuid
FROM "LawWebPage" AS e
         JOIN "WebPage" AS wp ON e."WebPageId" = wp."Id"
         JOIN "ServiceLaw" AS sl ON sl."LawId" = e."LawId"
         JOIN "ServiceVersioned" AS ver ON sl."ServiceVersionedId" = ver."Id"
         JOIN "PublishingStatusType" AS pst ON ver."PublishingStatusId" = pst."Id"
         JOIN "ServiceType" AS typ ON ver."TypeId" = typ."Id"
WHERE wp."IsBroken" = TRUE
  AND (pst."Code" = 'Published' OR pst."Code" = 'Draft' OR pst."Code" = 'Modified')
UNION
-- E-Channels
SELECT DISTINCT e."WebPageId", wp."Url", ver."OrganizationId", wp."IsException", wp."ExceptionComment", wp."ValidationDate", 'Channel' AS "EntityType", 'EChannel' AS "SubEntityType", ver."Id", NULL::uuid
FROM "ElectronicChannelUrl" AS e
         JOIN "WebPage" AS wp ON e."WebPageId" = wp."Id"
         JOIN "ElectronicChannel" AS ec ON e."ElectronicChannelId" = ec."Id"
         JOIN "ServiceChannelVersioned" AS ver ON ec."ServiceChannelVersionedId" = ver."Id"
         JOIN "PublishingStatusType" AS pst ON ver."PublishingStatusId" = pst."Id"
WHERE wp."IsBroken" = TRUE
  AND (pst."Code" = 'Published' OR pst."Code" = 'Draft' OR pst."Code" = 'Modified')
UNION
-- Printable forms
SELECT DISTINCT e."WebPageId", wp."Url", ver."OrganizationId", wp."IsException", wp."ExceptionComment", wp."ValidationDate", 'Channel' AS "EntityType", 'PrintableForm' AS "SubEntityType", ver."Id", NULL::uuid
FROM "PrintableFormChannelUrl" AS e
         JOIN "WebPage" AS wp ON e."WebPageId" = wp."Id"
         JOIN "PrintableFormChannel" AS pfc ON e."PrintableFormChannelId" = pfc."Id"
         JOIN "ServiceChannelVersioned" AS ver ON pfc."ServiceChannelVersionedId" = ver."Id"
         JOIN "PublishingStatusType" AS pst ON ver."PublishingStatusId" = pst."Id"
WHERE wp."IsBroken" = TRUE
  AND (pst."Code" = 'Published' OR pst."Code" = 'Draft' OR pst."Code" = 'Modified')
UNION
-- Web channels
SELECT DISTINCT e."WebPageId", wp."Url", ver."OrganizationId", wp."IsException", wp."ExceptionComment", wp."ValidationDate", 'Channel' AS "EntityType", 'WebPage' AS "SubEntityType", ver."Id", NULL::uuid
FROM "WebpageChannelUrl" AS e
         JOIN "WebPage" AS wp ON e."WebPageId" = wp."Id"
         JOIN "WebpageChannel" AS wc ON e."WebpageChannelId" = wc."Id"
         JOIN "ServiceChannelVersioned" AS ver ON wc."ServiceChannelVersionedId" = ver."Id"
         JOIN "PublishingStatusType" AS pst ON ver."PublishingStatusId" = pst."Id"
WHERE wp."IsBroken" = TRUE
  AND (pst."Code" = 'Published' OR pst."Code" = 'Draft' OR pst."Code" = 'Modified');

ALTER TABLE public."VBrokenLink" OWNER TO postgres;
