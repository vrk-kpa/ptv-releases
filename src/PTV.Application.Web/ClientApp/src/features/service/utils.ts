import { BusinessTargetGroup, CitizenTargetGroup } from 'types/constants';
import { AreaInformationType, AreaType, ServiceType, VoucherType } from 'types/enumTypes';
import { ServiceModelLangaugeVersionsValuesType } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';

export function isGeneralDescriptionSelected(value?: GeneralDescriptionModel): boolean {
  return !!value;
}

export function isPermissionOrQualification(value: ServiceType): boolean {
  return value === 'PermissionAndObligation' || value === 'ProfessionalQualifications';
}

export function isServiceVoucherNoUrl(value: VoucherType): boolean {
  return value === 'NoUrl';
}

export function isServiceVoucherWithUrl(value: VoucherType): boolean {
  return value === 'Url';
}

export function isAreaInformationTypeAreaType(values: AreaInformationType): boolean {
  return values.includes('AreaType');
}

export function isAreaTypeMunicipality(values: AreaType[]): boolean {
  return values.includes('Municipality');
}

export function isAreaTypeProvince(values: AreaType[]): boolean {
  return values.includes('Province');
}

export function isAreaTypeBusinessRegions(values: AreaType[]): boolean {
  return values.includes('BusinessRegions');
}

export function isAreaTypeHospitalRegions(values: AreaType[]): boolean {
  return values.includes('HospitalRegions');
}

export function containsAnyCitizenTargetGroup(targetGroups: string[]): boolean {
  return hasTargetGroupThatStartsWith(CitizenTargetGroup, targetGroups);
}

export function containsAnyBusinessTargetGroup(targetGroups: string[]): boolean {
  return hasTargetGroupThatStartsWith(BusinessTargetGroup, targetGroups);
}

function hasTargetGroupThatStartsWith(targetGroupCode: string, targetGroups: string[]): boolean {
  return targetGroups.find((x) => x.startsWith(targetGroupCode)) !== undefined;
}

export function anyLanguageInTranslation(languageVersions: ServiceModelLangaugeVersionsValuesType): boolean {
  return Object.values(languageVersions).some((languageVersion) => languageVersion.translationAvailability?.isInTranslation === true);
}
