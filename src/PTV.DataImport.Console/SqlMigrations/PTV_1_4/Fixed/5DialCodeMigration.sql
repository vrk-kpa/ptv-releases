INSERT INTO "Country"("Id", "Code", "Created", "CreatedBy", "Modified", "ModifiedBy")
   VALUES ('00107383-04e7-4321-95d9-bda979bb5899', 'FAKE', '2017-03-01 00:00:00.000000', 'PTVMigration', '2017-03-01 00:00:00.000000', 'PTVMigration');

INSERT INTO "DialCode"("Code", "Id", "CountryId", "Created", "CreatedBy", "Modified", "ModifiedBy")
       SELECT DISTINCT ON ("PrefixNumber") "PrefixNumber", "Id", '00107383-04e7-4321-95d9-bda979bb5899', '2017-03-01 00:00:00.000000', 'PTVMigration', '2017-03-01 00:00:00.000000', 'PTVMigration'
  FROM "Phone" WHERE "PrefixNumber" IS NOT NULL AND "PrefixNumber" <> '' AND "PrefixNumber" <> ' ';


UPDATE "Phone" p SET "PrefixNumberId" = d."Id" FROM "DialCode" d WHERE d."Code" = p."PrefixNumber";
