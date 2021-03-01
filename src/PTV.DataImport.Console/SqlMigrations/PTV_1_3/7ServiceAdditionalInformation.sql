INSERT INTO public."DescriptionType"(
            "Id", "Code", "Created", "CreatedBy", "Modified", "ModifiedBy", "OrderNumber")
   SELECT "Id", "Code" || 'AdditionalInfo' code, "Created", "CreatedBy", "Modified", "ModifiedBy", null
  FROM public."ServiceAdditionalInformationType";



INSERT INTO public."ServiceDescription"(
            "ServiceId", "TypeId", "LocalizationId", "Created", "CreatedBy", "Description", "Modified", "ModifiedBy")

    SELECT "ServiceId", "TypeId", "LocalizationId", "Created", "CreatedBy", "Text", "Modified", "ModifiedBy"
  FROM public."ServiceAdditionalInformation";
