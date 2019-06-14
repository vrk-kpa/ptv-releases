CREATE OR REPLACE FUNCTION GetPhoneTypeId(text) RETURNS uuid LANGUAGE SQL AS  $$ SELECT "Id" FROM "PhoneNumberType" WHERE "Code" = $1; $$;
CREATE OR REPLACE FUNCTION GetChargeTypeId(text) RETURNS uuid LANGUAGE SQL AS  $$ SELECT "Id" FROM "ServiceChargeType" WHERE "Code" = $1; $$;
CREATE OR REPLACE FUNCTION GetChargeTypeId(uuid[]) RETURNS uuid LANGUAGE SQL AS  $$ SELECT "Id" FROM "ServiceChargeType" WHERE "Id" = ANY ($1) order by "OrderNumber" limit 1; $$;

-- service location
-- creates phones
INSERT INTO public."Phone"(
            "Id", "Created", "CreatedBy", "Modified", "ModifiedBy", "LocalizationId",
            "AdditionalInformation", "ChargeDescription", "Number", "PrefixNumber",
	    "ServiceChargeTypeId", "TypeId", "Visible")
  SELECT GenerateGuidByText("Id", 'phone') Id, slc."Created", slc."CreatedBy", slc."Modified", slc."ModifiedBy", lang,
	null additionalInfo, pcd."Description", slc."Phone", null,
	coalesce( cht."ServiceChargeTypeId", chargeType), phoneType, true
  FROM 	GetPhoneTypeId('Phone') phoneType ,
	GetChargeTypeId('Charged') chargeType,
	public."ServiceLocationChannel" slc
	join GetLanguageId('fi') lang on ( 1 = 1)
  left outer join  public."ServiceLocationChannelPhoneChargeDescription" pcd on (slc."Id" = pcd."ServiceLocationChannelId" AND pcd."LocalizationId" = lang )
  left outer join ( select "ServiceLocationChannelId", GetChargeTypeId(array_agg("ServiceChargeTypeId")) "ServiceChargeTypeId" from public."ServiceLocationChannelServiceChargeType" group by "ServiceLocationChannelId") cht
	on (cht."ServiceLocationChannelId" = slc."Id")
  where coalesce( trim("Phone"),'')!='' ;

-- creates channel phones connection
INSERT INTO public."ServiceChannelPhone"(
            "Created", "CreatedBy", "Modified", "ModifiedBy", "ServiceChannelId", "PhoneId", "TypeId")
    SELECT "Created", "CreatedBy", "Modified", "ModifiedBy", "ServiceChannelId", GenerateGuidByText("Id", 'phone') PhoneId, phoneType
  FROM public."ServiceLocationChannel" , GetPhoneTypeId('Phone') phoneType where coalesce( trim("Phone"),'')!='';




-- creates faxes
INSERT INTO public."Phone"(
            "Id", "Created", "CreatedBy", "Modified", "ModifiedBy", "LocalizationId",
            "AdditionalInformation", "ChargeDescription", "Number", "PrefixNumber", "ServiceChargeTypeId", "TypeId", "Visible")
  SELECT GenerateGuidByText("Id", 'fax') Id, "Created", "CreatedBy", "Modified", "ModifiedBy", lang,
	null additionalInfo, null chargeDescription, "Fax", null, chargeType, phoneType, true
  FROM public."ServiceLocationChannel",
	GetLanguageId('fi') lang,
	GetPhoneTypeId('Fax') phoneType,
	GetChargeTypeId('Charged') chargeType
  where coalesce( trim("Fax"),'')!='';

-- creates channel phones connection
INSERT INTO public."ServiceChannelPhone"(
            "Created", "CreatedBy", "Modified", "ModifiedBy", "ServiceChannelId", "PhoneId", "TypeId")
    SELECT "Created", "CreatedBy", "Modified", "ModifiedBy", "ServiceChannelId", GenerateGuidByText("Id", 'fax') PhoneId, phoneType
  FROM public."ServiceLocationChannel", GetPhoneTypeId('Fax') phoneType where coalesce( trim("Fax"),'')!='';





-- Channel support support
-- creates phones
INSERT INTO public."Phone"(
            "Id", "Created", "CreatedBy", "Modified", "ModifiedBy", "LocalizationId",
	"AdditionalInformation", "ChargeDescription", "Number", "PrefixNumber",
	"ServiceChargeTypeId", "TypeId", "Visible")

  SELECT GenerateGuidByText("Id", 'phone') Id, "Created", "CreatedBy", "Modified", "ModifiedBy", "LocalizationId",
	null, "PhoneChargeDescription", "Phone", null,
	coalesce( "ServiceChargeTypeId", chargeType), phoneType, true
  FROM public."ServiceChannelSupport" scs
       left outer join ( select "ServiceChannelSupportId", GetChargeTypeId(array_agg("ServiceChargeTypeId")) "ServiceChargeTypeId" from public."ServiceChannelSupportServiceChargeType" group by "ServiceChannelSupportId") cht
	on (cht."ServiceChannelSupportId" = scs."Id"),
	GetChargeTypeId('Charged') chargeType,
	GetPhoneTypeId('Phone') phoneType
  where coalesce( trim("Phone"),'')!='';

-- creates channel phones connection
INSERT INTO public."ServiceChannelPhone"(
            "Created", "CreatedBy", "Modified", "ModifiedBy", "ServiceChannelId", "PhoneId", "TypeId")

    SELECT "Created", "CreatedBy", "Modified", "ModifiedBy", "ServiceChannelId", GenerateGuidByText("Id", 'phone') PhoneId, phoneType
  FROM public."ServiceChannelSupport", GetPhoneTypeId('Phone') phoneType
  where coalesce( trim("Phone"),'')!='';


-- Phone channel
-- creates phones
INSERT INTO public."Phone"(
            "Id", "Created", "CreatedBy", "Modified", "ModifiedBy", "LocalizationId",
	    "AdditionalInformation", "ChargeDescription", "Number", "PrefixNumber",
	    "ServiceChargeTypeId", "TypeId", "Visible")

  SELECT GenerateGuidByText(pcp."PhoneChannelId", pcp."LocalizationId") Id, pcp."Created", pcp."CreatedBy", pcp."Modified", pcp."ModifiedBy", pcp."LocalizationId",
	null, pcd."Description", pcp."Number", null,
	coalesce( cht."ServiceChargeTypeId", chargeType), pc."PhoneTypeId", true
  FROM public."PhoneChannelPhone" pcp
	join public."PhoneChannel" pc on  (pc."Id" = pcp."PhoneChannelId")
	left outer join  public."PhoneChannelPhoneChargeDescription" pcd on (pc."Id" = pcd."PhoneChannelId" AND pcd."LocalizationId" = pcp."LocalizationId" )
	left outer join ( select "PhoneChannelId", GetChargeTypeId(array_agg("ServiceChargeTypeId")) "ServiceChargeTypeId" from public."PhoneChannelServiceChargeType" group by "PhoneChannelId") cht
	on (cht."PhoneChannelId" = pc."Id"),
	GetChargeTypeId('Charged') chargeType;




-- creates channel phones connection
INSERT INTO public."ServiceChannelPhone"(
            "Created", "CreatedBy", "Modified", "ModifiedBy", "ServiceChannelId", "PhoneId", "TypeId")

    SELECT pcp."Created", pcp."CreatedBy", pcp."Modified", pcp."ModifiedBy", "ServiceChannelId",  GenerateGuidByText("PhoneChannelId", "LocalizationId")  PhoneId, pc."PhoneTypeId"
  FROM public."PhoneChannelPhone" pcp
  join public."PhoneChannel" pc on  (pc."Id" = pcp."PhoneChannelId");

-- remove charged type 'other' from services
UPDATE public."Service"
   SET "ServiceChargeTypeId" =  null
 WHERE "ServiceChargeTypeId" = GetChargeTypeId('Other');

  DROP FUNCTION GetPhoneTypeId(text);
  DROP FUNCTION GetChargeTypeId(text);
  DROP FUNCTION GetChargeTypeId(uuid[]);