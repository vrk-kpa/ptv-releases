UPDATE public."ServiceVersioned"
 SET "Modified" = to_timestamp('31.01.2018', 'DD-MM-YYYY')
 WHERE 
"Modified" <= '2018-01-30 00:00:00';

UPDATE public."ServiceChannelVersioned"
 SET "Modified" = to_timestamp('31.01.2018', 'DD-MM-YYYY')
 WHERE 
"Modified" <= '2018-01-30 00:00:00';

UPDATE public."OrganizationVersioned"
 SET "Modified" = to_timestamp('31.01.2018', 'DD-MM-YYYY')
 WHERE 
"Modified" <= '2018-01-30 00:00:00';

 UPDATE public."StatutoryServiceGeneralDescriptionVersioned"
 SET "Modified" = to_timestamp('31.01.2018', 'DD-MM-YYYY')
 WHERE 
"Modified" <= '2018-01-30 00:00:00';


UPDATE public."ServiceChannelLanguageAvailability"
 SET "Modified" = to_timestamp('31.01.2018', 'DD-MM-YYYY')
WHERE  
 "ServiceChannelVersionedId" IN (SELECT "Id" FROM "ServiceChannelVersioned" WHERE "Modified" = '2018-01-31 00:00:00')
 AND
 "Modified" <= '2018-01-30 00:00:00';

UPDATE public."ServiceLanguageAvailability"
 SET "Modified" = to_timestamp('31.01.2018', 'DD-MM-YYYY')
WHERE  
 "ServiceVersionedId" IN (SELECT "Id" FROM "ServiceVersioned" WHERE "Modified" = '2018-01-31 00:00:00')
 AND
 "Modified" <= '2018-01-30 00:00:00';

UPDATE public."GeneralDescriptionLanguageAvailability"
 SET "Modified" = to_timestamp('31.01.2018', 'DD-MM-YYYY')
WHERE  
 "StatutoryServiceGeneralDescriptionVersionedId" IN (SELECT "Id" FROM "StatutoryServiceGeneralDescriptionVersioned" WHERE "Modified" = '2018-01-31 00:00:00')
 AND
"Modified" <= '2018-01-30 00:00:00';

UPDATE public."OrganizationLanguageAvailability"
 SET "Modified" = to_timestamp('31.01.2018', 'DD-MM-YYYY')
WHERE  
 "OrganizationVersionedId" IN (SELECT "Id" FROM "OrganizationVersioned" WHERE "Modified" = '2018-01-31 00:00:00')
 AND
 "Modified" <= '2018-01-30 00:00:00';