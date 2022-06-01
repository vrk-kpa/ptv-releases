import { i18n } from 'i18next';
import {
  ChannelType,
  ChargeType,
  GeneralDescriptionType,
  HolidayEnum,
  Language,
  PublishingStatus,
  ServiceType,
  Weekday,
  language,
} from 'types/enumTypes';
import { OpeningHourType } from 'types/forms/connectionFormTypes';
import { LocalizedText, RequiredLocalizedText } from 'types/miscellaneousTypes';
import { LanguagePriorities } from './languages';

export function translate(i18: i18n, text: LocalizedText, defaultValue?: string | null | undefined): string | null | undefined {
  if (i18.language === 'fi' && text.fi) {
    return text.fi;
  }

  if (i18.language === 'sv' && text.sv) {
    return text.sv;
  }

  if (i18.language === 'en' && text.en) {
    return text.en;
  }

  return defaultValue;
}

export function translateToLang(lang: Language, text: LocalizedText, defaultValue?: string | null | undefined): string | null | undefined {
  return text[lang] ?? defaultValue;
}

export const getDefaultTranslation = (names: LocalizedText): string => {
  const keys = (names && (Object.keys(names) as Language[])) || [];
  const items = keys.map((key) => ({ key, value: names[key] }));
  const sortedItems = items.sort((a, b) => {
    const langOrderA = language.indexOf(a.key);
    const langOrderB = language.indexOf(b.key);
    return langOrderA > langOrderB ? 1 : -1;
  });
  const firstItem = sortedItems.filter((x) => x)[0] || {};
  return firstItem.value || '';
};

export function getTextByLangPriority(wantedLanguage: Language, text: LocalizedText, fallBackValue?: string | null | undefined): string {
  const languages = [wantedLanguage].concat(LanguagePriorities);
  for (const lang of languages) {
    const txt = text[lang];
    if (txt) {
      return txt;
    }
  }

  return fallBackValue || '';
}

export function localeCompareTexts(left: LocalizedText, right: LocalizedText, lang: Language): number {
  const l = translateToLang(lang, left, undefined) ?? '';
  const r = translateToLang(lang, right, undefined) ?? '';
  return l.localeCompare(r);
}

export function getKeyForServiceType(type: ServiceType): string {
  return `Ptv.Service.ServiceType.${type}`;
}

export function getKeyForGdType(type: GeneralDescriptionType): string {
  if (type === 'Church') return 'Ptv.EnumTypes.GeneralDescriptionType.EvangelicalLutheranChurch';
  return `Ptv.EnumTypes.GeneralDescriptionType.${type}`;
}

export function getKeyForChannelType(type: ChannelType): string {
  return `Ptv.ChannelType.${type}`;
}

export function getKeysForStatusType(type: PublishingStatus): string {
  return `Ptv.StatusType.${type}`;
}

const LanguageKeys = new Map<Language, string>([
  ['en', 'Ptv.Language.English'],
  ['fi', 'Ptv.Language.Finnish'],
  ['sv', 'Ptv.Language.Swedish'],
  ['se', 'Ptv.Language.NorthernSami'],
  ['smn', 'Ptv.Language.InariSami'],
  ['sms', 'Ptv.Language.SkoltSami'],
]);

export function getKeyForLanguage(language: Language): string {
  return LanguageKeys.get(language) || 'Ptv.Language.English';
}

export function getKeyForWeekday(day: Weekday): string {
  return `Ptv.Day.${day}`;
}

export function getKeyForHoliday(holiday: HolidayEnum): string {
  return `Ptv.Holiday.${holiday}`;
}

export function getKeyForServiceChargeType(chargeType: ChargeType | null | undefined): string {
  if (!chargeType) return 'Ptv.Service.ChargeType.NoValue';
  return `Ptv.Service.ChargeType.${chargeType}`;
}

export function getKeyForStandardServiceHourOpeningType(type: OpeningHourType): string {
  return `Ptv.StandardServiceHour.OpeningType.${type}`;
}

export function toRequiredLocalizedText(source: LocalizedText, defaultValue: string): RequiredLocalizedText {
  return {
    en: source.en ?? defaultValue,
    fi: source.fi ?? defaultValue,
    sv: source.sv ?? defaultValue,
    se: source.se ?? defaultValue,
    smn: source.smn ?? defaultValue,
    sms: source.sms ?? defaultValue,
  };
}
