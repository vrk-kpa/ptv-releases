import { LocalizedText } from 'types/miscellaneousTypes';

export type StreetModel = {
  id?: string;
  names: LocalizedText;
  municipalityCode: string;
  isValid: boolean;
  streetNumbers: StreetNumberModel[];
};

export type StreetNumberModel = {
  id: string;
  startNumber: number;
  endNumber: number;
  isEven: boolean;
  postalCode: string;
  isValid: boolean;
  streetId: string;
};
