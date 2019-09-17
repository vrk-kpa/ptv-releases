do $$
declare 

  -- organization types
  sotePublicId uuid = (select "Id" from "OrganizationType" where "Code" = 'SotePublic');
  sotePrivateId uuid = (select "Id" from "OrganizationType" where "Code" = 'SotePrivate');
  regionId uuid = (select "Id" from "OrganizationType" where "Code" = 'Region');
  regionalOrganizationId uuid = (select "Id" from "OrganizationType" where "Code" = 'RegionalOrganization');
  organizationId uuid = (select "Id" from "OrganizationType" where "Code" = 'Organization');
  municipalityId uuid = (select "Id" from "OrganizationType" where "Code" = 'Municipality');
 
  -- general description types
  prescribedByFreedomId uuid = (select "Id" from "GeneralDescriptionType" where "Code" = 'PrescribedByFreedomOfChoiceAct');
  otherPermissionId uuid = (select "Id" from "GeneralDescriptionType" where "Code" = 'OtherPermissionGrantedSote');
  businessSubregionId uuid = (select "Id" from "GeneralDescriptionType" where "Code" = 'BusinessSubregion');
  gdMunicipalityId uuid = (select "Id" from "GeneralDescriptionType" where "Code" = 'Municipality');
begin
	
	-- handle organization types
	update "OrganizationVersioned" set "TypeId" = regionalOrganizationId where "TypeId" = sotePublicId;
	update "OrganizationVersioned" set "TypeId" = organizationId where "TypeId" = sotePrivateId;
	update "OrganizationVersioned" set "TypeId" = municipalityId where "TypeId" = regionId;
	update "OrganizationType" set "IsValid" = false where "Id" in (sotePublicId, sotePrivateId, regionId);

	-- handle restriction types
	delete from "RestrictedType" where "Value" = prescribedByFreedomId;
	delete from "RestrictedType" where "Value" = otherPermissionId;

	-- handle general description types
	update "StatutoryServiceGeneralDescriptionVersioned" set "GeneralDescriptionTypeId" = businessSubregionId where  "GeneralDescriptionTypeId" = otherPermissionId;
	update "StatutoryServiceGeneralDescriptionVersioned" set "GeneralDescriptionTypeId" = gdMunicipalityId where  "GeneralDescriptionTypeId" = prescribedByFreedomId;
	delete from "GeneralDescriptionType" where "Id" = otherPermissionId;
	delete from "GeneralDescriptionType" where "Id" = prescribedByFreedomId;

	-- handle extra type Sote 
	delete from "ExtraType" where "Code" = 'Sote';

	-- handle AccessRightType
	delete from "AccessRightType" where "Code" = 'SOTEWrite';
end $$