-- delete address type 'Moving' from "AddressTypeName"
DELETE from public."AddressTypeName" where "TypeId" = (Select "Id" from public."AddressType" where "Code" = 'Moving');

DELETE FROM public."AddressType"
WHERE "Code" = 'Moving';