import { Language } from 'types/enumTypes';
import { LocalizedText } from 'types/miscellaneousTypes';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { translateToLang } from 'utils/translations';

export function useTranslateWithUiLanguage(): (text?: LocalizedText, fallBackValue?: string) => string {
  const lang = useGetUiLanguage();

  return (text, fallBackValue) => {
    if (!text) {
      return fallBackValue || '';
    }

    return translateText(lang, text, fallBackValue);
  };
}

function translateText(lang: Language, text: LocalizedText, fallBackValue?: string): string {
  const value = translateToLang(lang, text);
  return value || fallBackValue || '';
}
