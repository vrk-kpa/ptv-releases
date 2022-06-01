import { getFieldId } from 'utils/fieldIds';
import { useGetSelectedLanguage } from './';

type ReturnValue = (fieldPath: string) => string;

export function useGetFieldId(): ReturnValue {
  const lang = useGetSelectedLanguage();

  return (fieldPath: string) => {
    return getFieldId(fieldPath, lang, undefined);
  };
}
