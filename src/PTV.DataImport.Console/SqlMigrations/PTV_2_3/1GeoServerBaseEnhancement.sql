drop index if exists "IX_GeoCoordinate";
drop materialized view if exists "geoCoordinate";

create materialized view "geoCoordinate" as 
select distinct
	sca."AddressId",
	sca."ServiceChannelVersionedId" as "VersionedId",
    scv."UnificRootId" as "RootId",
    gco."CoordinateState",
    gco."Location" as "Location-3067",
    st_transform(gco."Location", 4326) as "Location-4326"
from "ServiceChannelAddress" sca 
join (
		select "AddressId", "CoordinateState", st_setsrid(st_point("Longitude", "Latitude"), 3067)::geometry(Point,3067)  as "Location"
		from "Coordinate" 
		where lower("CoordinateState") in ('enteredbyar', 'enteredbyuser', 'ok')
) gco on sca."AddressId" = gco."AddressId" 
join "ServiceChannelVersioned" scv on sca."ServiceChannelVersionedId" = scv."Id"
where scv."PublishingStatusId" = ( select pst."Id" from "PublishingStatusType" pst where lower(pst."Code") = 'published')
;

create unique index "IX_GeoCoordinate" on "geoCoordinate"("AddressId", "VersionedId", "RootId", "CoordinateState", "Location-3067", "Location-4326");