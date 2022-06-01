------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- REPLACE MATERIALIZED VIEW  ---------------------------------------
------------------------------------------------------------------------------------------------------------------------

DROP MATERIALIZED VIEW IF EXISTS geo."mv_layer_serviceChannel_grouped";

CREATE MATERIALIZED VIEW IF NOT EXISTS geo."mv_layer_serviceChannel_grouped"
    TABLESPACE pg_default
AS
SELECT row_number() OVER () AS id,
       "mv_layer_serviceChannel"."rootId",
       "mv_layer_serviceChannel".location,
       string_agg("mv_layer_serviceChannel"."versionedId"::character varying::text, ';'::text) AS "versionedId",
       string_agg(distinct "mv_layer_serviceChannel"."organizationId"::character varying::text, ';'::text) AS "organizationId",
       string_agg(distinct "mv_layer_serviceChannel"."addressId"::character varying::text, ';'::text) AS "addressId",
       string_agg(distinct "mv_layer_serviceChannel"."addressType"::character varying::text, ';'::text) AS "addressType",
       string_agg("mv_layer_serviceChannel"."currentlyOpen"::character varying::text, ';'::text) AS "currentlyOpen",
       string_agg(distinct "mv_layer_serviceChannel"."channelNameFi"::character varying::text, ';'::text) AS "channelNameFi",
       string_agg(distinct "mv_layer_serviceChannel"."channelNameSv"::character varying::text, ';'::text) AS "channelNameSv",
       string_agg(distinct "mv_layer_serviceChannel"."channelNameEn"::character varying::text, ';'::text) AS "channelNameEn",
       string_agg(distinct "mv_layer_serviceChannel"."organizationNameFi"::character varying::text, ';'::text) AS "organizationNameFi",
       string_agg(distinct "mv_layer_serviceChannel"."organizationNameSv"::character varying::text, ';'::text) AS "organizationNameSv",
       string_agg(distinct "mv_layer_serviceChannel"."organizationNameEn"::character varying::text, ';'::text) AS "organizationNameEn",
       string_agg(distinct "mv_layer_serviceChannel"."addressFi"::character varying::text, ';'::text) AS "addressFi",
       string_agg(distinct "mv_layer_serviceChannel"."addressSv"::character varying::text, ';'::text) AS "addressSv",
       string_agg(distinct "mv_layer_serviceChannel"."addressEn"::character varying::text, ';'::text) AS "addressEn",
       string_agg("mv_layer_serviceChannel"."addressAdditionalInformationFi"::character varying::text, ';'::text) AS "addressAdditionalInformationFi",
       string_agg("mv_layer_serviceChannel"."addressAdditionalInformationSv"::character varying::text, ';'::text) AS "addressAdditionalInformationSv",
       string_agg("mv_layer_serviceChannel"."addressAdditionalInformationEn"::character varying::text, ';'::text) AS "addressAdditionalInformationEn",
       string_agg(distinct "mv_layer_serviceChannel"."municipalityCode"::character varying::text, ';'::text) AS "municipalityCode",
       string_agg(distinct "mv_layer_serviceChannel"."municipalityNameFi"::character varying::text, ';'::text) AS "municipalityNameFi",
       string_agg(distinct "mv_layer_serviceChannel"."municipalityNameSv"::character varying::text, ';'::text) AS "municipalityNameSv",
       string_agg(distinct "mv_layer_serviceChannel"."municipalityNameEn"::character varying::text, ';'::text) AS "municipalityNameEn",
       string_agg(distinct "mv_layer_serviceChannel"."phoneNumbersFi"::character varying::text, ';'::text) AS "phoneNumbersFi",
       string_agg(distinct "mv_layer_serviceChannel"."phoneNumbersSv"::character varying::text, ';'::text) AS "phoneNumbersSv",
       string_agg(distinct "mv_layer_serviceChannel"."phoneNumbersEn"::character varying::text, ';'::text) AS "phoneNumbersEn",
       string_agg(distinct "mv_layer_serviceChannel"."normalOpeningHoursFi"::character varying::text, ';'::text) AS "normalOpeningHoursFi",
       string_agg(distinct "mv_layer_serviceChannel"."normalOpeningHoursSv"::character varying::text, ';'::text) AS "normalOpeningHoursSv",
       string_agg(distinct "mv_layer_serviceChannel"."normalOpeningHoursEn"::character varying::text, ';'::text) AS "normalOpeningHoursEn",
       string_agg("mv_layer_serviceChannel"."exceptionalOpeningHoursFi"::character varying::text, ';'::text) AS "exceptionalOpeningHoursFi",
       string_agg("mv_layer_serviceChannel"."exceptionalOpeningHoursSv"::character varying::text, ';'::text) AS "exceptionalOpeningHoursSv",
       string_agg("mv_layer_serviceChannel"."exceptionalOpeningHoursEn"::character varying::text, ';'::text) AS "exceptionalOpeningHoursEn",
       string_agg("mv_layer_serviceChannel"."bankholidayOpeningHoursFi"::character varying::text, ';'::text) AS "bankholidayOpeningHoursFi",
       string_agg("mv_layer_serviceChannel"."bankholidayOpeningHoursSv"::character varying::text, ';'::text) AS "bankholidayOpeningHoursSv",
       string_agg("mv_layer_serviceChannel"."bankholidayOpeningHoursEn"::character varying::text, ';'::text) AS "bankholidayOpeningHoursEn",
       string_agg("mv_layer_serviceChannel"."overmidnightOpeningHoursFi"::character varying::text, ';'::text) AS "overmidnightOpeningHoursFi",
       string_agg("mv_layer_serviceChannel"."overmidnightOpeningHoursSv"::character varying::text, ';'::text) AS "overmidnightOpeningHoursSv",
       string_agg("mv_layer_serviceChannel"."overmidnightOpeningHoursEn"::character varying::text, ';'::text) AS "overmidnightOpeningHoursEn",
       string_agg("mv_layer_serviceChannel"."serviceId"::character varying::text, ';'::text) AS "serviceId",
       string_agg("mv_layer_serviceChannel"."serviceNameFi"::character varying::text, ';'::text) AS "serviceNameFi",
       string_agg("mv_layer_serviceChannel"."serviceNameSv"::character varying::text, ';'::text) AS "serviceNameSv",
       string_agg("mv_layer_serviceChannel"."serviceNameEn"::character varying::text, ';'::text) AS "serviceNameEn",
       string_agg(distinct "mv_layer_serviceChannel"."coordinateState"::character varying::text, ';'::text) AS "coordinateState",
       string_agg(distinct "mv_layer_serviceChannel"."coordinateType"::character varying::text, ';'::text) AS "coordinateType"
FROM geo."mv_layer_serviceChannel"
GROUP BY "mv_layer_serviceChannel"."rootId", "mv_layer_serviceChannel".location
WITH DATA;

CREATE INDEX "mv_layer_serviceChannel_grouped_addressId_idx"
    ON geo."mv_layer_serviceChannel_grouped" USING btree
        ("addressId")
    TABLESPACE pg_default;
CREATE UNIQUE INDEX "mv_layer_serviceChannel_grouped_id_idx"
    ON geo."mv_layer_serviceChannel_grouped" USING btree
        (id)
    TABLESPACE pg_default;
CREATE INDEX "mv_layer_serviceChannel_grouped_location_idx"
    ON geo."mv_layer_serviceChannel_grouped" USING gist
    (location)
    TABLESPACE pg_default;
CREATE INDEX "mv_layer_serviceChannel_grouped_rootId_idx"
    ON geo."mv_layer_serviceChannel_grouped" USING btree
        ("rootId")
    TABLESPACE pg_default;
CREATE INDEX "mv_layer_serviceChannel_grouped_versionedId_idx"
    ON geo."mv_layer_serviceChannel_grouped" USING btree
        ("versionedId")
    TABLESPACE pg_default;

grant select on geo."mv_layer_serviceChannel_grouped" to geoserver_user;