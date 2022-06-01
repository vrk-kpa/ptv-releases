import { LocalizedText } from './miscellaneousTypes';

export type EnumItemType = {
  id: string;
  code: string;
  names: LocalizedText;
};

export type DialCode = {
  id: string;
  countryCode: string;
  code: string;
};
