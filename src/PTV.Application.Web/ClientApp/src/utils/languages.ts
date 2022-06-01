import { Language } from 'types/enumTypes';
import { LocalizedText } from 'types/miscellaneousTypes';

export const LanguagePriorities: Language[] = ['fi', 'sv', 'en', 'se', 'smn', 'sms'];

export function getLanguagePriority(language: Language): number {
  const index = LanguagePriorities.findIndex((x) => x === language);
  if (index === -1) {
    return LanguagePriorities.length;
  }

  return index;
}

export const languagesSort = (a: Language, b: Language): number => getLanguagePriority(a) - getLanguagePriority(b);

export function getFirstTranslationKey(text: LocalizedText): Language | undefined {
  return Object.getOwnPropertyNames(text)
    .map((key) => key as Language)
    .sort(languagesSort)
    .find((key) => !!text[key]);
}

export function getOrderedLanguageVersionKeys<T>(languageVersions: { [language in Language]?: T }): Language[] {
  const keys = Object.keys(languageVersions) as Language[];
  keys.sort((first, second) => getLanguagePriority(first) - getLanguagePriority(second));
  return keys;
}
