INSERT INTO public."OrganizationDisplayNameType"("OrganizationVersionedId", "LocalizationId", "Created", "CreatedBy", "DisplayNameTypeId", "Modified", "ModifiedBy")
SELECT ov."Id", GetLanguageId('fi'), ov."Created", ov."CreatedBy", ov."DisplayNameTypeId", ov."Modified", ov."ModifiedBy"
FROM  public."OrganizationVersioned"  ov;

INSERT INTO public."OrganizationDisplayNameType"("OrganizationVersionedId", "LocalizationId", "Created", "CreatedBy", "DisplayNameTypeId", "Modified", "ModifiedBy")
SELECT ov."Id", GetLanguageId('sv'), ov."Created", ov."CreatedBy", ov."DisplayNameTypeId", ov."Modified", ov."ModifiedBy"
FROM  public."OrganizationVersioned"  ov;
