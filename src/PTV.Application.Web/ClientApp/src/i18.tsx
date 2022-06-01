/* eslint-disable import/no-named-as-default-member */
import { initReactI18next } from 'react-i18next';
// eslint-disable-next-line import/no-named-as-default
import i18n from 'i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import Backend from 'i18next-http-backend';

i18n
  .use(Backend) // https://github.com/i18next/i18next-http-backend
  .use(LanguageDetector) // https://github.com/i18next/i18next-browser-languageDetector
  .use(initReactI18next) // for init options https://www.i18next.com/overview/configuration-options
  .init({
    backend: {
      loadPath: '/locales/{{lng}}.json',
      allowMultiLoading: true,
    },
    supportedLngs: ['en', 'fi', 'sv', 'se'],
    preload: ['en', 'fi', 'sv', 'se'],
    debug: false,
    keySeparator: false, // so that we can use dot in key name e.g. Application.Environment.Development
    interpolation: {
      escapeValue: false, // not needed for react as it escapes by default
    },
  });

export default i18n;
