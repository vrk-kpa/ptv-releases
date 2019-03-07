CREATE OR REPLACE FUNCTION GetPublishingStatusId(text) RETURNS uuid LANGUAGE SQL AS  $$ SELECT "Id" FROM "PublishingStatusType" where "Code" = $1; $$;

UPDATE public."ServiceVersioned"
 SET "Modified" = TIMESTAMP '11.9.2017'
 WHERE 
 "Modified" < '11.9.2017'
 and "PublishingStatusId" != GetPublishingStatusId('OldPublished') and "PublishingStatusId" != GetPublishingStatusId('Deleted');


UPDATE public."ServiceChannelVersioned"
  SET "Modified" = TIMESTAMP '11.9.2017'
 WHERE 
 "Modified" < '11.9.2017'
 and "PublishingStatusId" != GetPublishingStatusId('OldPublished') and "PublishingStatusId" != GetPublishingStatusId('Deleted');


UPDATE public."OrganizationVersioned"
  SET "Modified" = TIMESTAMP '11.9.2017'
 WHERE 
 "Modified" < '11.9.2017'
 and "PublishingStatusId" != GetPublishingStatusId('OldPublished') and "PublishingStatusId" != GetPublishingStatusId('Deleted');

 UPDATE public."StatutoryServiceGeneralDescriptionVersioned"
  SET "Modified" = TIMESTAMP '11.9.2017'
 WHERE 
 "Modified" < '11.9.2017'
 and "PublishingStatusId" != GetPublishingStatusId('OldPublished') and "PublishingStatusId" != GetPublishingStatusId('Deleted');


 -- select * from public."ServiceVersioned" WHERE "Modified" < '11.9.2017' and "PublishingStatusId" != GetPublishingStatusId('OldPublished') and "PublishingStatusId" != GetPublishingStatusId('Deleted');
 -- select * from public."ServiceChannelVersioned" WHERE "Modified" < '11.9.2017' and "PublishingStatusId" != GetPublishingStatusId('OldPublished') and "PublishingStatusId" != GetPublishingStatusId('Deleted');
 -- select * from public."OrganizationVersioned" WHERE "Modified" < '11.9.2017' and "PublishingStatusId" != GetPublishingStatusId('OldPublished') and "PublishingStatusId" != GetPublishingStatusId('Deleted');
 -- select * from public."StatutoryServiceGeneralDescriptionVersioned" WHERE "Modified" < '11.9.2017' and "PublishingStatusId" != GetPublishingStatusId('OldPublished') and "PublishingStatusId" != GetPublishingStatusId('Deleted');