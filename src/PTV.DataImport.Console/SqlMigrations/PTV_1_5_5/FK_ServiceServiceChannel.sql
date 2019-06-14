ALTER TABLE public."ServiceServiceChannel" DROP CONSTRAINT IF EXISTS "FK_ServiceServiceChannel_ServiceChannel_ServiceChannelId";
DELETE FROM public."ServiceServiceChannel" WHERE "ServiceChannelId" = '00000000-0000-0000-0000-000000000000';
ALTER TABLE public."ServiceServiceChannel" ADD
  CONSTRAINT "FK_ServiceServiceChannel_ServiceChannel_ServiceChannelId" FOREIGN KEY ("ServiceChannelId")
      REFERENCES public."ServiceChannel" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION;
