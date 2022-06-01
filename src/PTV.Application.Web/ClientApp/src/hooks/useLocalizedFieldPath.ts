import { cService } from 'types/forms/serviceFormTypes';
import { useFormMetaContext } from 'context/formMeta';

export const useLocalizedFieldPath = (fieldName: string): string => {
  const { selectedLanguageCode } = useFormMetaContext();

  return `${cService.languageVersions}.${selectedLanguageCode}.${fieldName}`;
};
