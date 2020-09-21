UPDATE public."ServiceHours"
   SET "OpeningHoursFrom"= date_trunc('day', "OpeningHoursFrom" + interval '3 hour')
   -- select * from public."ServiceHours"
 WHERE 
 extract(hour from "OpeningHoursFrom") > 20 and extract(minute from "OpeningHoursFrom") = 0
 and "OpeningHoursTo" is not null;
 

UPDATE public."ServiceHours"
   SET "OpeningHoursTo"= date_trunc('day', "OpeningHoursTo" + interval '3 hour')
   -- select * from public."ServiceHours"
 WHERE 
  extract(hour from "OpeningHoursTo") > 20 and extract(minute from "OpeningHoursTo") = 0;