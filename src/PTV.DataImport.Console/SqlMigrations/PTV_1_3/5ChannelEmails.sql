-- service location
-- creates emails
INSERT INTO public."Email"(
            "Id", "Created", "CreatedBy", "Description", "LocalizationId", "Modified", "ModifiedBy", "Value", "Visible")


  SELECT GenerateGuidByText("Id", 'email') Id, "Created", "CreatedBy", null description, lang,"Modified", "ModifiedBy", "Email", true

  FROM public."ServiceLocationChannel", GetLanguageId('fi') lang where "Email" is not null and "Email" != '';

-- creates channel emails connection
INSERT INTO public."ServiceChannelEmail"(
            "Created", "CreatedBy", "Modified", "ModifiedBy", "ServiceChannelId", "EmailId")
    SELECT "Created", "CreatedBy", "Modified", "ModifiedBy", "ServiceChannelId", GenerateGuidByText("Id", 'email') EmailId
  FROM public."ServiceLocationChannel" where "Email" is not null and "Email" != '';


-- check support
-- creates emails
INSERT INTO public."Email"(
            "Id", "Created", "CreatedBy", "Description", "LocalizationId", "Modified", "ModifiedBy", "Value", "Visible")

  SELECT GenerateGuidByText("Id", 'email') Id, "Created", "CreatedBy", null description, "LocalizationId",  "Modified", "ModifiedBy", "Email", true
  FROM public."ServiceChannelSupport"
  where coalesce( trim("Email"),'')!='';

-- creates channel emails connection
INSERT INTO public."ServiceChannelEmail"(
            "Created", "CreatedBy", "Modified", "ModifiedBy", "ServiceChannelId", "EmailId")
    SELECT "Created", "CreatedBy", "Modified", "ModifiedBy", "ServiceChannelId", GenerateGuidByText("Id", 'email') EmailId
  FROM public."ServiceChannelSupport"
  where coalesce( trim("Email"),'')!='';


