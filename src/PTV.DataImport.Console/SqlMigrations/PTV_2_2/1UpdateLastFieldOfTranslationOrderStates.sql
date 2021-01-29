UPDATE public."TranslationOrderState"
  SET "Last" = FALSE
   WHERE "Id" IN ( SELECT ts."Id" 
   FROM public."TranslationOrderState" ts
  INNER JOIN (
      SELECT Count("TranslationOrderId"), "TranslationOrderId", MAX("SendAt") as MaxDatum
      FROM public."TranslationOrderState" 
       GROUP BY "TranslationOrderId"
  ) md on (md."TranslationOrderId" = ts."TranslationOrderId" AND md.MaxDatum = ts."SendAt")
  WHERE ts."TranslationOrderId" IN (SELECT "PreviousTranslationOrderId"
                  FROM public."TranslationOrder" WHERE 
                  "PreviousTranslationOrderId" IS NOT NULL)        
        AND ts."Last" = TRUE); 
        
        
 UPDATE public."TranslationOrderState"
  SET "Last" = TRUE
   WHERE "Id" IN ( SELECT ts."Id" 
   FROM public."TranslationOrderState" ts
  INNER JOIN (
      SELECT Count("TranslationOrderId"), "TranslationOrderId", MAX("SendAt") as MaxDatum
      FROM public."TranslationOrderState" 
       GROUP BY "TranslationOrderId"
  ) md on (md."TranslationOrderId" = ts."TranslationOrderId" AND md.MaxDatum = ts."SendAt")
  WHERE ts."TranslationOrderId" NOT IN (SELECT "PreviousTranslationOrderId"
                  FROM public."TranslationOrder" WHERE 
                  "PreviousTranslationOrderId" IS NOT NULL)        
        AND ts."Last" = FALSE);


