INSERT INTO public."ServiceChannelAreaMunicipality"("ServiceChannelVersionedId", "MunicipalityId", "Created", "CreatedBy", "Modified", "ModifiedBy")
SELECT (SELECT slc."ServiceChannelVersionedId" FROM public."ServiceLocationChannel" slc WHERE slc."Id" = slcsa."ServiceLocationChannelId"), slcsa."MunicipalityId", slcsa."Created", slcsa."CreatedBy", slcsa."Modified", slcsa."ModifiedBy"
FROM  public."ServiceLocationChannelServiceArea"  slcsa;


UPDATE public."ServiceChannelVersioned"
SET "AreaInformationTypeId" = GetOrCreateDefaultAreaInformationTypeId('AreaType')
WHERE "Id" IN (
	SELECT slc."ServiceChannelVersionedId"
	FROM public."ServiceLocationChannel" slc
	WHERE slc."Id" IN (
		SELECT slcsa."ServiceLocationChannelId"
		FROM public."ServiceLocationChannelServiceArea"  slcsa)
	);