INSERT INTO public."ServiceChannelAttachment"(
            "ServiceChannelId", "AttachmentId", "Created", "CreatedBy", "Modified", "ModifiedBy")
    SELECT ec."ServiceChannelId", "AttachmentId", eca."Created", eca."CreatedBy", eca."Modified", eca."ModifiedBy"
  FROM public."ElectronicChannelAttachment" eca join public."ElectronicChannel" ec on (eca."ElectronicChannelId" = ec."Id");

INSERT INTO public."ServiceChannelAttachment"(
            "ServiceChannelId", "AttachmentId", "Created", "CreatedBy", "Modified", "ModifiedBy")
    SELECT pfc."ServiceChannelId", "AttachmentId", pfca."Created", pfca."CreatedBy", pfca."Modified", pfca."ModifiedBy"
  FROM public."PrintableFormChannelAttachment" pfca join public."PrintableFormChannel" pfc on (pfca."PrintableFormChannelId" = pfc."Id");

INSERT INTO public."ServiceChannelAttachment"(
            "ServiceChannelId", "AttachmentId", "Created", "CreatedBy", "Modified", "ModifiedBy")
    SELECT pfc."ServiceChannelId", "AttachmentId", pfca."Created", pfca."CreatedBy", pfca."Modified", pfca."ModifiedBy"
  FROM public."WebpageChannelAttachment" pfca join public."WebpageChannel" pfc on (pfca."WebpageChannelId" = pfc."Id");

