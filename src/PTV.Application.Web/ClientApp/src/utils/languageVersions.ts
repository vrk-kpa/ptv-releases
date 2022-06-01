import { Language, inTranslationStateType } from 'types/enumTypes';
import { LastTranslationType } from 'types/forms/translationTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { LanguagePriorities } from './languages';

export function hasItems<T>(data: LanguageVersionType<T[]>, lang: Language): boolean {
  const items = data[lang];
  return !items ? false : items.length > 0;
}

type Selector<T, TValue> = (lv: T) => TValue | null | undefined;

/**
 * Returns field from language version specified by @param wantedLanguage.
 * If the @param selector returns null/undefined it looks up other langauge versions
 * using priority specified by {@link LanguagePriorities}
 * If no value cannot be found, @param fallBackValue is returned
 */
export function getLanguageVersionValue<T, TValue>(
  languageVersions: LanguageVersionType<T>,
  wantedLanguage: Language,
  selector: Selector<T, TValue>,
  fallBackValue: TValue
): TValue {
  const wantedValue = findValueBySelector(languageVersions, wantedLanguage, selector);
  if (wantedValue) return wantedValue;

  // Wanted language version does not contain value, loop through
  // all the languages to find it. This is mainly for cases where we
  // want to show e.g. name with UI/language version language but
  // for some data it does not exist so we settle for next best thing

  for (const lang of LanguagePriorities) {
    const value = findValueBySelector(languageVersions, lang, selector);
    if (value) return value;
  }

  return fallBackValue;
}

function findValueBySelector<T, TValue>(
  languageVersions: LanguageVersionType<T>,
  wantedLanguage: Language,
  selector: Selector<T, TValue>
): TValue | null | undefined {
  const languageVersion = languageVersions[wantedLanguage];
  if (!languageVersion) return undefined;
  return selector(languageVersion);
}

export function getUniqueInTranslationSourceLangs(lastTranslations: LastTranslationType[]): Language[] {
  const sourceLangs = lastTranslations
    .filter((translation) => inTranslationStateType.includes(translation.state))
    .map((translation) => translation.sourceLanguage);

  const uniqueSourceLangs = Array.from(new Set(sourceLangs));
  return uniqueSourceLangs;
}
