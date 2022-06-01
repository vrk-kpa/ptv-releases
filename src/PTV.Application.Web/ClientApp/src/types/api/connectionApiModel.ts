import * as enumTypes from 'types/enumTypes';
import * as uimodels from 'types/forms/connectionFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { LinkModel } from 'types/link';
import { LocalizedText } from 'types/miscellaneousTypes';
import { StreetModel, StreetNumberModel } from './streetModel';

export type ConnectionApiModel = {
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
  languageVersions: LanguageVersionType<uimodels.ConnectionFormLvModel>;
  openingHours: OpeningHours;
  digitalAuthorizations: string[];
  emails: LanguageVersionType<uimodels.EmailLvModel[]>;
  webPages: LanguageVersionType<uimodels.WebPageLvModel[]>;
  faxNumbers: LanguageVersionType<uimodels.FaxNumberLvModel[]>;
  phoneNumbers: LanguageVersionType<PhoneNumberLv[]>;
  addresses: AddressModel[];
};

export type PhoneNumberLv = {
  id?: string;
  number: string;
  dialCodeId: string | null | undefined;
  additionalInformation: string;
  chargeDescription: string;
  chargeType: enumTypes.ChargeType;
  orderNumber: number;
  type: enumTypes.PhoneType;
};

export type OpeningHours = {
  standardHours: StandardServiceHour[];
  specialHours: SpecialServiceHour[];
  holidayHours: uimodels.HolidayServiceHourModel[];
  exceptionalHours: ExceptionalServiceHour[];
};

export type WeekdayMap<T> = {
  [weekday in enumTypes.Weekday]?: T;
};

export type SpecialServiceHour = {
  id?: string;
  openingHoursFrom: string | null;
  openingHoursTo: string | null;
  dayFrom: enumTypes.Weekday;
  dayTo: enumTypes.Weekday | null;
  timeFrom: string;
  timeTo: string;
  isPeriod: boolean;
  orderNumber: number;
  type: enumTypes.ServiceHourType;
  languageVersions: LanguageVersionType<uimodels.ServiceHourLvModel>;
};

export type ExceptionalServiceHour = {
  id?: string;
  openingHoursFrom: string | null;
  openingHoursTo: string | null;
  timeFrom: string;
  timeTo: string;
  type: enumTypes.ServiceHourType;
  isClosed: boolean;
  orderNumber: number;
  languageVersions: LanguageVersionType<uimodels.ServiceHourLvModel>;
};

export type StandardServiceHour = {
  id?: string;
  openingHoursFrom: string | null;
  openingHoursTo: string | null;
  isPeriod: boolean;
  type: enumTypes.ServiceHourType;
  isReservation: boolean;
  isNonStop: boolean;
  orderNumber: number;
  languageVersions: LanguageVersionType<uimodels.ServiceHourLvModel>;
  dailyOpeningTimes: WeekdayMap<DailyOpeningTime>;
};

export type DailyOpeningTime = {
  day: enumTypes.Weekday;
  times: FromTo[];
};

export type FromTo = {
  from: string;
  to: string;
};

export type ChargeApiModel = {
  info: string | null | undefined;
  hasLink: boolean;
  link: LinkModel;
};

export type AddressModel = {
  type: enumTypes.AddressType;
  character: enumTypes.AddressCharacterType;
  orderNumber: number;
  postalCode: string | null;
  countryCode: string | null;
  streetNumber: string;
  street: StreetModel | null;
  streetName: LocalizedText;
  streetNumberRange: StreetNumberModel | null;
  languageVersions: LanguageVersionType<uimodels.AddressLvModel>;
};
