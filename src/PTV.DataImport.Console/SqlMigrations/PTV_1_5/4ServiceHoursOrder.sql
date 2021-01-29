CREATE OR REPLACE FUNCTION updateServiceHourOrderNumbers() RETURNS void AS
$BODY$
DECLARE
    sc_row RECORD;
    sc_sh_row RECORD;
    sc_sh_number integer DEFAULT 1;
BEGIN
  -- Get service channels that have service hours
  FOR sc_row IN 
	(SELECT distinct("ServiceChannelServiceHours"."ServiceChannelVersionedId") FROM "ServiceChannelServiceHours")
  LOOP
    sc_sh_number := 1;
    -- Get service hours for a channel
    FOR sc_sh_row IN
	(
	-- Standard (valid for now)
	(SELECT "scsh".*, "sht"."Code", '1' "SortOrder" FROM "ServiceChannelServiceHours" AS "scsh" 
		INNER JOIN "ServiceHourType" AS "sht" ON ("sht"."Id" = "scsh"."ServiceHourTypeId")
		AND "sht"."Code" = 'Standard'
 		AND "scsh"."OpeningHoursTo" IS NULL
		AND "scsh"."ServiceChannelVersionedId" = sc_row."ServiceChannelVersionedId"
		AND "scsh"."OrderNumber" IS NULL
		)
	UNION
	-- Standard (valid for time period)
		(SELECT "scsh".*, "sht"."Code", '2' FROM "ServiceChannelServiceHours" AS "scsh" 
		INNER JOIN "ServiceHourType" AS "sht" ON ("sht"."Id" = "scsh"."ServiceHourTypeId")
		AND "sht"."Code" = 'Standard'
 		AND "scsh"."OpeningHoursTo" IS NOT NULL
		AND "scsh"."ServiceChannelVersionedId" = sc_row."ServiceChannelVersionedId"
		AND "scsh"."OrderNumber" IS NULL
		)
	-- Special (valid for now)
	UNION
		(SELECT "scsh".*, "sht"."Code", '3' "SortOrder" FROM "ServiceChannelServiceHours" AS "scsh" 
		INNER JOIN "ServiceHourType" AS "sht" ON ("sht"."Id" = "scsh"."ServiceHourTypeId")
		AND "sht"."Code" = 'Special'
 		AND "scsh"."OpeningHoursTo" IS NULL
		AND "scsh"."ServiceChannelVersionedId" = sc_row."ServiceChannelVersionedId"
		AND "scsh"."OrderNumber" IS NULL
		)
	-- Special (valid for time period)
	UNION
		(SELECT "scsh".*, "sht"."Code", '4' "SortOrder" FROM "ServiceChannelServiceHours" AS "scsh" 
		INNER JOIN "ServiceHourType" AS "sht" ON ("sht"."Id" = "scsh"."ServiceHourTypeId")
		AND "sht"."Code" = 'Special'
 		AND "scsh"."OpeningHoursTo" IS NOT NULL
		AND "scsh"."ServiceChannelVersionedId" = sc_row."ServiceChannelVersionedId"
		AND "scsh"."OrderNumber" IS NULL
		)
	-- Exception (in opening hours from order)
	UNION
		(SELECT "scsh".*, "sht"."Code", '5' "SortOrder" FROM "ServiceChannelServiceHours" AS "scsh" 
		INNER JOIN "ServiceHourType" AS "sht" ON ("sht"."Id" = "scsh"."ServiceHourTypeId")
		AND "sht"."Code" = 'Exception'
		AND "scsh"."ServiceChannelVersionedId" = sc_row."ServiceChannelVersionedId"
		AND "scsh"."OrderNumber" IS NULL
		)
	-- Order by Custom ordering and Opening hours from
	ORDER BY "SortOrder", "OpeningHoursFrom")
	LOOP
		UPDATE "ServiceChannelServiceHours"
		SET "OrderNumber" = sc_sh_number
		WHERE "ServiceChannelServiceHours"."Id" = sc_sh_row."Id";
		
		sc_sh_number := sc_sh_number + 1;
	END LOOP;
	
  END LOOP;
  RETURN;
END
$BODY$
LANGUAGE plpgsql;

SELECT updateServiceHourOrderNumbers();

DROP FUNCTION updateServiceHourOrderNumbers();