import { EnumItemType } from './enumItemType';
import { AreaInformationType, AreaType } from './enumTypes';
import { LocalizedText } from './miscellaneousTypes';
import { PostalCode } from './postalCodeTypes';

export type AreaInformationModel = {
  areaInformationType: AreaInformationType;
  areaTypes: AreaType[];
  businessRegions: string[];
  hospitalRegions: string[];
  municipalities: string[];
  provinces: string[];
};

export enum cAreaInformation {
  'areaInformationType' = 'areaInformationType',
  'areaTypes' = 'areaTypes',
  'businessRegions' = 'businessRegions',
  'hospitalRegions' = 'hospitalRegions',
  'municipalities' = 'municipalities',
  'provinces' = 'provinces',
}

type AreaModel = EnumItemType & {
  id: string;
  code: string;
  names: LocalizedText;
  descriptions: LocalizedText;
};

export type Municipality = AreaModel & {
  postalCodes: PostalCode[];
};

export type Region = AreaModel & {
  municipalities: Municipality[];
};

export type Country = {
  code: string;
  names: LocalizedText;
};
