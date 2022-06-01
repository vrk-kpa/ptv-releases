import { Language } from 'types/enumTypes';
import { getFieldId } from 'utils/fieldIds';
import { useGetSelectedLanguage } from './';

type ReturnValue = (fieldPath: string, compareLanguageCode: Language | undefined) => string;

export function useGetCompareFieldId(): ReturnValue {
  const lang = useGetSelectedLanguage();

  return (fieldPath: string, compareLanguageCode: Language | undefined) => {
    return getFieldId(fieldPath, lang, compareLanguageCode);
  };
}
