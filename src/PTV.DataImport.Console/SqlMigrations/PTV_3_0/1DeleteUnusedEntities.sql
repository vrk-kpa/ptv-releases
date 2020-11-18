-- Delete unused versionings.
DELETE
FROM "Versioning" AS x
WHERE "UnificRootId" NOT IN (SELECT DISTINCT ver."UnificRootId"
                             FROM "Versioning" AS ver
                                      JOIN "ServiceVersioned" AS sv ON ver."Id" = sv."VersioningId")
  AND "UnificRootId" NOT IN (SELECT DISTINCT ver."UnificRootId"
                             FROM "Versioning" AS ver
                                      JOIN "ServiceChannelVersioned" AS sv ON ver."Id" = sv."VersioningId")
  AND "UnificRootId" NOT IN (SELECT DISTINCT ver."UnificRootId"
                             FROM "Versioning" AS ver
                                      JOIN "ServiceCollectionVersioned" AS sv ON ver."Id" = sv."VersioningId")
  AND "UnificRootId" NOT IN (SELECT DISTINCT ver."UnificRootId"
                             FROM "Versioning" AS ver
                                      JOIN "OrganizationVersioned" AS sv ON ver."Id" = sv."VersioningId")
  AND "UnificRootId" NOT IN (SELECT DISTINCT ver."UnificRootId"
                             FROM "Versioning" AS ver
                                      JOIN "StatutoryServiceGeneralDescriptionVersioned" AS sv ON ver."Id" = sv."VersioningId");

-- Delete unused web pages.
DELETE
FROM "WebPage"
WHERE "Id" NOT IN (SELECT DISTINCT "WebPageId" FROM "LawWebPage")
  AND "Id" NOT IN (SELECT DISTINCT "WebPageId" FROM "ServiceWebPage")
  AND "Id" NOT IN (SELECT DISTINCT "WebPageId" FROM "ServiceChannelWebPage")
  AND "Id" NOT IN (SELECT DISTINCT "WebPageId" FROM "ServiceServiceChannelWebPage")
  AND "Id" NOT IN (SELECT DISTINCT "WebPageId" FROM "OrganizationWebPage")
  AND "Id" NOT IN (SELECT DISTINCT "WebPageId" FROM "ElectronicChannelUrl")
  AND "Id" NOT IN (SELECT DISTINCT "WebPageId" FROM "WebpageChannelUrl")
  AND "Id" NOT IN (SELECT DISTINCT "WebPageId" FROM "PrintableFormChannelUrl");

-- Delete unused laws.
DELETE
FROM "Law"
WHERE "Id" NOT IN (SELECT DISTINCT "LawId" FROM "ServiceLaw")
  AND "Id" NOT IN (SELECT DISTINCT "LawId" FROM "StatutoryServiceLaw");