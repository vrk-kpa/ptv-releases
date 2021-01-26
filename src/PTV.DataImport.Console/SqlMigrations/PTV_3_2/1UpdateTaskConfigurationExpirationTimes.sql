-- Update expiration time to 18 months
UPDATE public."TasksConfiguration"
SET "Interval"= '18 months'
WHERE "Code" like '%ExpirationTime';

-- Update lastWarning time to 15 months 
UPDATE public."TasksConfiguration"
SET "Interval"= '15 months'
WHERE "Code" like '%LastWarningTime';

-- Delete unused firstWarningsTime and MiddleWarningTime
DELETE FROM public."TasksConfiguration"
WHERE "Code" like '%FirstWarningTime' OR "Code" like '%MiddleWarningTime';

-- Delete unused entity
DELETE FROM public."TasksConfiguration"
WHERE "Entity" = 'Organization';
	