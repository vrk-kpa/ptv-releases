DO $$
DECLARE 
  addressCharacterId uuid;
  countryId uuid; 
  addressTypeId uuid;
  deliveryAddressId uuid;
BEGIN
	
    SELECT "Id" INTO addressCharacterId FROM "AddressCharacter" WHERE LOWER ("Code") = 'delivery';   
            
    INSERT INTO "ServiceChannelAddress" ("ServiceChannelVersionedId", "AddressId", "CharacterId", "Created", "CreatedBy", "Modified", "ModifiedBy")
    SELECT "ServiceChannelVersionedId", "DeliveryAddressId", addressCharacterId, "Created", "CreatedBy", "Modified", "ModifiedBy"
    FROM "PrintableFormChannel"
    WHERE "DeliveryAddressId" IS NOT NULL;

    UPDATE "AddressReceiver" ar 
    SET "AddressId" = pfc."DeliveryAddressId"
    FROM "PrintableFormChannel" pfc 
    WHERE ar."PrintableFormChannelId" = pfc."Id" AND pfc."DeliveryAddressId" IS NOT NULL;

    SELECT "Id" INTO countryId FROM "Country" WHERE LOWER("Code") = 'fi'; 
    SELECT "Id" INTO addressTypeId FROM "AddressType" WHERE LOWER("Code") = 'noaddress'; 
        
    INSERT INTO "Address" ("Id", "UniqueId", "CountryId", "TypeId", "Created", "Modified", "CreatedBy", "ModifiedBy")
    SELECT GenerateGuidByText(pfc."Id", 'address'), GenerateGuidByText(pfc."Id", 'address'),  countryId, addressTypeId, pfc."Created", pfc."Modified", pfc."CreatedBy", pfc."ModifiedBy"
    FROM "PrintableFormChannel" pfc WHERE pfc."DeliveryAddressId" IS NULL AND EXISTS (SELECT 1 FROM "AddressReceiver" pfcr WHERE pfcr."PrintableFormChannelId" = pfc."Id");
    
    INSERT INTO "ServiceChannelAddress" ("ServiceChannelVersionedId", "AddressId", "CharacterId", "Created", "CreatedBy", "Modified", "ModifiedBy")
    SELECT "ServiceChannelVersionedId", GenerateGuidByText(pfc."Id", 'address'), addressCharacterId, "Created", "CreatedBy", "Modified", "ModifiedBy"
    FROM "PrintableFormChannel" pfc WHERE pfc."DeliveryAddressId" IS NULL AND EXISTS (SELECT 1 FROM "AddressReceiver" pfcr WHERE pfcr."PrintableFormChannelId" = pfc."Id");

    UPDATE "AddressReceiver" ar 
    SET "AddressId" = GenerateGuidByText(pfc."Id", 'address')
    FROM "PrintableFormChannel" pfc 
    WHERE ar."PrintableFormChannelId" = pfc."Id" AND pfc."DeliveryAddressId" IS NULL;
        
    -- workaround
    -- delete duplicity in AddressReceiver table (one addreessId is used for more channels 
    -- (invalid data in printableFormChannel table - one addressId has been used for more service channels)
    FOR deliveryAddressId IN SELECT "DeliveryAddressId"
                          FROM "PrintableFormChannel" pfc
                          GROUP BY "DeliveryAddressId"
                          HAVING COUNT ("DeliveryAddressId") > 1
    LOOP
      
      DELETE FROM "AddressReceiver"
      WHERE "PrintableFormChannelId" IN 
      (
        SELECT "Id"
        FROM
        (
          SELECT *, row_number() OVER (ORDER BY "Modified" DESC) RowNumber
          FROM "PrintableFormChannel"
          WHERE "DeliveryAddressId" = deliveryAddressId
        ) tt
        WHERE RowNumber != 1
      );
      
	  END LOOP;
	  
	  -- migrate service channel locations
	  INSERT INTO "ServiceChannelAddress" ("ServiceChannelVersionedId", "AddressId", "CharacterId", "Created", "CreatedBy", "Modified", "ModifiedBy")
    SELECT slc."ServiceChannelVersionedId", slca."AddressId", slca."CharacterId", slca."Created", slca."CreatedBy", slca."Modified", slca."ModifiedBy"
    FROM "ServiceLocationChannelAddress" slca
    JOIN "ServiceLocationChannel" slc ON slca."ServiceLocationChannelId" = slc."Id";
    
END $$;



