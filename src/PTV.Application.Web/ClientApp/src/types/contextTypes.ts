import { Language } from './enumTypes';

export type ServiceMetaContextState = {
  compareLanguage: Language | undefined;
  compareLanguageIndex: number;
  updateCompareLanguage: (language: Language | undefined, index?: number) => void;
};
