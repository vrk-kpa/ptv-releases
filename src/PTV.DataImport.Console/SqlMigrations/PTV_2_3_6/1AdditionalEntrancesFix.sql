do
$$
declare	
	addressTypeStreetId uuid = (select "Id" from  "AddressType" where "Code" = 'Street');
	addressTypeOtherId uuid = (select "Id" from  "AddressType" where "Code" = 'Other');

	are_row "AccessibilityRegisterEntrance"%rowtype;
	aren_row "AccessibilityRegisterEntranceName"%rowtype;
	aai_row "AddressAdditionalInformation"%rowtype;
	
	postalCodeId uuid;
begin

	for are_row in 
        select are.*
        from "AccessibilityRegisterEntrance" are
		    join "Address" adr on are."AddressId" = adr."Id"
		    join "AccessibilityRegister" acr on are."AccessibilityRegisterId" = acr."Id"
		    where are."IsMain" = false 
			    and adr."TypeId" = addressTypeStreetId
			    and acr."AddressId" != are."AddressId"
  loop
    
      if exists (select 1 from "AddressOther" where "AddressId" = are_row."AddressId") then
    		continue;	
    	end if;
  
    	select "PostalCodeId" into postalCodeId from "ClsAddressPoint" where "AddressId" = are_row."AddressId";
    	if (postalCodeId is null) then continue; end if;
    
    	update "Address" set "TypeId" = addressTypeOtherId where "Id" = are_row."AddressId";
    	
    	insert into "AddressOther" ("AddressId", "PostalCodeId", "Created", "CreatedBy", "Modified", "ModifiedBy")
   	  values(are_row."AddressId", postalCodeId, are_row."Created", are_row."CreatedBy", are_row."Modified", are_row."ModifiedBy");
    
    	for aren_row in
    		  select * 
    		  from "AccessibilityRegisterEntranceName"
    		  where "AccessibilityRegisterEntranceId" = are_row."Id"
    	loop 
    		
    		  if exists (select 1 from "AddressAdditionalInformation" where "AddressId" = are_row."AddressId" and "LocalizationId" = aren_row."LocalizationId") then 
    			    continue;
    	   	end if;	
    	   
    	   insert into "AddressAdditionalInformation" ("AddressId", "LocalizationId", "Created", "CreatedBy", "Modified", "ModifiedBy", "Text")
    	   values(are_row."AddressId", aren_row."LocalizationId", aren_row."Created", aren_row."CreatedBy", aren_row."Modified", aren_row."ModifiedBy", aren_row."Name");
    	   
    	end loop;
    	
    end loop;
	
end
$$
;