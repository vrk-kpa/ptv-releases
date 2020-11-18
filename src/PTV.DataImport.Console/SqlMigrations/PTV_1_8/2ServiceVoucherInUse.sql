UPDATE "ServiceVersioned"
	SET "WebPageInUse" = true
	WHERE "Id" IN (SELECT "ServiceVersionedId" FROM "ServiceWebPage");

