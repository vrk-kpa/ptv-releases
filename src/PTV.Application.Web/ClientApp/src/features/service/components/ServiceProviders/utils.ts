import { OrganizationModel } from 'types/organizationTypes';

export function getDefaultSelfProducers(
  responsibleOrganization: OrganizationModel | null | undefined,
  otherResponsibleOrganizations: OrganizationModel[]
): OrganizationModel[] {
  if (responsibleOrganization && otherResponsibleOrganizations.length > 0) {
    return [...otherResponsibleOrganizations, responsibleOrganization].filter((x) => x !== null);
  }

  if (responsibleOrganization) {
    return [responsibleOrganization];
  }

  if (otherResponsibleOrganizations && otherResponsibleOrganizations.length > 0) {
    return otherResponsibleOrganizations;
  }

  return [];
}
