CREATE OR REPLACE
	FUNCTION GetServiceHourTypeIdByName(text)
	RETURNS uuid
	LANGUAGE SQL AS
	$$
		SELECT "Id"
		FROM "ServiceHourType"
		WHERE "Code" = $1;
	$$;

UPDATE "ServiceHours"
SET "IsNonStop" = TRUE
WHERE
	"ServiceHours"."ServiceHourTypeId" = GetServiceHourTypeIdByName('Standard') AND
	NOT EXISTS (
		SELECT "Id"
		FROM "DailyOpeningTime"
		WHERE "DailyOpeningTime"."OpeningHourId" = "ServiceHours"."Id"
	);

DROP FUNCTION IF EXISTS GetServiceHourTypeIdByName(text);
