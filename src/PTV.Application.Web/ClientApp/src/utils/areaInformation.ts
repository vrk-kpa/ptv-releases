import { AreaInformationModel } from 'types/areaTypes';

export function CreateEmptyAreaInformation(): AreaInformationModel {
  return {
    areaInformationType: 'WholeCountry',
    areaTypes: [],
    municipalities: [],
    provinces: [],
    businessRegions: [],
    hospitalRegions: [],
  };
}
