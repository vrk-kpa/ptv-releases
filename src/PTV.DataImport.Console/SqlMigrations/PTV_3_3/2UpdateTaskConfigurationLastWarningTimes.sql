-- Update last warning time for draft to 0 months
UPDATE public."TasksConfiguration"
SET "Interval"= '0 months'
WHERE "Code" like 'Draft%LastWarningTime';
