import { Language } from 'types/enumTypes';
import { useFormMetaContext } from './';

export function useGetSelectedLanguage(): Language {
  const ctx = useFormMetaContext();
  const lang = ctx.selectedLanguageCode;
  if (!lang) {
    throw Error('Selected language in FormMetaContext has not been provided.');
  }
  return lang;
}
