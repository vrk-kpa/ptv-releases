CREATE OR REPLACE
FUNCTION RemoveSoftHyphens()
RETURNS void
LANGUAGE plpgsql AS
$$
DECLARE
  tableNames CURSOR FOR
    SELECT tablename as tableName
    FROM pg_tables
    WHERE tablename NOT LIKE 'pg_%';
  hyphen char := chr(173);
  columnName text;
  sqlCommand text := (
    'UPDATE %I ' ||
    'SET %I = replace(%I, %L, %L) ' ||
    'WHERE %I LIKE ''%%%s%%'''
  );
BEGIN
  FOR tableName IN tableNames LOOP
    FOR columnName IN (
      SELECT column_name
      FROM information_schema.Columns
      WHERE
        table_schema = 'public' AND
        table_name = tableName.tableName AND
        data_type = 'text'
    ) LOOP
      EXECUTE format(
        sqlCommand
        ,
        tableName.tableName,
        columnName,
        columnName,
        hyphen,
        '',
        columnName,
        hyphen
      );
    END LOOP;
  END LOOP;
END;
$$;


DO $$
BEGIN
  PERFORM RemoveSoftHyphens();
END;
$$;

DROP FUNCTION IF EXISTS "RemoveSoftHyphens"(text);
