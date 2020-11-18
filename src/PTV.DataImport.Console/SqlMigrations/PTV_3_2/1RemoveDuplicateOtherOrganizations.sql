delete
from "OrganizationService" as os
where exists (select 1
    from "ServiceVersioned" as sv
    where sv."OrganizationId" = os."OrganizationId" and sv."Id" = os."ServiceVersionedId");
