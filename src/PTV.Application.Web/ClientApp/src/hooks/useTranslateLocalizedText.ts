import { useTranslation } from 'react-i18next';
import { i18n } from 'i18next';
import { LocalizedText } from 'types/miscellaneousTypes';
import { translate } from 'utils/translations';

export default function useTranslateLocalizedText(): (text: LocalizedText, defaultValue?: string) => string {
  const { i18n } = useTranslation();
  return (text, defaultValue) => {
    return translateText(i18n, text, defaultValue);
  };
}

export function translateText(i18: i18n, text: LocalizedText, defaultValue?: string): string {
  const value = translate(i18, text);
  return value || defaultValue || '';
}
