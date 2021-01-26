UPDATE "AddressStreet"
	SET "PostalCodeId"= NULL
	WHERE "PostalCodeId" = (SELECT "Id" FROM "PostalCode" WHERE "Code" = 'Undefined');

UPDATE "AddressPostOfficeBox"
	SET "PostalCodeId"= NULL
	WHERE "PostalCodeId" = (SELECT "Id" FROM "PostalCode" WHERE "Code" = 'Undefined'); 