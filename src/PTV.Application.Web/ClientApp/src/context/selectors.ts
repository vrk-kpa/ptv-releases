import { OrganizationModel } from 'types/organizationTypes';
import { IAppContext } from 'context/AppContextProvider';

export function getUserMainOrganization(context: IAppContext): OrganizationModel | undefined {
  const mainOrgRole = context.organizationRoles.find((x) => x.isMain);
  if (!mainOrgRole) {
    return undefined;
  }

  return context.userOrganizations.find((x) => x.id === mainOrgRole.organizationId);
}
