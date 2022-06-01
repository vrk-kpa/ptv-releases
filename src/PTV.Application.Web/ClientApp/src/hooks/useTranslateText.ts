import { useTranslation } from 'react-i18next';
import { i18n } from 'i18next';
import { TranslatedText } from 'types/miscellaneousTypes';
import { translate } from 'utils/translations';

export function useTranslateText(): (text: TranslatedText, defaultValue?: string) => string {
  const { i18n } = useTranslation();
  return (text, defaultValue) => {
    return translateText(i18n, text, defaultValue);
  };
}

function translateText(i18: i18n, text: TranslatedText, defaultValue?: string): string {
  const value = translate(i18, text.Texts, text.DefaultText);
  return value || defaultValue || '';
}
