import { Language } from 'types/enumTypes';
import { GeneralDescriptionLanguageModel } from 'types/generalDescriptionTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { LanguagePriorities } from './languages';

type LanguageVersions = LanguageVersionType<GeneralDescriptionLanguageModel>;
type Selector<T> = (lv: GeneralDescriptionLanguageModel) => T | null | undefined;

export function getGdValueByLangPriority<T>(lvs: LanguageVersions, wantedLanguage: Language, selector: Selector<T>, fallBackValue: T): T {
  const languages = [wantedLanguage].concat(LanguagePriorities);
  for (const lang of languages) {
    const lv = lvs[lang];
    if (lv) {
      const value = selector(lv);
      if (value) {
        return value;
      }
    }
  }

  return fallBackValue;
}

export function getGdValueOrDefault<T>(
  lvs: LanguageVersions | null | undefined,
  wantedLanguage: Language | null | undefined,
  selector: Selector<T>,
  defaultValue: T
): T {
  if (!wantedLanguage || !lvs) {
    return defaultValue;
  }

  const lv = lvs[wantedLanguage];
  if (lv) {
    const value = selector(lv);
    if (value) {
      return value;
    }
  }

  return defaultValue;
}

export function createEmptyLanguageVersion(language: Language): GeneralDescriptionLanguageModel {
  return {
    isEnabled: false,
    language: language,
    status: 'Draft',
    name: '',
    alternativeName: '',
    description: null,
    backgroundDescription: null,
    generalDescriptionTypeAdditionalInformation: null,
    summary: '',
    userInstructions: null,
    deadline: null,
    processingTime: null,
    periodOfValidity: null,
    conditions: null,
    charge: {
      info: '',
    },
    keywords: [],
    laws: [],
  };
}
