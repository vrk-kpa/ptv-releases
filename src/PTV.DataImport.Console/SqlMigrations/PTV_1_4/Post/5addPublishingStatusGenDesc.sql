UPDATE public."StatutoryServiceGeneralDescriptionVersioned"
   SET "PublishingStatusId"=(SELECT "Id" FROM public."PublishingStatusType" WHERE "Code" = 'Published')
 WHERE "PublishingStatusId" IS NULL;
