ALTER TABLE public."ServiceVersioned" DROP CONSTRAINT "FK_ServiceVersioned_ServiceVersioned_OriginalId";
ALTER TABLE public."ServiceVersioned" ADD CONSTRAINT "FK_ServiceVersioned_ServiceVersioned_OriginalId" FOREIGN KEY ("OriginalId")
      REFERENCES public."ServiceVersioned" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE SET NULL;

ALTER TABLE public."ServiceChannelVersioned" DROP CONSTRAINT "FK_ServiceChannelVersioned_ServiceChannelVersioned_OriginalId";
ALTER TABLE public."ServiceChannelVersioned"
  ADD CONSTRAINT "FK_ServiceChannelVersioned_ServiceChannelVersioned_OriginalId" FOREIGN KEY ("OriginalId")
      REFERENCES public."ServiceChannelVersioned" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE SET NULL;

ALTER TABLE public."ServiceCollectionVersioned" DROP CONSTRAINT "FK_SerColVer_ServiceCollectionVersioned_OriginalId";
ALTER TABLE public."ServiceCollectionVersioned"
  ADD CONSTRAINT "FK_SerColVer_ServiceCollectionVersioned_OriginalId" FOREIGN KEY ("OriginalId")
      REFERENCES public."ServiceCollectionVersioned" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE SET NULL;
      
ALTER TABLE public."Versioning" DROP CONSTRAINT "FK_Versioning_Versioning_PreviousVersionId";
ALTER TABLE public."Versioning"
  ADD CONSTRAINT "FK_Versioning_Versioning_PreviousVersionId" FOREIGN KEY ("PreviousVersionId")
      REFERENCES public."Versioning" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE SET NULL;