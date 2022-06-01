import { Language } from './enumTypes';
import { LocalizedText } from './miscellaneousTypes';

export type AnnotationToolRequest = {
  description: string;
  id: string | null;
  industrialClasses?: string[];
  languageCode: Language | undefined;
  lifeEvents?: string[];
  name: string;
  serviceClasses: string[];
  shortDescription: string;
  targetGroups: string[];
};

export type OntologyTermsListRequest = {
  searchValue: string | undefined;
  language: Language;
};

export type OntologyTermsResponse = {
  code: string;
  descriptions: LocalizedText;
  id: string;
  names: LocalizedText;
};
