-- TEST
-- Allow the Church access GDs
INSERT INTO public."AccessRightsOperationsUI" ("Id", "AllowedAllOrganizations", "OrganizationId", "Permission", "Role", "RulesAll", "RulesOwn")
    VALUES (uuid_generate_v4(), false, '4403f2a2-d83d-4a86-85ef-cb653099cb61', 'generalDescriptions', 'pete', 31, 31);

-- Restrict them from accessing non-church GDs
INSERT INTO public."OrganizationFilter" ("FilterId", "OrganizationId") 
    VALUES ('ba3293c0-16b4-4c62-bb22-33f1c42ddb8e', '4403f2a2-d83d-4a86-85ef-cb653099cb61');
