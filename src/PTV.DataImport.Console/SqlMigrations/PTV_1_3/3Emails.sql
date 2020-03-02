-- organization email
INSERT INTO public."Email"(
           "Id", "Created", "CreatedBy", "Description", "LocalizationId", "Modified", "ModifiedBy", "Value", "Visible")
           
 SELECT oe."Id", oe."Created", oe."CreatedBy", oed."Description", COALESCE(oed."LocalizationId", l."Id") LocalizationId, oe."Modified", oe."ModifiedBy", "Email", true
  FROM public."OrganizationEmail" oe 
  join public."Language" l on (l."Code" = 'fi')
  left outer join public."OrganizationEmailDescription" oed on (oe."Id" = oed."OrganizationEmailId");


UPDATE public."OrganizationEmail"
   SET "EmailId"="Id";

