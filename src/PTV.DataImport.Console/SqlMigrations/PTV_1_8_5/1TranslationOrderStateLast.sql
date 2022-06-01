  UPDATE "TranslationOrderState"
	SET "Last" = true
	WHERE "Id" IN ( SELECT ts."Id" 
	FROM "TranslationOrderState" ts
    INNER JOIN (
    	SELECT Count("TranslationOrderId"), "TranslationOrderId", MAX("SendAt") as MaxDatum
        FROM "TranslationOrderState" 
        GROUP BY "TranslationOrderId"
    ) md on (md."TranslationOrderId" = ts."TranslationOrderId" AND md.MaxDatum = ts."SendAt")
    WHERE ts."Last" = false);  