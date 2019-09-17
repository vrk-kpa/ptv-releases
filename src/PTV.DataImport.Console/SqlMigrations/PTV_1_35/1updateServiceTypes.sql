
-- update service type name ENGLISH
UPDATE public."ServiceTypeName"
	SET "Name"='Permission and Obligation' where "Name"='Permission';

-- update service type name FINISH
UPDATE public."ServiceTypeName"
	SET "Name"='Luvat ja Velvoitteet' where "Name"='Lupa';

-- delete service type 'Notice' and 'Registration' from "ServiceTypeName"
DELETE from public."ServiceTypeName" where "TypeId" in (Select "Id" from public."ServiceType" where "Code" in ('Notice','Registration'));

-- update service type id in "Service" table 'Notice' -> Permission
update public."Service" 
	SET "TypeId" = (Select "Id" from public."ServiceType" where "Code" = 'Permission') where "TypeId" = (Select "Id" from public."ServiceType" where "Code" = 'Notice');

-- update service type id in "Service" table 'Registration' -> Permission
update public."Service" 
	SET "TypeId" = (Select "Id" from public."ServiceType" where "Code" = 'Permission') where "TypeId" = (Select "Id" from public."ServiceType" where "Code" = 'Registration');

-- update service type id in "StatutoryServiceGeneralDescription" table 'Notice' -> Permission
update public."StatutoryServiceGeneralDescription" 
	SET "TypeId" = (Select "Id" from public."ServiceType" where "Code" = 'Permission') where "TypeId" = (Select "Id" from public."ServiceType" where "Code" = 'Notice');

-- update service type id in "StatutoryServiceGeneralDescription" table 'Registration' -> Permission
update public."StatutoryServiceGeneralDescription" 
	SET "TypeId" = (Select "Id" from public."ServiceType" where "Code" = 'Permission') where "TypeId" = (Select "Id" from public."ServiceType" where "Code" = 'Registration');

-- delete service type 'Notice' and 'Registration' from "ServiceType"
DELETE from public."ServiceType" where "Code" in ('Notice','Registration');

-- update service type code 
UPDATE public."ServiceType"
	SET "Code"='PermissionAndObligation' where "Code"='Permission';

-- Update ServiceRequirements with TasksAdditionalInfo

update public."ServiceRequirement" sr
	SET "Requirement" = "Requirement" || ' ' || sd."Description" 
	FROM public."ServiceDescription" sd
	WHERE sr."ServiceId" = sd."ServiceId" AND 
		sr."LocalizationId" = sd."LocalizationId" AND 
		sd."TypeId" =  (Select "Id" from public."DescriptionType" where "Code" = 'TasksAdditionalInfo');

-- Delete TasksAdditionalInfo from 'ServiceDescription'
DELETE from public."ServiceDescription" where "TypeId" in (Select "Id" from public."DescriptionType" where "Code" = 'TasksAdditionalInfo');

-- Delete TasksAdditionalInfo from "DescriptionTypeName"
DELETE from public."DescriptionTypeName" where "TypeId" in (Select "Id" from public."DescriptionType" where "Code" = 'TasksAdditionalInfo');

-- Delete TasksAdditionalInfo from 'DescriptionType'
DELETE from public."DescriptionType" where "Code"  = 'TasksAdditionalInfo';