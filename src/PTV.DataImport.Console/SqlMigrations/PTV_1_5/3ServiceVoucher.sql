CREATE OR REPLACE FUNCTION migrateServiceVoucher() RETURNS void AS
$BODY$
DECLARE    DECLARE
    --os_row os%rowtype;
    os_row RECORD;
    oswp_row RECORD;
    osai_row RECORD;
    webPageId uuid;
BEGIN
  FOR os_row IN 
  SELECT "Id", "Created", "CreatedBy", "Modified", "ModifiedBy", "ServiceVersionedId" 
    FROM "OrganizationService"
    WHERE "ProvisionTypeId" = (SELECT "Id" FROM "ProvisionType" WHERE "Code" = 'VoucherServices')
  LOOP
    
    FOR oswp_row IN 
    SELECT "WebPageId", "LocalizationId" 
      FROM "OrganizationServiceWebPage" oswp
      JOIN "WebPage" wp ON oswp."WebPageId" = wp."Id"
      WHERE oswp."OrganizationServiceId" = os_row."Id"
    LOOP
      
      BEGIN
        SELECT "Id", "Text" INTO osai_row 
        FROM "OrganizationServiceAdditionalInformation" osai 
        WHERE "OrganizationServiceId" = os_row."Id" AND "LocalizationId" = oswp_row."LocalizationId";
      EXCEPTION
      WHEN NO_DATA_FOUND THEN
        osai_row := NULL;
      END;
      
      IF (osai_row."Id" IS NOT NULL) THEN
        UPDATE "WebPage" wp SET "Description" = osai_row."Text" WHERE wp."Id" = oswp_row."WebPageId";
        DELETE FROM "OrganizationServiceAdditionalInformation" WHERE "Id" = osai_row."Id";
      END IF;
            
      INSERT INTO "ServiceWebPage" ("ServiceVersionedId", "WebPageId", "Created", "CreatedBy", "Modified", "ModifiedBy") 
      VALUES(os_row."ServiceVersionedId", oswp_row."WebPageId", os_row."Created", os_row."CreatedBy", os_row."Modified", os_row."ModifiedBy");
      
      DELETE FROM "OrganizationServiceWebPage" WHERE "OrganizationServiceId" = os_row."Id" AND "WebPageId" = oswp_row."WebPageId";
            
    END LOOP;
    
    FOR osai_row IN 
      SELECT "Id", "Text", "LocalizationId"
      FROM "OrganizationServiceAdditionalInformation" 
      WHERE "OrganizationServiceId" = os_row."Id"
    LOOP
      
      INSERT INTO "WebPage"("Id", "Created", "CreatedBy", "LocalizationId", "Modified", "ModifiedBy", "Description")
      VALUES(uuid_generate_v4(), os_row."Created", os_row."CreatedBy", osai_row."LocalizationId", os_row."Modified", os_row."ModifiedBy", osai_row."Text")
      RETURNING "Id" INTO webPageId ;
      
      INSERT INTO "ServiceWebPage" ("ServiceVersionedId", "WebPageId", "Created", "CreatedBy", "Modified", "ModifiedBy") 
      VALUES(os_row."ServiceVersionedId", webPageId, os_row."Created", os_row."CreatedBy", os_row."Modified", os_row."ModifiedBy");

      DELETE FROM "OrganizationServiceAdditionalInformation" WHERE "Id" = osai_row."Id";
      
    END LOOP;
    
  END LOOP;
  
  DELETE FROM "OrganizationService" WHERE "ProvisionTypeId" = (SELECT "Id" FROM "ProvisionType" WHERE "Code" = 'VoucherServices'); 
  DELETE FROM "ProvisionTypeName" WHERE "TypeId" = (SELECT "Id" FROM "ProvisionType" WHERE "Code" = 'VoucherServices');
  DELETE FROM "ProvisionType" WHERE "Code" = 'VoucherServices';
  
  RETURN;
END
$BODY$
LANGUAGE plpgsql;

SELECT migrateServiceVoucher();
DROP FUNCTION migrateServiceVoucher();