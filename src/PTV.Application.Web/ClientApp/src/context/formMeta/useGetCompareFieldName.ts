import { Language } from 'types/enumTypes';
import { getFieldPath } from 'utils/fieldIds';
import { useGetSelectedLanguage } from './useGetSelectedLanguage';

type ReturnValue = (fieldPath: string, compareLanguageCode: Language | undefined) => string;

export function useGetCompareFieldName(): ReturnValue {
  const lang = useGetSelectedLanguage();

  return (fieldPath: string, compareLanguageCode: Language | undefined) => {
    return getFieldPath(fieldPath, lang, compareLanguageCode);
  };
}
