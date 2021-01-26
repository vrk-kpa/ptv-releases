UPDATE public."SahaOrganizationInformation"
   SET "LastOperationId"= substr("LastOperationId",2,65) 
   WHERE LEFT("LastOperationId",1) = '"' AND 
         RIGHT("LastOperationId",1) = '"' AND
         char_length("LastOperationId") = 67;
