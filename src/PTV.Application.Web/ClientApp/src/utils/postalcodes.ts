import { Language } from 'types/enumTypes';
import { PostalCode } from 'types/postalCodeTypes';
import { getTextByLangPriority } from './translations';

/**
 * Returns name of the city in langauge specified by @param wantedLanguage.
 * If there is no name for country in that language it looks up other langauge versions
 * using priority specified by {@link LanguagePriorities}
 * If no value cannot be found, @param fallBackValue is returned
 */
export function getNameOfCity(code: string | null, wantedLanguage: Language, postalCodes: PostalCode[], fallBackValue = ''): string {
  if (!code) return fallBackValue;
  const postalCode = postalCodes.find((x) => x.code === code);
  return postalCode ? getTextByLangPriority(wantedLanguage, postalCode.names, postalCode.names.fi ?? fallBackValue) : fallBackValue;
}
