INSERT INTO "GeneralDescriptionType"("Id", "Code", "Created", "CreatedBy", "Modified", "ModifiedBy") VALUES (GenerateGuid(), 'Municipality', Now(), 'PTVApp', Now(), 'PTVapp');

CREATE OR REPLACE FUNCTION GeneralDescriptionType_Default() RETURNS uuid LANGUAGE SQL AS  $$ SELECT "Id" FROM "GeneralDescriptionType" WHERE "Code" = 'Municipality'; $$;