INSERT INTO public."Organization"("Id") SELECT "Id" FROM public."OrganizationVersioned";
INSERT INTO public."Service"("Id") SELECT "Id" FROM public."ServiceVersioned";
INSERT INTO public."ServiceChannel"("Id") SELECT "Id" FROM public."ServiceChannelVersioned";
INSERT INTO public."StatutoryServiceGeneralDescription"("Id") SELECT "Id" FROM public."StatutoryServiceGeneralDescriptionVersioned";
