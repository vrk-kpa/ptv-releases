with resty as 
(
	insert into "RestrictedType" ("Id", "TypeName", "Value") 
	values(uuid_generate_v4(), 'OrganizationType', (select "Id" from "OrganizationType" where "Code" = 'SotePublic'))
	returning *
)
insert into "RestrictionFilter" ("Id", "EntityType", "FilterName", "ColumnName", "RestrictedTypeId", "BlockOtherTypes", "FilterType")
values(uuid_generate_v4(), 'OrganizationVersioned', 'SotePublicOrganizationType', 'OrganizationTypeId', (select "Id" from resty), false, 2)
;

with resty as 
(
	insert into "RestrictedType" ("Id", "TypeName", "Value")
	values(uuid_generate_v4(), 'OrganizationType', (select "Id" from "OrganizationType" where "Code" = 'SotePrivate'))
	returning *
)
insert into "RestrictionFilter" ("Id", "EntityType", "FilterName", "ColumnName", "RestrictedTypeId", "BlockOtherTypes", "FilterType")
values(uuid_generate_v4(), 'OrganizationVersioned', 'SotePrivateOrganizationType', 'OrganizationTypeId', (select "Id" from resty), false, 2)
;