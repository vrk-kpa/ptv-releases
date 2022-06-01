import { useTranslation } from 'react-i18next';
import { Language } from 'types/enumTypes';

export function useGetUiLanguage(): Language {
  const { i18n } = useTranslation();
  return i18n.language as Language;
}
