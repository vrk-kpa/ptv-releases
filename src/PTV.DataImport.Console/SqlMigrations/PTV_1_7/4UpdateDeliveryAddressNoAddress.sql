update "Address"
   set "TypeId" = (select "Id" from "AddressType" where "Code" = 'NoAddress')
where "Id" in
(
select "Id" from "Address" as A
where "TypeId" = (select "Id" from "AddressType" where "Code" = 'Street')
and not exists (select "AddressId" from "AddressStreet" as S
        where S."AddressId" = A."Id")
and exists (select "AddressId" from "AddressAdditionalInformation" as I
        where I."AddressId" = A."Id")
);
