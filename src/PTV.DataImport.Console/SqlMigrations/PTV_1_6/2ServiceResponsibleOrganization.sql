DO $$
DECLARE 
  defaultOrgId uuid; 
BEGIN
	
    SELECT "Id" INTO defaultOrgId FROM "Organization" LIMIT 1;   
    
    UPDATE "ServiceVersioned" sv
	SET "OrganizationId" = os."OrganizationId"
	FROM (
		SELECT "OrganizationId","ServiceVersionedId"
		FROM "OrganizationService"	
	) AS os
	WHERE "Id" = os."ServiceVersionedId"
	;

	UPDATE "ServiceVersioned" sv
	SET "OrganizationId" = defaultOrgId	
	WHERE "OrganizationId" = '00000000-0000-0000-0000-000000000000'
	;

END $$;

