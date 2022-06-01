import React from 'react';
import { useTranslation } from 'react-i18next';
import { LanguageMenu, LanguageMenuItem } from 'suomifi-ui-components';
import { AppEnvironment, Language } from 'types/enumTypes';

const FiKey = 'Ptv.Header.LanguageSelector.InFinnish';
const SvKey = 'Ptv.Header.LanguageSelector.InSwedish';
const EnKey = 'Ptv.Header.LanguageSelector.InEnglish';
const NoTranslationKey = 'Ptv.Header.LanguageSelector.NoTranslation';

const LanguageTitles = new Map<Language | string, string>([
  ['fi', FiKey],
  ['sv', SvKey],
  ['en', EnKey],
  ['se', NoTranslationKey],
]);

const SupportedUiLanguages = ['fi', 'sv', 'en'];

type LanguageSelectorProps = {
  environment: AppEnvironment;
};

export default function LanguageSelector(props: LanguageSelectorProps): React.ReactElement {
  const { i18n, t } = useTranslation();
  const selectedLanguage = i18n.language;

  function changeLanguage(language: Language) {
    i18n.changeLanguage(language);
  }

  function getSupportedLanguages(): string[] {
    if (props.environment === 'Dev') {
      return SupportedUiLanguages.concat(['se']);
    }

    return SupportedUiLanguages;
  }

  const key = LanguageTitles.get(selectedLanguage) || EnKey;
  const languages = getSupportedLanguages();
  return (
    <LanguageMenu name={t(key)}>
      {languages.map((lang) => (
        <LanguageMenuItem key={lang} onSelect={() => changeLanguage(lang as Language)} selected={selectedLanguage === lang}>
          {t(LanguageTitles.get(lang) || EnKey)}
        </LanguageMenuItem>
      ))}
    </LanguageMenu>
  );
}
