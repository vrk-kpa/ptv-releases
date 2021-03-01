CREATE OR REPLACE FUNCTION GetPhoneDescriptionTypeId(text) RETURNS uuid LANGUAGE SQL AS  $$ SELECT "Id" FROM "PhoneDescriptionType" WHERE "Code" = $1; $$;

SELECT * FROM GetPhoneDescriptionTypeId('ChargeDescription');
SELECT * FROM GetPhoneDescriptionTypeId('AdditionalInformation');

INSERT INTO public."Phone"(
            "Id", "Created", "CreatedBy", "Modified", "ModifiedBy", "LocalizationId",
            "AdditionalInformation", "ChargeDescription", "Number", "PrefixNumber", "ServiceChargeTypeId", "TypeId", "Visible")


  SELECT op."Id", op."Created", op."CreatedBy", op."Modified", op."ModifiedBy", l."Id", 
	opd."Description" AdditionalInfo, opdCharge."Description" Charge, op."Number", op."PrefixNumber", op."ServiceChargeTypeId", op."TypeId", true
  FROM public."OrganizationPhone" op
  join public."Language" l on (l."Code" = 'fi')
  left outer join public."OrganizationPhoneDescription" opd on (op."Id" = opd."OrganizationPhoneId" and opd."TypeId" = GetPhoneDescriptionTypeId('AdditionalInformation'))
  left outer join public."OrganizationPhoneDescription" opdCharge on (op."Id" = opdCharge."OrganizationPhoneId" and opdCharge."TypeId" = GetPhoneDescriptionTypeId('ChargeDescription'));

UPDATE public."OrganizationPhone"
   SET "PhoneId"="Id";

DROP FUNCTION GetPhoneDescriptionTypeId(text);