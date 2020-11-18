DO $$

DECLARE
	counter INTEGER := 7;
	rec RECORD;
BEGIN
	FOR rec IN SELECT *
                          FROM public."Language" lang              
        LOOP
		UPDATE public."Language"
			SET "OrderNumber"=1000, "IsForData"=false;
      
	END LOOP;
	
	UPDATE public."Language"
	   SET "IsForData"=true, "OrderNumber"=1
	 WHERE "Code" = 'fi';

	UPDATE public."Language"
	   SET "IsForData"=true, "OrderNumber"=2
	 WHERE "Code" = 'sv';

	UPDATE public."Language"
	   SET "IsForData"=true, "OrderNumber"=3
	 WHERE "Code" = 'en';

	UPDATE public."Language"
	   SET "IsForData"=true, "OrderNumber"=4
	 WHERE "Code" = 'se';

	UPDATE public."Language"
	   SET "IsForData"=true, "OrderNumber"=5
	 WHERE "Code" = 'smn';

	UPDATE public."Language"
	   SET "IsForData"=true, "OrderNumber"=6
	 WHERE "Code" = 'sms';

	FOR rec IN SELECT *
                          FROM public."Language" lang WHERE "OrderNumber" > 6            
        LOOP
		UPDATE public."Language"
			SET "OrderNumber"=counter
		WHERE "Id" = rec."Id";

		counter := counter + 1;
      
	END LOOP;
END $$;