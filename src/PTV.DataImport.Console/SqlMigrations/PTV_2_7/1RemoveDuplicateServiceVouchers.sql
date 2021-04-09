DELETE FROM public."ServiceWebPage"
 WHERE "Id" IN (SELECT swp."Id"
		  FROM public."ServiceWebPage" AS swp
		  JOIN (SELECT "ServiceVersionedId", "WebPageId", "Name", "Description", "LocalizationId", MAX("Id"::text) as "max", Count(*) as "count"
			  FROM public."ServiceWebPage"
			  GROUP BY "ServiceVersionedId", "WebPageId", "Name", "LocalizationId", "Description"
			  HAVING Count(*) > 1
			  ORDER BY count) as swp2
			  ON swp."ServiceVersionedId" = swp2."ServiceVersionedId" AND
			     swp."WebPageId" = swp2."WebPageId" AND
			     (swp."Name" = swp2."Name" OR (swp."Name" is null and swp2."Name" is null)) AND
			     (swp."Description" = swp2."Description" OR (swp."Description" IS NULL AND swp2."Description" IS NULL)) AND
			     swp."LocalizationId" = swp2."LocalizationId" AND 
			     swp."Id"::text < swp2.max);