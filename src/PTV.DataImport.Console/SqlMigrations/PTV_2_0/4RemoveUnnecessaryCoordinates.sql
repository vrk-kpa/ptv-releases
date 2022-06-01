DELETE FROM public."Coordinate"
 WHERE "CoordinateState" = 'EmptyInputReceived' OR "CoordinateState" = 'Loading';