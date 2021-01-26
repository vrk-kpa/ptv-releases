-- Delete unused external sources of services without versions or with versions where all are in removed state.
DELETE 
FROM public."ExternalSource" ext
WHERE ext."PTVId" IN (
  SELECT "Id"
  FROM public."Service" ser
  WHERE NOT EXISTS (SELECT 1
                     FROM public."ServiceVersioned" serv
                     WHERE ser."Id" = serv."UnificRootId" AND
                           serv."PublishingStatusId" <> (SELECT "Id" 
						         FROM public."PublishingStatusType" 
						         WHERE "Code" = 'Removed'))); 
							                             
-- Delete unused external sources of channels without versions or with versions where all are in removed state.                
DELETE
FROM public."ExternalSource" ext
WHERE 
 ext."PTVId" IN (
 SELECT "Id"
 FROM public."ServiceChannel" chan
 WHERE NOT EXISTS (SELECT 1
                   FROM   public."ServiceChannelVersioned" chanv
                   WHERE  chan."Id" = chanv."UnificRootId" AND
                          chanv."PublishingStatusId" <> (SELECT "Id" 
                                                         FROM public."PublishingStatusType" 
						         WHERE "Code" = 'Removed')));                     