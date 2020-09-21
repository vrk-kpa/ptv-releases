DO $$
DECLARE newGuid uuid;
BEGIN
	SELECT GenerateGuid() INTO newGuid;
INSERT INTO public."RestrictedType"(
	"Id", "TypeName", "Value")
	VALUES (newGuid, 'GeneralDescriptionType', (SELECT "Id" FROM "GeneralDescriptionType" where "Code" = 'PrescribedByFreedomOfChoiceAct'));
INSERT INTO public."RestrictionFilter"(
	"Id", "EntityType", "FilterName", "ColumnName", "RestrictedTypeId", "BlockOtherTypes", "FilterType")
	VALUES ((SELECT GenerateGuid()), 'StatutoryServiceGeneralDescriptionVersioned', 'GeneralDescriptionFocTypeOrganizationSoteRelation', 'GeneralDescriptionTypeId', newGuid, false, 0);
END $$