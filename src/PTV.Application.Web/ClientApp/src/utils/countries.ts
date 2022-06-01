import { Country } from 'types/areaTypes';
import { Language } from 'types/enumTypes';
import { getTextByLangPriority } from './translations';

/**
 * Returns name of the country in language specified by @param wantedLanguage.
 * If there is no name for country in that language it looks up other langauge versions
 * using priority specified by {@link LanguagePriorities}
 * If no value cannot be found, @param fallBackValue is returned
 */
export function getNameofCountry(
  countryCode: string | null | undefined,
  wantedLanguage: Language,
  countries: Country[],
  fallBackValue = ''
): string {
  if (!countryCode) return fallBackValue;
  const country = countries.find((x) => x.code === countryCode);
  if (!country) return fallBackValue;
  return country?.names ? getTextByLangPriority(wantedLanguage, country.names) : fallBackValue;
}
