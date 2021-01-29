update "UserOrganization"
set "OrganizationId" = (
 select "Id"
 from "OrganizationVersioned"
 where "OrganizationVersioned"."Oid" = "UserOrganization"."RelationId"
  and ("UserOrganization"."RelationId" is not null and "UserOrganization"."RelationId" != '')
)
where "OrganizationId" is null and
 exists(
  select "Oid"
  from "OrganizationVersioned"
  where "OrganizationVersioned"."Oid" = "UserOrganization"."RelationId"
   and ("OrganizationVersioned"."Oid" is not null and "OrganizationVersioned"."Oid" != '')
   and ("UserOrganization"."RelationId" is not null and "UserOrganization"."RelationId" != '')
  group by "Oid" having count(*) = 1
 );