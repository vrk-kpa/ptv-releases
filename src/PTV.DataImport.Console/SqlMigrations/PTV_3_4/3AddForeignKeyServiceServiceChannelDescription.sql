-- Remove not used ServiceServiceChannelDescription 
DELETE FROM public."ServiceServiceChannelDescription" sschd
  WHERE NOT EXISTS
  (SELECT 1
  FROM public."ServiceServiceChannel" ssch
  WHERE ssch."ServiceId" = sschd."ServiceId" AND ssch."ServiceChannelId" = sschd."ServiceChannelId" );
 
-- Create foreign key between ServiceServiceChannelDescription and ServiceServiceChannel
ALTER TABLE public."ServiceServiceChannelDescription"
  ADD CONSTRAINT "FK_ServiceServiceChannelDescription_ServiceServiceChannel_ServiceId_ServiceChannelId" FOREIGN KEY ("ServiceChannelId", "ServiceId")
  REFERENCES public."ServiceServiceChannel" ("ServiceChannelId", "ServiceId") MATCH SIMPLE
  ON UPDATE NO ACTION
  ON DELETE CASCADE;