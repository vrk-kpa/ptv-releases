INSERT INTO public."PrintableFormChannelReceiver"("PrintableFormChannelId", "LocalizationId", "Created", "CreatedBy", "FormReceiver", "Modified", "ModifiedBy")
SELECT pfc."Id", (SELECT "Id" FROM "Language" WHERE "Code" = 'fi'), pfc."Created", pfc."CreatedBy", pfc."FormReceiver", pfc."Modified", pfc."ModifiedBy"
FROM  public."PrintableFormChannel"  pfc;

INSERT INTO  public."PrintableFormChannelReceiver"("PrintableFormChannelId", "LocalizationId", "Created", "CreatedBy", "FormReceiver", "Modified", "ModifiedBy")
SELECT pfc."Id", (SELECT "Id" FROM "Language" WHERE "Code" = 'sv'), pfc."Created", pfc."CreatedBy", pfc."FormReceiver", pfc."Modified", pfc."ModifiedBy"
FROM  public."PrintableFormChannel"  pfc;


INSERT INTO  public."PrintableFormChannelIdentifier"("PrintableFormChannelId", "LocalizationId", "Created", "CreatedBy", "FormIdentifier", "Modified", "ModifiedBy")
SELECT pfc."Id", (SELECT "Id" FROM "Language" WHERE "Code" = 'fi'), pfc."Created", pfc."CreatedBy", pfc."FormIdentifier", pfc."Modified", pfc."ModifiedBy"
FROM  public."PrintableFormChannel"  pfc;

INSERT INTO  public."PrintableFormChannelIdentifier"("PrintableFormChannelId", "LocalizationId", "Created", "CreatedBy", "FormIdentifier", "Modified", "ModifiedBy")
SELECT pfc."Id", (SELECT "Id" FROM "Language" WHERE "Code" = 'sv'), pfc."Created", pfc."CreatedBy", pfc."FormIdentifier", pfc."Modified", pfc."ModifiedBy"
FROM  public."PrintableFormChannel"  pfc;