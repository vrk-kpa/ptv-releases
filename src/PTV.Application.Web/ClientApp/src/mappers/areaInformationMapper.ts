import { AreaInformationModel } from 'types/areaTypes';

export function unsetAreasWithoutParents(input: AreaInformationModel): AreaInformationModel {
  return {
    areaInformationType: input.areaInformationType,
    areaTypes: input.areaTypes,
    businessRegions: input.areaTypes.includes('BusinessRegions') ? input.businessRegions : [],
    hospitalRegions: input.areaTypes.includes('HospitalRegions') ? input.hospitalRegions : [],
    municipalities: input.areaTypes.includes('Municipality') ? input.municipalities : [],
    provinces: input.areaTypes.includes('Province') ? input.provinces : [],
  };
}
