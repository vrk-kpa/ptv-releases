DROP VIEW IF EXISTS "VTasksConfiguration";

CREATE OR REPLACE VIEW "VTasksConfiguration"
AS
SELECT
    "Id",
    "Code",
    "PublishingStatusId",
    "Entity", 
    (SELECT CURRENT_TIMESTAMP AT TIME ZONE 'UTC') - ("Interval")::INTERVAL AS "Interval",
    CASE 
        WHEN "Interval" ~ 'month' THEN REGEXP_REPLACE("Interval", 'month.*', '')::NUMERIC
        WHEN "Interval" ~ 'year' THEN REGEXP_REPLACE("Interval", 'year.*', '')::NUMERIC * 12
    END AS "Months"
FROM
    "TasksConfiguration";