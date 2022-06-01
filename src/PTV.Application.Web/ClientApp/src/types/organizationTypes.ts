import { OrganizationType, PublishingStatus, UserRole } from './enumTypes';
import { LocalizedText } from './miscellaneousTypes';

export type OrganizationModel = {
  id: string;
  name: string;
  alternateName: string | null | undefined;
  code: string | null | undefined;
  publishingStatus: PublishingStatus;
  type: OrganizationType | null;
  texts: LocalizedText;
  versionedId: string;
};

export type OrganizationRoleModel = {
  isMain: boolean;
  organizationId: string;
  role: UserRole;
};

export type UserOrganizationsAndRolesResponse = {
  organizationRoles: OrganizationRoleModel[];
  userOrganizations: OrganizationModel[];
};
