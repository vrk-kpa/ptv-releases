ALTER TABLE "ServiceServiceChannel" ADD COLUMN "ServiceId" uuid
;

ALTER TABLE "ServiceServiceChannel" DROP CONSTRAINT "FK_ServiceServiceChannel_ServiceChannel_ServiceChannelId";
ALTER TABLE "ServiceServiceChannel" DROP CONSTRAINT "FK_ServiceServiceChannel_Service_ServiceVersionedId";

UPDATE "ServiceServiceChannel" ssc
SET "ServiceId" = sv."ServiceId"
FROM (
    SELECT "Id", "UnificRootId" "ServiceId"
    FROM "ServiceVersioned"
) AS sv
WHERE "ServiceVersionedId" = sv."Id"
;

DELETE FROM "ServiceServiceChannel" ssc1
WHERE EXISTS (
SELECT 1 
FROM "ServiceServiceChannel" ssc2
  WHERE ssc1."ServiceId" = ssc2."ServiceId"
  AND ssc1."ServiceChannelId" = ssc2."ServiceChannelId"
  AND ssc1.ctid > ssc2.ctid
)
;

ALTER TABLE "ServiceServiceChannel" DROP COLUMN "ServiceVersionedId" CASCADE
;