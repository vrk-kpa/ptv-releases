UPDATE public."ServiceName"
	SET "Name"=substring("Name" from 1 for 100);
	
UPDATE public."Attachment"
SET "Name"=substring("Name" from 1 for 100);
	
UPDATE public."Email"
SET "Value"=substring("Value" from 1 for 100);	
	
UPDATE public."Email"
SET "Description"=substring("Description" from 1 for 100);	

UPDATE public."OrganizationName"
SET "Name"=substring("Name" from 1 for 100);
	
UPDATE public."Organization"
SET "Oid"=substring("Oid" from 1 for 100);

UPDATE public."ServiceHoursAdditionalInformation"
SET "Text"=substring("Text" from 1 for 100);

UPDATE public."ServiceChannelName"
SET "Name"=substring("Name" from 1 for 100);

UPDATE public."StreetName"
SET "Text"=substring("Text" from 1 for 100);

UPDATE public."Phone"
	SET "Number"=substring("Number" from 1 for 20);

UPDATE public."AddressAdditionalInformation"
	SET "Text"=substring("Text" from 1 for 150);

UPDATE public."Attachment"
	SET "Description"=substring("Description" from 1 for 150);
	
UPDATE public."Business"
SET "Code"=substring("Code" from 1 for 9);
	