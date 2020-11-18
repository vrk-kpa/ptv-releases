CREATE OR REPLACE VIEW public."VLastArchivedService"
AS
SELECT DISTINCT ON ("UnificRootId") "UnificRootId", ev."Id", ev."PublishingStatusId", ev."Created", ev."CreatedBy", ev."Modified", ev."ModifiedBy",
    ev."LastOperationType", ev."VersioningId", ev."OrganizationId", ev."OriginalId", ev."Expiration", ev."TypeId", ev."StatutoryServiceGeneralDescriptionId"
FROM public."ServiceVersioned" AS ev
         JOIN "PublishingStatusType" AS pst ON ev."PublishingStatusId" = pst."Id"
WHERE pst."Code" = 'OldPublished' OR pst."Code" = 'Deleted'
ORDER BY "UnificRootId", ev."Modified" DESC;

CREATE OR REPLACE VIEW public."VLastArchivedServiceChannel"
AS
SELECT DISTINCT ON ("UnificRootId") "UnificRootId", ev."Id", ev."PublishingStatusId", ev."Created", ev."CreatedBy", ev."Modified", ev."ModifiedBy",
    ev."LastOperationType", ev."VersioningId", ev."OrganizationId", ev."OriginalId", ev."Expiration", ev."TypeId"
FROM public."ServiceChannelVersioned" AS ev
         JOIN "PublishingStatusType" AS pst ON ev."PublishingStatusId" = pst."Id"
WHERE pst."Code" = 'OldPublished' OR pst."Code" = 'Deleted'
ORDER BY "UnificRootId", ev."Modified" DESC;

CREATE OR REPLACE VIEW public."VLastArchivedServiceCollection"
AS
SELECT DISTINCT ON ("UnificRootId") "UnificRootId", ev."Id", ev."PublishingStatusId", ev."Created", ev."CreatedBy", ev."Modified", ev."ModifiedBy",
    ev."LastOperationType", ev."VersioningId", ev."OrganizationId", ev."OriginalId"
FROM public."ServiceCollectionVersioned" AS ev
         JOIN "PublishingStatusType" AS pst ON ev."PublishingStatusId" = pst."Id"
WHERE pst."Code" = 'OldPublished' OR pst."Code" = 'Deleted'
ORDER BY "UnificRootId", ev."Modified" DESC;

CREATE OR REPLACE VIEW public."VLastArchivedOrganization"
AS
SELECT DISTINCT ON ("UnificRootId") "UnificRootId", ev."Id", ev."PublishingStatusId", ev."Created", ev."CreatedBy", ev."Modified", ev."ModifiedBy",
    ev."LastOperationType", ev."VersioningId", ev."ParentId"
FROM public."OrganizationVersioned" AS ev
         JOIN "PublishingStatusType" AS pst ON ev."PublishingStatusId" = pst."Id"
WHERE pst."Code" = 'OldPublished' OR pst."Code" = 'Deleted'
ORDER BY "UnificRootId", ev."Modified" DESC;

CREATE OR REPLACE VIEW public."VLastArchivedGeneralDescription"
AS
SELECT DISTINCT ON ("UnificRootId") "UnificRootId", ev."Id", ev."PublishingStatusId", ev."Created", ev."CreatedBy", ev."Modified", ev."ModifiedBy",
    ev."LastOperationType", ev."VersioningId", ev."TypeId", ev."GeneralDescriptionTypeId"
FROM public."StatutoryServiceGeneralDescriptionVersioned" AS ev
         JOIN "PublishingStatusType" AS pst ON ev."PublishingStatusId" = pst."Id"
WHERE pst."Code" = 'OldPublished' OR pst."Code" = 'Deleted'
ORDER BY "UnificRootId", ev."Modified" DESC;
