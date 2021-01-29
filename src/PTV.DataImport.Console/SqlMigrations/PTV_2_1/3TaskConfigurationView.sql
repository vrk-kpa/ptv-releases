DROP VIEW IF EXISTS "VTasksConfiguration";

CREATE OR REPLACE VIEW "VTasksConfiguration"
AS
SELECT
	"Id",
    "Code",
    "PublishingStatusId",
    "Entity", (
        SELECT
            CURRENT_TIMESTAMP AT TIME ZONE 'UTC') - ("Interval")::interval AS "Interval"
    FROM
        "TasksConfiguration";