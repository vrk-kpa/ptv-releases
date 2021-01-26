 delete from "ServiceServiceChannelDigitalAuthorization"
            where 
            ("ServiceServiceChannelDigitalAuthorization"."DigitalAuthorizationId",  
            "ServiceServiceChannelDigitalAuthorization"."ServiceChannelId",
            "ServiceServiceChannelDigitalAuthorization"."ServiceId")
            in (
            select 
                sd."DigitalAuthorizationId",
                sd."ServiceChannelId",
                sd."ServiceId"
            from "ServiceServiceChannelDigitalAuthorization" AS sd
            left join "ServiceServiceChannel" AS ss On ss."ServiceId" = sd."ServiceId" AND ss."ServiceChannelId" = sd."ServiceChannelId"
            where ss."ServiceId" is null);
            
  ALTER TABLE public."ServiceServiceChannelDigitalAuthorization"
  ADD CONSTRAINT "FK_SerSerChaDesDigAut_SerChaId" FOREIGN KEY ("ServiceChannelId", "ServiceId")
  REFERENCES public."ServiceServiceChannel" ("ServiceChannelId", "ServiceId") MATCH SIMPLE
  ON UPDATE NO ACTION
  ON DELETE CASCADE;
