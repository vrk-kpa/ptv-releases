CREATE TABLE IF NOT EXISTS public."VoucherType"
(
    "Id" uuid NOT NULL,
    "Created" timestamp without time zone NOT NULL,
    "CreatedBy" text COLLATE pg_catalog."default",
    "Modified" timestamp without time zone NOT NULL,
    "ModifiedBy" text COLLATE pg_catalog."default",
    "LastOperationIdentifier" uuid NOT NULL,
    "LastOperationTimeStamp" timestamp without time zone NOT NULL,
    "LastOperationType" integer NOT NULL,
    "Code" text COLLATE pg_catalog."default",
    "OrderNumber" integer,
    CONSTRAINT "PK_VoucherType" PRIMARY KEY ("Id")
);

INSERT INTO public."VoucherType"(
	"Id", "Created", "CreatedBy", "Modified", "ModifiedBy", "LastOperationIdentifier", "LastOperationTimeStamp", "LastOperationType", "Code", "OrderNumber")
	VALUES (uuid_generate_v4(), now() , 'PTVapp', now(), 'PTVapp', uuid_generate_v4(), now(), 12,'NotUsed', 0);

INSERT INTO public."VoucherType"(
	"Id", "Created", "CreatedBy", "Modified", "ModifiedBy", "LastOperationIdentifier", "LastOperationTimeStamp", "LastOperationType", "Code", "OrderNumber")
	VALUES (uuid_generate_v4(), now() , 'PTVapp', now(), 'PTVapp', uuid_generate_v4(), now(), 12,'NoUrl', 1);

INSERT INTO public."VoucherType"(
	"Id", "Created", "CreatedBy", "Modified", "ModifiedBy", "LastOperationIdentifier", "LastOperationTimeStamp", "LastOperationType", "Code", "OrderNumber")
	VALUES (uuid_generate_v4(), now() , 'PTVapp', now(), 'PTVapp', uuid_generate_v4(), now(), 12,'Url', 2);

