UPDATE public."OrganizationVersioned" o SET "PublishingStatusId" = p."Id" FROM public."PublishingStatusType" p WHERE p."Code" = 'Published' AND o."PublishingStatusId" IS NULL;
UPDATE public."ServiceVersioned" o SET "PublishingStatusId" = p."Id" FROM public."PublishingStatusType" p WHERE p."Code" = 'Published' AND o."PublishingStatusId" IS NULL;
UPDATE public."ServiceChannelVersioned" o SET "PublishingStatusId" = p."Id" FROM public."PublishingStatusType" p WHERE p."Code" = 'Published' AND o."PublishingStatusId" IS NULL;
UPDATE public."StatutoryServiceGeneralDescriptionVersioned" o SET "PublishingStatusId" = p."Id" FROM public."PublishingStatusType" p WHERE p."Code" = 'Published' AND o."PublishingStatusId" IS NULL;
