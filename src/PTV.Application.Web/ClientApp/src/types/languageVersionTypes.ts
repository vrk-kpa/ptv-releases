import { Language } from './enumTypes';

export type LanguageVersionType<T> = {
  [language in Language]?: T;
};

export type RequiredLanguageVersionType<T> = {
  [language in Language]: T;
};

export type LanguageVersionWithName = {
  language: Language;
  name: string;
};
