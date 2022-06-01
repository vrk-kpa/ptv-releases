import { Language } from 'types/enumTypes';

export function withNamespace(namespace: string, name: string): string {
  if (!namespace) {
    return name;
  }
  return `${namespace}.${name}`;
}

export const getFieldPath = (name: string, language: Language | undefined, compareLanguage: Language | undefined): string => {
  const lang = compareLanguage || language;
  return lang ? `languageVersions.${lang}.${name}` : name;
};

export const getFieldId = (name: string, language: Language | undefined, compareLanguage: Language | undefined): string => {
  if (compareLanguage) {
    return language ? `languageVersions.${language}.${compareLanguage}.${name}` : name;
  }

  return language ? `languageVersions.${language}.${name}` : name;
};
