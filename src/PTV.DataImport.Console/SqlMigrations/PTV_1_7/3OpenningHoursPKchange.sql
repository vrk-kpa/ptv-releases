UPDATE "DailyOpeningTime"
   SET "Order"=1
 WHERE "IsExtra" = true;

UPDATE "DailyOpeningTime"
   SET "Order"=0
 WHERE "IsExtra" = false;