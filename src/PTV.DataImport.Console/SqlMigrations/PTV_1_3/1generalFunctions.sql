-- creates random unique guid
CREATE OR REPLACE FUNCTION GenerateGuid() RETURNS uuid LANGUAGE SQL AS  $$ SELECT md5(random()::text || clock_timestamp()::text)::uuid; $$;
-- creates unique guid for uuid and text combination
CREATE OR REPLACE FUNCTION GenerateGuidByText(uuid, text) RETURNS uuid LANGUAGE SQL AS  $$ SELECT md5($1::text || $2::text)::uuid; $$;
-- creates unique guid for uuid combination
CREATE OR REPLACE FUNCTION GenerateGuidByText(uuid, uuid) RETURNS uuid LANGUAGE SQL AS  $$ SELECT md5($1::text || $2::text)::uuid; $$;

-- gets language id
CREATE OR REPLACE FUNCTION GetLanguageId(text) RETURNS uuid LANGUAGE SQL AS  $$ SELECT "Id" FROM public."Language" where "Code" = $1; $$;