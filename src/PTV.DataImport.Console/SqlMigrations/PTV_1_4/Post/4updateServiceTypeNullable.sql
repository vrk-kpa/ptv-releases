
UPDATE public."ServiceVersioned"
	SET "TypeId"=null where "StatutoryServiceGeneralDescriptionId" IS NOT NULL;

ALTER TABLE "ServiceVersioned"
ADD CONSTRAINT "CK_TYPE_ID_XOR_STATUTORY_SERVICE_GENERAL_DESCRIPTION_ID" CHECK (("TypeId" IS NOT NULL) != ("StatutoryServiceGeneralDescriptionId" IS NOT NULL));