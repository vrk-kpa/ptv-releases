INSERT INTO public."ServiceAreaMunicipality"("ServiceVersionedId", "MunicipalityId", "Created", "CreatedBy", "Modified", "ModifiedBy")
SELECT sm."ServiceVersionedId", sm."MunicipalityId", sm."Created", sm."CreatedBy", sm."Modified", sm."ModifiedBy"
FROM  public."ServiceMunicipality"  sm;

UPDATE public."ServiceVersioned" SET "AreaInformationTypeId" = GetOrCreateDefaultAreaInformationTypeId('AreaType') WHERE "Id" IN (SELECT sm."ServiceVersionedId" FROM public."ServiceMunicipality"  sm);