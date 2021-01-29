DELETE FROM "StreetName" WHERE ("Text" = '') OR ("Text" IS NULL);
DELETE FROM "PostOfficeBoxName" WHERE ("Text" = '') OR ("Text" IS NULL);

UPDATE "Address" a SET "TypeId" = sub."Id" FROM (SELECT "Id" FROM "AddressType" WHERE "Code" = 'PostOfficeBox') AS sub WHERE a."Id" IN (SELECT "AddressId" FROM "PostOfficeBoxName");
UPDATE "Address" a SET "TypeId" = sub."Id" FROM (SELECT "Id" FROM "AddressType" WHERE "Code" = 'Street') AS sub WHERE a."Id" IN (SELECT "AddressId" FROM "StreetName");
UPDATE "Address" a SET "TypeId" = sub."Id" FROM (SELECT "Id" FROM "AddressType" WHERE "Code" = 'Street') AS sub WHERE "TypeId" = '00000000-0000-0000-0000-000000000000';

INSERT INTO "AddressStreet"("AddressId","MunicipalityId", "PostalCodeId","StreetNumber", "Created","Modified") (SELECT "Id" as "AddressId", "MunicipalityId", "PostalCodeId", "StreetNumber", "Created", "Modified" FROM "Address" WHERE "Id" IN (SELECT "AddressId" FROM "StreetName"));  
INSERT INTO "AddressPostOfficeBox"("AddressId","MunicipalityId", "PostalCodeId", "Created","Modified") (SELECT "Id" as "AddressId", "MunicipalityId", "PostalCodeId", "Created", "Modified" FROM "Address" WHERE "Id" IN (SELECT "AddressId" FROM "PostOfficeBoxName"));  