import { ChargeApiModel } from 'types/api/chargeApiModel';
import { StreetModel, StreetNumberModel } from 'types/api/streetModel';
import * as enumTypes from 'types/enumTypes';
import { LanguageVersionType, RequiredLanguageVersionType } from 'types/languageVersionTypes';
import { RequiredLocalizedText } from 'types/miscellaneousTypes';
import { createDefaultEndTime, createDefaultStartTime } from 'utils/timespan';

export type ConnectionFormModel = {
  serviceId: string;
  serviceUnificRootId: string;
  serviceChannelUnificRootId: string;
  isASTIConnection: boolean;
  channelType: enumTypes.ChannelType;
  channelOrderNumber: number | null | undefined;
  serviceOrderNumber: number | null | undefined;
  modified: string;
  modifiedBy: string;
  chargeType: enumTypes.ChargeType | null | undefined;
  digitalAuthorizations: AuthorizationModel[];
  languageVersions: LanguageVersionType<ConnectionFormLvModel>;
  openingHours: OpeningHoursModel;
  // react hook form does not support optional languages https://github.com/react-hook-form/react-hook-form/discussions/5906
  emails: RequiredLanguageVersionType<EmailLvModel[]>;
  webPages: RequiredLanguageVersionType<WebPageLvModel[]>;
  faxNumbers: RequiredLanguageVersionType<FaxNumberLvModel[]>;
  phoneNumbers: RequiredLanguageVersionType<PhoneNumberLvModel[]>;
  addresses: AddressModel[];
};

export enum cC {
  chargeType = 'chargeType',
  openingHours = 'openingHours',
  standardOpeningHours = 'openingHours.standardHours',
  specialOpeningHours = 'openingHours.specialHours',
  holidayOpeningHours = 'openingHours.holidayHours',
  exceptionalOpeningHours = 'openingHours.exceptionalHours',
  validityPeriod = 'validityPeriod',
  languageVersions = 'languageVersions',
  emails = 'emails',
  webPages = 'webPages',
  faxNumbers = 'faxNumbers',
  phoneNumbers = 'phoneNumbers',
  addresses = 'addresses',
  digitalAuthorizations = 'digitalAuthorizations',
}

export type LanguageVersionExpanderTypes = 'BasicInfo' | 'ServiceHours' | 'ContactInformation' | 'Authorization';

export type StateLanguageVersionExpander = {
  [key in LanguageVersionExpanderTypes]: boolean;
};

// react hook form does not support binding to simple array of strings,
// it needs to be an object.
export type AuthorizationModel = {
  id: string;
};

export type ConnectionFormLvModel = {
  language: enumTypes.Language;
  description: string | null | undefined;
  charge: ChargeApiModel;
};

export enum cLv {
  description = 'description',
  chargeInfo = 'charge.info',
}

export enum cOh {
  standardHours = 'standardHours',
}

export type OpeningHoursModel = {
  standardHours: StandardServiceHourModel[];
  specialHours: SpecialServiceHourModel[];
  holidayHours: HolidayServiceHourModel[];
  exceptionalHours: ExceptionalServiceHourModel[];
};

export type ValidityPeriod = 'UntilFurtherNotice' | 'BetweenDates';

export const openingHourType = ['DaysAndTimes', 'Always', 'OpenByReservation'] as const;
export type OpeningHourType = typeof openingHourType[number];

export type ExceptionalHourValidityPeriod = 'Day' | 'BetweenDates';

export enum cHour {
  validityPeriod = 'validityPeriod',
  openingHoursFrom = 'openingHoursFrom',
  openingHoursTo = 'openingHoursTo',
  openingType = 'openingType',
  dailyOpeningTimes = 'dailyOpeningTimes',
  isReservation = 'isReservation',
  from = 'from',
  to = 'to',
  dayFrom = 'dayFrom',
  dayTo = 'dayTo',
  timeFrom = 'timeFrom',
  timeTo = 'timeTo',
  isClosed = 'isClosed',
}

export type ExceptionalServiceHourModel = {
  id?: string;
  validityPeriod: ExceptionalHourValidityPeriod | null;
  openingHoursFrom: string | null;
  openingHoursTo: string | null;
  timeFrom: string;
  timeTo: string;
  type: enumTypes.ServiceHourType;
  isClosed: boolean;
  orderNumber: number;
  languageVersions: LanguageVersionType<ServiceHourLvModel>;
};

export type HolidayServiceHourModel = {
  id?: string;
  code: enumTypes.HolidayEnum;
  type: enumTypes.ServiceHourType;
  isClosed: boolean;
  from: string;
  to: string;
};

export type SpecialServiceHourModel = {
  id?: string;
  validityPeriod: ValidityPeriod;
  openingHoursFrom: string | null;
  openingHoursTo: string | null;
  dayFrom: enumTypes.Weekday;
  dayTo: enumTypes.Weekday | null;
  timeFrom: string;
  timeTo: string;
  orderNumber: number;
  type: enumTypes.ServiceHourType;
  languageVersions: LanguageVersionType<ServiceHourLvModel>;
};

export type StandardServiceHourModel = {
  id?: string;
  openingHoursFrom: string | null;
  openingHoursTo: string | null;
  validityPeriod: ValidityPeriod;
  openingType: OpeningHourType | null;
  type: enumTypes.ServiceHourType;
  isReservation: boolean;
  dailyOpeningTimes: DailyOpeningTimeModel[];
  languageVersions: LanguageVersionType<ServiceHourLvModel>;
};

export type DailyOpeningTimeModel = {
  active: boolean;
  day: enumTypes.Weekday;
  times: TimeRangeModel[];
};

export enum cDaily {
  active = 'active',
  times = 'times',
  day = 'day',
}

export enum cTimeRange {
  from = 'from',
  to = 'to',
}

export type TimeRangeModel = {
  from: string;
  to: string;
};

export type ServiceHourLvModel = {
  language: enumTypes.Language;
  additionalInformation: string;
};

export enum cHourLv {
  additionalInformation = 'additionalInformation',
}

export enum cFaxNumberLv {
  number = 'number',
}

export type FaxNumberLvModel = {
  id?: string;
  number: string;
  dialCodeId: string | null | undefined;
  orderNumber: number;
  type: enumTypes.PhoneType;
};

export enum cPhoneNumberLv {
  number = 'number',
  additionalInformation = 'additionalInformation',
  chargeDescription = 'chargeDescription',
  chargeType = 'chargeType',
  isLocalNumber = 'isLocalNumber',
  dialCodeType = 'dialCodeType',
  dialCodeId = 'dialCodeId',
}

export type PhoneNumberLvModel = {
  id?: string;
  number: string;
  dialCodeId: string | null | undefined;
  additionalInformation: string;
  chargeDescription: string;
  chargeType: enumTypes.ChargeType;
  orderNumber: number;
  type: enumTypes.PhoneType;
  dialCodeType: enumTypes.PhoneNumberDialCodeType;
};

export type EmailLvModel = {
  id?: string;
  orderNumber: number;
  description: string;
  value: string;
};

export enum cEmailLv {
  value = 'value',
}

export enum cWebPageLv {
  name = 'name',
  url = 'url',
  additionalInformation = 'additionalInformation',
}

export type WebPageLvModel = {
  id?: string;
  name: string;
  url: string;
  additionalInformation: string;
  orderNumber: number;
};

export enum cAddress {
  type = 'type',
  street = 'street',
  streetName = 'streetName',
  languageVersions = 'languageVersions',
  streetNumber = 'streetNumber',
  postalCode = 'postalCode',
  countryCode = 'countryCode',
}

export type AddressModel = {
  type: enumTypes.AddressType;
  character: enumTypes.AddressCharacterType;
  orderNumber: number;
  postalCode: string | null;
  countryCode: string | null;
  streetNumber: string;
  street: StreetModel | null;
  streetName: RequiredLocalizedText;
  streetNumberRange: StreetNumberModel | null;
  languageVersions: RequiredLanguageVersionType<AddressLvModel>;
};

export enum cAddLv {
  additionalInformation = 'additionalInformation',
  poBox = 'poBox',
  foreignAddress = 'foreignAddress',
}

export type AddressLvModel = {
  language: enumTypes.Language;
  additionalInformation: string;
  poBox: string;
  foreignAddress: string;
};

export function createAddress(languages: enumTypes.Language[]): AddressModel {
  const lvs: RequiredLanguageVersionType<AddressLvModel> = {
    en: createAddressLv('en'),
    fi: createAddressLv('fi'),
    se: createAddressLv('se'),
    sv: createAddressLv('sv'),
    smn: createAddressLv('smn'),
    sms: createAddressLv('sms'),
  };

  const streetName = Object.assign({}, ...languages.map((l) => ({ [l]: '' })));

  return {
    type: 'Street',
    character: 'Postal',
    streetNumber: '',
    orderNumber: 0,
    languageVersions: lvs,
    countryCode: null,
    postalCode: null,
    street: null,
    streetName: streetName,
    streetNumberRange: null,
  };
}

function createAddressLv(language: enumTypes.Language): AddressLvModel {
  return {
    language: language,
    additionalInformation: '',
    foreignAddress: '',
    poBox: '',
  };
}

export function createStandardServiceHour(languages: enumTypes.Language[]): StandardServiceHourModel {
  const lvs = Object.assign({}, ...languages.map((l) => ({ [l]: createHourLvModel(l) })));

  return {
    type: 'Standard',
    validityPeriod: 'UntilFurtherNotice',
    openingType: null,
    isReservation: false,
    openingHoursFrom: null,
    openingHoursTo: null,
    languageVersions: lvs,
    dailyOpeningTimes: createDailyOpeningTimes(),
  };
}

function createHourLvModel(language: enumTypes.Language): ServiceHourLvModel {
  return {
    language: language,
    additionalInformation: '',
  };
}

function createDailyOpeningTimes(): DailyOpeningTimeModel[] {
  const result: DailyOpeningTimeModel[] = [];

  for (const day of enumTypes.weekDayType) {
    result.push(createDailyOpeningTime(day));
  }
  return result;
}

export function createDailyOpeningTime(day: enumTypes.Weekday): DailyOpeningTimeModel {
  return {
    day: day,
    times: [],
    active: false,
  };
}

export function createTimeRangeModel(): TimeRangeModel {
  return {
    from: createDefaultStartTime().toString(),
    to: createDefaultEndTime().toString(),
  };
}

export function createSpecialServiceHour(languages: enumTypes.Language[]): SpecialServiceHourModel {
  const lvs = Object.assign({}, ...languages.map((l) => ({ [l]: createHourLvModel(l) })));

  return {
    type: 'Special',
    validityPeriod: 'UntilFurtherNotice',
    openingHoursFrom: null,
    openingHoursTo: null,
    languageVersions: lvs,
    dayFrom: 'Monday',
    dayTo: 'Sunday',
    timeFrom: createDefaultStartTime().toString(),
    timeTo: createDefaultEndTime().toString(),
    orderNumber: 0,
  };
}

export function createHolidayServiceHour(holiday: enumTypes.HolidayEnum): HolidayServiceHourModel {
  return {
    isClosed: false,
    type: 'Exception',
    code: holiday,
    from: createDefaultStartTime().toString(),
    to: createDefaultEndTime().toString(),
  };
}

export function createExceptionalServiceHour(languages: enumTypes.Language[]): ExceptionalServiceHourModel {
  const lvs = Object.assign({}, ...languages.map((l) => ({ [l]: createHourLvModel(l) })));
  return {
    isClosed: true,
    languageVersions: lvs,
    openingHoursFrom: null,
    openingHoursTo: null,
    orderNumber: 0,
    type: 'Exception',
    validityPeriod: null,
    timeFrom: createDefaultStartTime().toString(),
    timeTo: createDefaultEndTime().toString(),
  };
}

export function createEmailAddress(): EmailLvModel {
  return {
    description: '',
    orderNumber: 0,
    value: '',
  };
}

export function createWebPage(): WebPageLvModel {
  return {
    additionalInformation: '',
    name: '',
    url: '',
    orderNumber: 0,
  };
}

export function createFaxNumber(dialCodeId: string | null | undefined): FaxNumberLvModel {
  return {
    number: '',
    dialCodeId: dialCodeId,
    type: 'Fax',
    orderNumber: 0,
  };
}

export function createPhoneNumber(dialCodeId: string | null | undefined): PhoneNumberLvModel {
  return {
    number: '',
    dialCodeId: dialCodeId,
    dialCodeType: 'Normal',
    type: 'Phone',
    orderNumber: 0,
    additionalInformation: '',
    chargeDescription: '',
    chargeType: 'Free',
  };
}
