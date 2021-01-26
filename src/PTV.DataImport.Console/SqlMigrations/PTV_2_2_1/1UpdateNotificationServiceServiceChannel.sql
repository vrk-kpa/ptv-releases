Update "NotificationServiceServiceChannel" nnssc
SET "OrganizationId" = (select "OrganizationId" from "ServiceVersioned" WHERE nnssc."ServiceId" = "UnificRootId" order by "Modified" desc limit 1)