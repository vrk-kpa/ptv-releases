import { DateTime } from 'luxon';
import { getKeys } from 'utils';
import * as apimodels from 'types/api/connectionApiModel';
import * as enumTypes from 'types/enumTypes';
import * as uimodels from 'types/forms/connectionFormTypes';
import { LanguageVersionType, RequiredLanguageVersionType } from 'types/languageVersionTypes';
import { toLocalDateTime, toOptionalDateTime, toWeekDay } from 'utils/date';

export function toApiModel(source: uimodels.ConnectionFormModel): apimodels.ConnectionApiModel {
  const { languageVersions, openingHours, emails, webPages, faxNumbers, phoneNumbers, addresses, ...sameFields } = source;

  const languages = getKeys(languageVersions);

  /* eslint-disable @typescript-eslint/no-non-null-assertion */
  const convertedLVs = Object.assign({}, ...languages.map((k) => ({ [k]: languageVersions[k]! })));

  return {
    ...sameFields,
    digitalAuthorizations: source.digitalAuthorizations.map((x) => x.id),
    languageVersions: convertedLVs,
    openingHours: toOpeningHours(openingHours),
    emails: toLanguageVersions(emails, languages),
    webPages: toLanguageVersions(webPages, languages),
    faxNumbers: toFaxNumbers(faxNumbers, languages),
    phoneNumbers: toPhoneNumbers(phoneNumbers, languages),
    addresses: addresses.map((x) => toAddress(x, languages)),
  };
}

function toAddress(source: uimodels.AddressModel, languages: enumTypes.Language[]): apimodels.AddressModel {
  const { languageVersions, ...sameFields } = source;
  return {
    ...sameFields,
    languageVersions: toAddressLv(languageVersions, languages),
  };
}

function toAddressLv(
  source: RequiredLanguageVersionType<uimodels.AddressLvModel>,
  languages: enumTypes.Language[]
): LanguageVersionType<uimodels.AddressLvModel> {
  const result: LanguageVersionType<uimodels.AddressLvModel> = {};
  for (const lang of languages) {
    result[lang] = source[lang];
  }
  return result;
}

function toPhoneNumbers(
  source: RequiredLanguageVersionType<uimodels.PhoneNumberLvModel[]>,
  languages: enumTypes.Language[]
): LanguageVersionType<apimodels.PhoneNumberLv[]> {
  const result: LanguageVersionType<apimodels.PhoneNumberLv[]> = {};
  for (const lang of languages) {
    result[lang] = [];
    const items = source[lang];
    items?.forEach((x) =>
      result[lang]?.push({
        ...x,
        dialCodeId: x.dialCodeType === 'Normal' ? x.dialCodeId : null,
      })
    );
  }

  return result;
}

function toFaxNumbers(
  source: RequiredLanguageVersionType<uimodels.FaxNumberLvModel[]>,
  languages: enumTypes.Language[]
): LanguageVersionType<uimodels.FaxNumberLvModel[]> {
  const result: LanguageVersionType<uimodels.FaxNumberLvModel[]> = {};
  for (const lang of languages) {
    result[lang] = [];
    const items = source[lang];
    items?.forEach((x) =>
      result[lang]?.push({
        ...x,
      })
    );
  }

  return result;
}

function toOpeningHours(source: uimodels.OpeningHoursModel): apimodels.OpeningHours {
  return {
    standardHours: source.standardHours.map((x, index) => toStandardHour(x, index)),
    specialHours: source.specialHours.map((x, index) => toSpecialHour(x, index)),
    holidayHours: source.holidayHours,
    exceptionalHours: source.exceptionalHours.map((x, index) => toExceptionalHour(x, index)),
  };
}

function toExceptionalHour(source: uimodels.ExceptionalServiceHourModel, orderNumber: number): apimodels.ExceptionalServiceHour {
  const { openingHoursFrom, openingHoursTo, validityPeriod, ...sameFields } = source;

  return {
    ...sameFields,
    openingHoursFrom: openingHoursFrom ? toLocalDateTime(openingHoursFrom).toUTC().toISO() : null,
    openingHoursTo: validityPeriod === 'BetweenDates' && openingHoursTo ? toLocalDateTime(openingHoursTo).toUTC().toISO() : null,
    orderNumber: orderNumber,
  };
}

function toSpecialHour(source: uimodels.SpecialServiceHourModel, orderNumber: number): apimodels.SpecialServiceHour {
  const { openingHoursFrom, openingHoursTo, dayFrom, dayTo, validityPeriod, ...sameFields } = source;

  return {
    ...sameFields,
    openingHoursFrom: validityPeriod === 'BetweenDates' && openingHoursFrom ? toLocalDateTime(openingHoursFrom).toUTC().toISO() : null,
    openingHoursTo: validityPeriod === 'BetweenDates' && openingHoursTo ? toLocalDateTime(openingHoursTo).toUTC().toISO() : null,
    dayFrom: toDay(validityPeriod, toOptionalDateTime(openingHoursFrom), dayFrom),
    dayTo: toDay(validityPeriod, toOptionalDateTime(openingHoursTo), dayTo),
    isPeriod: validityPeriod === 'BetweenDates',
    orderNumber: orderNumber,
  };
}

function toDay(validityPeriod: uimodels.ValidityPeriod, date: DateTime | null, weekday: enumTypes.Weekday | null): enumTypes.Weekday {
  if (validityPeriod === 'UntilFurtherNotice') {
    if (!weekday) {
      throw new Error('Cannot convert to weekday: weekday is null');
    }
    return weekday;
  }

  if (!date) {
    throw new Error(`Cannot convert to weekday: date is null`);
  }

  return toWeekDay(date);
}

function toStandardHour(source: uimodels.StandardServiceHourModel, orderNumber: number): apimodels.StandardServiceHour {
  const { openingHoursFrom, openingHoursTo, dailyOpeningTimes, isReservation, ...sameFields } = source;
  return {
    ...sameFields,
    isReservation: source.openingType === 'OpenByReservation',
    openingHoursFrom: openingHoursFrom ? toLocalDateTime(openingHoursFrom).toUTC().toISO() : null,
    openingHoursTo: openingHoursTo ? toLocalDateTime(openingHoursTo).toUTC().toISO() : null,
    isPeriod: source.validityPeriod === 'BetweenDates',
    isNonStop: source.openingType === 'Always',
    orderNumber: orderNumber,
    dailyOpeningTimes: source.isReservation || source.openingType === 'Always' ? {} : toDailyOpeningTimes(dailyOpeningTimes),
  };
}

function toDailyOpeningTimes(source: uimodels.DailyOpeningTimeModel[]): apimodels.WeekdayMap<apimodels.DailyOpeningTime> {
  const result: apimodels.WeekdayMap<apimodels.DailyOpeningTime> = {};

  for (const openingTime of source) {
    if (!openingTime.times || openingTime.times.length === 0 || !openingTime.active) continue;

    result[openingTime.day] = {
      day: openingTime.day,
      times: openingTime.times,
    };
  }

  return result;
}

function toLanguageVersions<T>(source: RequiredLanguageVersionType<T[]>, languages: enumTypes.Language[]): LanguageVersionType<T[]> {
  const result: LanguageVersionType<T[]> = {};
  for (const lang of languages) {
    result[lang] = [];
    const items = source[lang];
    items?.forEach((x) => result[lang]?.push(x));
  }

  return result;
}
