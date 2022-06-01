-- Set voucherType URL 
UPDATE public."ServiceVersioned"
SET "VoucherTypeId"= (SELECT "Id"
	                       FROM public."VoucherType"
	                       WHERE "Code" = 'Url')
WHERE "WebPageInUse" = 'TRUE';

-- Set voucherType NotUsed 
UPDATE public."ServiceVersioned"
SET "VoucherTypeId"= (SELECT "Id"
	                       FROM public."VoucherType"
	                       WHERE "Code" = 'NotUsed')
WHERE "WebPageInUse" = 'FALSE';

