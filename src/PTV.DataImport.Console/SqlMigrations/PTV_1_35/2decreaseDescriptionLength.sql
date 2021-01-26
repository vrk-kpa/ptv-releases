UPDATE public."ServiceDescription"
	SET "Description"=substring("Description" from 1 for 2500);

UPDATE public."ServiceChannelDescription"
	SET "Description"=substring("Description" from 1 for 2500);

UPDATE public."ServiceRequirement"
	SET "Requirement"=substring("Requirement" from 1 for 2500);