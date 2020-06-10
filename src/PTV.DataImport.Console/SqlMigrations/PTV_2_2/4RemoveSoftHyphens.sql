CREATE OR REPLACE
FUNCTION RemoveSoftHyphens(text)
RETURNS void
LANGUAGE plpgsql AS
$$
DECLARE
	-- chr(173) is soft hyphen U+00AD
	hyphen char := chr(173);
BEGIN
  EXECUTE format(
    'UPDATE %I SET "Name" = replace("Name", %L, %L);',
    $1, hyphen, ''
  );
END;
$$;


DO $$
BEGIN
  PERFORM RemoveSoftHyphens('ServiceChannelName');
  PERFORM RemoveSoftHyphens('OrganizationName');
  PERFORM RemoveSoftHyphens('ServiceName');
  PERFORM RemoveSoftHyphens('StatutoryServiceName');
  PERFORM RemoveSoftHyphens('ServiceCollectionName');
END;
$$;

DROP FUNCTION IF EXISTS "RemoveSoftHyphens"(text);
