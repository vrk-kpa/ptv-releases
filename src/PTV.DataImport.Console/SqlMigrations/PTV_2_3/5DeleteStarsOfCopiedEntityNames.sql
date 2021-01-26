UPDATE public."ServiceName"
 SET "Name" = regexp_replace("Name",'\[\*\]\s*','','g')
 WHERE "Name" LIKE '%[*]%' AND
	"ServiceVersionedId" IN (
		SELECT sv."Id" 
		FROM public."ServiceVersioned" AS "sv"
		WHERE sv."OriginalId" IS NOT NULL); 

UPDATE public."ServiceChannelName"
 SET "Name" = regexp_replace("Name",'\[\*\]\s*','','g')
 WHERE "Name" LIKE '%[*]%' AND
       "ServiceChannelVersionedId" IN (
	     SELECT schv."Id" 
	     FROM public."ServiceChannelVersioned" AS "schv"
	     WHERE schv."OriginalId" IS NOT NULL); 

UPDATE public."ServiceCollectionName"
 SET "Name" = regexp_replace("Name",'\[\*\]\s*','','g')
 WHERE "Name" LIKE '%[*]%' AND
       "ServiceCollectionVersionedId" IN (
	     SELECT scv."Id" 
	     FROM public."ServiceCollectionVersioned" AS "scv"
	     WHERE scv."OriginalId" IS NOT NULL); 