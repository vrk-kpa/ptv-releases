CREATE OR REPLACE FUNCTION GetDayIsSelected(int, boolean) RETURNS int LANGUAGE SQL AS  $$ select case when $2 then $1 else null end; $$;
CREATE OR REPLACE FUNCTION GetIsClosed(uuid) RETURNS boolean LANGUAGE SQL AS  $$ select "Id" = $1 from "ExceptionHoursStatusType" where "Code" = 'Closed'; $$;

--select * from GetDayIsSelected(2, false);
--select * from GetIsClosed('6737b93c-7b4d-4668-543d-08d3c60ad665');

-- normal hours
-- creates daily
INSERT INTO public."DailyOpeningTime"(
	"Created", "CreatedBy", "Modified", "ModifiedBy",
            "OpeningHourId", "IsExtra", "DayFrom", "DayTo",
	"From", "To")

   SELECT sh."Created", sh."CreatedBy", sh."Modified", sh."ModifiedBy",
	sh."Id", false, sh.day, null,
	age(to_timestamp('2016-0101 ' || coalesce( trim(sh."Opens"), '00:00'), 'YYYY-MMDD HH24:MI'), timestamp '2016-01-01'), -- sh."Opens",
	age(to_timestamp('2016-0101 ' || coalesce( trim(sh."Closes"), '24:00'), 'YYYY-MMDD HH24:MI'), timestamp '2016-01-01') --, sh."Closes"

  FROM (
	select *,
	unnest(ARRAY[
		GetDayIsSelected(0, "Monday")::int,
		GetDayIsSelected(1, "Tuesday")::int,
		GetDayIsSelected(2, "Wednesday")::int,
		GetDayIsSelected(3, "Thursday")::int,
		GetDayIsSelected(4, "Friday")::int,
		GetDayIsSelected(5, "Saturday")::int,
		GetDayIsSelected(6, "Sunday")::int]
	) AS day from public."ServiceChannelServiceHours" ) sh
	join public."ServiceHourType" sht on (sh."ServiceHourTypeId" = sht."Id")
 where sht."Code" = 'Standard' and sh.day is not null;



-- exceptional
UPDATE public."ServiceChannelServiceHours"
   SET "IsClosed"= coalesce(GetIsClosed("ExceptionHoursTypeId"), false);


INSERT INTO public."DailyOpeningTime"(
	"Created", "CreatedBy", "Modified", "ModifiedBy",
            "OpeningHourId", "IsExtra", "DayFrom", "DayTo",
	"From", "To")
 SELECT sh."Created", sh."CreatedBy", sh."Modified", sh."ModifiedBy",
	sh."Id", false, 0, 6,
	age(to_timestamp('2016-0101 ' || coalesce( trim(sh."Opens"), '00:00'), 'YYYY-MMDD HH24:MI'), timestamp '2016-01-01'),-- sh."Opens",
	age(to_timestamp('2016-0101 ' || coalesce( trim(sh."Closes"), '00:00'), 'YYYY-MMDD HH24:MI'), timestamp '2016-01-01') --, sh."Closes"

  FROM public."ServiceChannelServiceHours" sh
	join public."ServiceHourType" sht on (sh."ServiceHourTypeId" = sht."Id")
 where sht."Code" = 'Exception' and coalesce( trim("Opens"),'')!='' ;


 DROP FUNCTION GetDayIsSelected(int, boolean);
 DROP FUNCTION GetIsClosed(uuid);