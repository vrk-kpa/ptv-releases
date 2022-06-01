import { getKeys } from 'utils';
import * as apimodels from 'types/api/connectionApiModel';
import * as enumTypes from 'types/enumTypes';
import * as uimodels from 'types/forms/connectionFormTypes';
import { LanguageVersionType, RequiredLanguageVersionType } from 'types/languageVersionTypes';
import { getTextByLangPriority, toRequiredLocalizedText } from 'utils/translations';

export function toUiModel(source: apimodels.ConnectionApiModel): uimodels.ConnectionFormModel {
  const { languageVersions, chargeType, ...sameFields } = source;

  const languages = getKeys(languageVersions);

  /* eslint-disable @typescript-eslint/no-non-null-assertion */
  const convertedLVs = Object.assign({}, ...languages.map((k) => ({ [k]: languageVersions[k]! })));

  return {
    ...sameFields,
    chargeType: chargeType ? chargeType : 'Charged',
    openingHours: openingHoursToUiModel(source.openingHours),
    digitalAuthorizations: source.digitalAuthorizations.map((x) => {
      return { id: x };
    }),
    languageVersions: convertedLVs,
    emails: toRequiredLanguageVersion(source.emails),
    webPages: toRequiredLanguageVersion(source.webPages),
    faxNumbers: toRequiredLanguageVersion(source.faxNumbers),
    phoneNumbers: mapPhoneNumbers(source.phoneNumbers),
    addresses: source.addresses.map((x) => toAddressUiModel(x)),
  };
}

function mapPhoneNumbers(
  source: LanguageVersionType<apimodels.PhoneNumberLv[]>
): RequiredLanguageVersionType<uimodels.PhoneNumberLvModel[]> {
  const result: RequiredLanguageVersionType<uimodels.PhoneNumberLvModel[]> = {
    en: toPhoneNumberLvModel(source['en'], 'en'),
    fi: toPhoneNumberLvModel(source['fi'], 'fi'),
    sv: toPhoneNumberLvModel(source['sv'], 'sv'),
    se: toPhoneNumberLvModel(source['se'], 'se'),
    smn: toPhoneNumberLvModel(source['smn'], 'smn'),
    sms: toPhoneNumberLvModel(source['sms'], 'sms'),
  };

  return result;
}

function toPhoneNumberLvModel(source: apimodels.PhoneNumberLv[] | undefined, language: enumTypes.Language): uimodels.PhoneNumberLvModel[] {
  if (!source) return [];

  const result: uimodels.PhoneNumberLvModel[] = [];

  for (const phoneNumber of source) {
    const { ...sameFields } = phoneNumber;

    result.push({
      ...sameFields,
      dialCodeType: phoneNumber.dialCodeId ? 'Normal' : 'NationalWithoutDialCode',
    });
  }

  return result;
}

function toAddressUiModel(source: apimodels.AddressModel): uimodels.AddressModel {
  const { streetName, ...sameFields } = source;

  const fallBack = getTextByLangPriority('fi', streetName, '');

  return {
    ...sameFields,
    streetName: toRequiredLocalizedText(streetName, fallBack),
    languageVersions: toAddressLanguageModels(source.languageVersions),
  };
}

function toAddressLanguageModels(
  source: LanguageVersionType<uimodels.AddressLvModel>
): RequiredLanguageVersionType<uimodels.AddressLvModel> {
  const result: RequiredLanguageVersionType<uimodels.AddressLvModel> = {
    en: toAddressLvModel(source['en'], 'en'),
    fi: toAddressLvModel(source['fi'], 'fi'),
    sv: toAddressLvModel(source['sv'], 'sv'),
    se: toAddressLvModel(source['se'], 'se'),
    smn: toAddressLvModel(source['smn'], 'smn'),
    sms: toAddressLvModel(source['sms'], 'sms'),
  };

  return result;
}

function toAddressLvModel(source: uimodels.AddressLvModel | undefined, language: enumTypes.Language): uimodels.AddressLvModel {
  return {
    additionalInformation: source?.additionalInformation ?? '',
    foreignAddress: source?.foreignAddress ?? '',
    language: language,
    poBox: source?.poBox ?? '',
  };
}

function openingHoursToUiModel(source: apimodels.OpeningHours): uimodels.OpeningHoursModel {
  return {
    standardHours: source.standardHours.map((x) => toStandardHourUiModel(x)),
    specialHours: source.specialHours.map((x) => toSpecialHourUiModel(x)),
    holidayHours: source.holidayHours,
    exceptionalHours: source.exceptionalHours.map((x) => toExceptionalHourUiModel(x)),
  };
}

function toExceptionalHourUiModel(source: apimodels.ExceptionalServiceHour): uimodels.ExceptionalServiceHourModel {
  const { ...sameFields } = source;

  return {
    ...sameFields,
    validityPeriod: source.openingHoursFrom && source.openingHoursTo ? 'BetweenDates' : 'Day',
  };
}

function toSpecialHourUiModel(source: apimodels.SpecialServiceHour): uimodels.SpecialServiceHourModel {
  const { ...sameFields } = source;

  return {
    ...sameFields,
    validityPeriod: source.isPeriod ? 'BetweenDates' : 'UntilFurtherNotice',
  };
}

function toStandardHourUiModel(source: apimodels.StandardServiceHour): uimodels.StandardServiceHourModel {
  const { ...sameFields } = source;

  return {
    ...sameFields,
    validityPeriod: source.isPeriod ? 'BetweenDates' : 'UntilFurtherNotice',
    openingType: toOpeningType(source),
    dailyOpeningTimes: toDailyOpeningTimes(source.dailyOpeningTimes),
  };
}

function toDailyOpeningTimes(source: apimodels.WeekdayMap<apimodels.DailyOpeningTime>): uimodels.DailyOpeningTimeModel[] {
  // For UI it makes it easier to have each day in place
  // and use the active flag to disable/enable days. Using array because of
  // https://github.com/react-hook-form/react-hook-form/discussions/5780
  const result: uimodels.DailyOpeningTimeModel[] = [];

  for (const day of enumTypes.weekDayType) {
    result.push(toDailyOpeningTime(source[day], day));
  }
  return result;
}

function toDailyOpeningTime(source: apimodels.DailyOpeningTime | null | undefined, day: enumTypes.Weekday): uimodels.DailyOpeningTimeModel {
  if (source) {
    return {
      day: source.day,
      times: source.times,
      active: true,
    };
  }

  return uimodels.createDailyOpeningTime(day);
}

function toOpeningType(source: apimodels.StandardServiceHour): uimodels.OpeningHourType | null {
  const days = Object.keys(source.dailyOpeningTimes);
  if (source.isNonStop) return 'Always';
  if (source.isPeriod || days.length > 0) return 'DaysAndTimes';
  if (source.isReservation) return 'OpenByReservation';
  return null;
}

function toRequiredLanguageVersion<T>(source: LanguageVersionType<T[]>): RequiredLanguageVersionType<T[]> {
  const result: RequiredLanguageVersionType<T[]> = {
    en: source['en'] ? source['en'] : [],
    fi: source['fi'] ? source['fi'] : [],
    sv: source['sv'] ? source['sv'] : [],
    se: source['se'] ? source['se'] : [],
    smn: source['smn'] ? source['smn'] : [],
    sms: source['sms'] ? source['sms'] : [],
  };
  return result;
}
