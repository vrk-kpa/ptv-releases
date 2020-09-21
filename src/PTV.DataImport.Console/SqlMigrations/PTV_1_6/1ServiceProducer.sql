DO $$
DECLARE 

  producerId uuid;
  responsibleId uuid;
  defaultOrgId uuid;

  selfProducedId uuid;
  purchasedId uuid;
  otherId uuid;
  cmBy CONSTANT text := 'ServiceProducerMigration';
  selfProduced CONSTANT text := 'SelfProduced';
BEGIN
	
    SELECT "Id" INTO producerId FROM "RoleType" WHERE lower("Code") = 'producer';
    SELECT "Id" INTO responsibleId FROM "RoleType" WHERE lower("Code") = 'responsible';

    SELECT "Id" INTO selfProducedId FROM "ProvisionType" WHERE lower("Code") = 'selfproduced';
    SELECT "Id" INTO purchasedId FROM "ProvisionType" WHERE lower("Code") = 'purchaseservices';
    SELECT "Id" INTO otherId FROM "ProvisionType" WHERE lower("Code") = 'other';
    SELECT "Id" INTO defaultOrgId FROM "Organization" LIMIT 1;   
    
    -- migrate producers with provision type PurchaseServices and Others
	INSERT INTO "ServiceProducer" ("Id", "Created", "CreatedBy", "Modified", "ModifiedBy", "ProvisionTypeId", "ServiceVersionedId")
  	SELECT "Id", "Created", "CreatedBy", "Modified", "ModifiedBy", "ProvisionTypeId", "ServiceVersionedId" 
    FROM "OrganizationService"
    WHERE "RoleTypeId" = producerId AND "ProvisionTypeId" IN (purchasedId, otherId);

    -- migrate producers organizations with provision type PurchaseServices and Others
    INSERT INTO "ServiceProducerOrganization"("ServiceProducerId", "OrganizationId",  "Created", "CreatedBy", "Modified", "ModifiedBy")
	SELECT "Id", "OrganizationId", "Created", "CreatedBy", "Modified", "ModifiedBy"
	FROM "OrganizationService"
	WHERE "RoleTypeId" = producerId AND "ProvisionTypeId" IN (purchasedId, otherId) AND "OrganizationId" IS NOT NULL;
        
    -- delete producers with provision type PurchaseServices and Others
    DELETE FROM "OrganizationService" WHERE "RoleTypeId" = producerId AND "ProvisionTypeId" IN (purchasedId, otherId);
    
    -- migrate producers with provision type SelfProduced
	INSERT INTO "ServiceProducer" ("Id", "Created", "CreatedBy", "Modified", "ModifiedBy", "ProvisionTypeId", "ServiceVersionedId")
  	SELECT GenerateGuidByText("ServiceVersionedId", selfProduced), current_timestamp, cmBy, current_timestamp, cmBy, selfProducedId, "ServiceVersionedId"
	FROM "OrganizationService"
	WHERE "RoleTypeId" = producerId AND "OrganizationId" IS NOT NULL AND ("ProvisionTypeId" = selfProducedId OR "ProvisionTypeId" IS NULL)
    GROUP BY "ServiceVersionedId";
    
    INSERT INTO "ServiceProducerOrganization"("ServiceProducerId", "OrganizationId",  "Created", "CreatedBy", "Modified", "ModifiedBy")
	SELECT GenerateGuidByText("ServiceVersionedId", selfProduced), "OrganizationId", current_timestamp, cmBy, current_timestamp, cmBy
	FROM "OrganizationService"
	WHERE "RoleTypeId" = producerId AND ("ProvisionTypeId" = selfProducedId OR "ProvisionTypeId" IS NULL) AND "OrganizationId" IS NOT NULL
    GROUP BY "ServiceVersionedId", "OrganizationId";

    DELETE FROM "OrganizationService" WHERE "RoleTypeId" = producerId AND "ProvisionTypeId" = selfProducedId;
    DELETE FROM "OrganizationService" WHERE "RoleTypeId" = producerId AND "ProvisionTypeId" IS NULL;

    -- delete "responsible" role duplicities from OrganizationServices
    DELETE FROM "OrganizationService"
    WHERE "Id" IN (
        SELECT "Id"
        FROM 
        (
            SELECT "Id", "RoleTypeId", ROW_NUMBER()
            OVER (PARTITION BY "OrganizationId", "ServiceVersionedId", "RoleTypeId" ORDER BY "Id") AS rnum
            FROM "OrganizationService"
        ) t
        WHERE t.rnum > 1 AND "RoleTypeId" = responsibleId)
;
    UPDATE "OrganizationService" SET "OrganizationId" = defaultOrgId WHERE "OrganizationId" is null;
END $$;

