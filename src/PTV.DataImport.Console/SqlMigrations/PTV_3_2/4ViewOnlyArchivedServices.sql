CREATE OR REPLACE VIEW public."VLastArchivedService"
AS SELECT DISTINCT ON (ev."UnificRootId") ev."UnificRootId",
    ev."Id",
    ev."PublishingStatusId",
    ev."Created",
    ev."CreatedBy",
    ev."Modified",
    ev."ModifiedBy",
    ev."LastOperationType",
    ev."VersioningId",
    ev."OrganizationId",
    ev."OriginalId",
    ev."Expiration",
    ev."TypeId",
    ev."StatutoryServiceGeneralDescriptionId"
   FROM "ServiceVersioned" ev
     JOIN "PublishingStatusType" pst ON ev."PublishingStatusId" = pst."Id"
  WHERE (pst."Code" = 'OldPublished'::text OR pst."Code" = 'Deleted'::text)
  AND not exists
    (
        SELECT 1
        FROM "ServiceVersioned" ev2
        JOIN "PublishingStatusType" pt ON ev2."PublishingStatusId" = pt."Id"
        WHERE ev2."UnificRootId" = ev."UnificRootId"
        and pt."Code" in ('Published'::text, 'Modified'::text, 'Draft'::text)
    )
  ORDER BY ev."UnificRootId", ev."Modified" DESC;