INSERT INTO public."PostOfficeBoxName"("Id","AddressId", "LocalizationId", "Created", "CreatedBy", "Text", "Modified", "ModifiedBy")
SELECT GenerateGuidByText(addr."Id",'poBoxFi') Id, addr."Id", GetLanguageId('fi'), addr."Created", addr."CreatedBy", addr."PostOfficeBox", addr."Modified", addr."ModifiedBy"
FROM  public."Address"  addr
WHERE length(addr."PostOfficeBox")>0;

INSERT INTO public."PostOfficeBoxName"("Id","AddressId", "LocalizationId", "Created", "CreatedBy", "Text", "Modified", "ModifiedBy")
SELECT GenerateGuidByText(addr."Id",'poBoxSv') Id, addr."Id", GetLanguageId('sv'), addr."Created", addr."CreatedBy", addr."PostOfficeBox", addr."Modified", addr."ModifiedBy"
FROM  public."Address"  addr
WHERE length(addr."PostOfficeBox")>0;
