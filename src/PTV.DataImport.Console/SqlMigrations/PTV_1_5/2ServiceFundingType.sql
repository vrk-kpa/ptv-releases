CREATE OR REPLACE FUNCTION GetOrCreateDefaultServiceFundingTypeId(varchar)
    RETURNS uuid LANGUAGE plpgsql AS $$
    DECLARE
      resId uuid;
    BEGIN
      SELECT "Id" INTO resId FROM "ServiceFundingType" WHERE "Code" = $1 LIMIT 1;      
      IF (resId IS NULL) THEN
          INSERT INTO public."ServiceFundingType"("Id", "Code", "Created", "CreatedBy", "Modified", "ModifiedBy", "OrderNumber")
          VALUES (public.generateguid(), $1, now(), 'ImportTask', now(), 'ImportTask', 0) RETURNING "Id" into resId;
          RETURN resId;
      END IF;
      RETURN resId; 
    END
    $$;  