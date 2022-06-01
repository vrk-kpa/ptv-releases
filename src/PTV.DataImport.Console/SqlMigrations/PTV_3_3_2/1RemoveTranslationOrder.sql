UPDATE public."TranslationOrderState"
SET "TranslationStateId"= (SELECT "Id"
	                       FROM public."TranslationStateType"
	                       WHERE "Code" = 'Canceled')
WHERE "Id" = (SELECT "Id"
	          FROM public."TranslationOrderState"
	          WHERE "TranslationOrderId"='39198a00-6db8-43ab-9157-eba2027f6177'
              ORDER BY "SendAt" DESC LIMIT 1);