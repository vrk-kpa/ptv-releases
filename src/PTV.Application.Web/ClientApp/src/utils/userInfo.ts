import { OrganizationModel } from 'types/organizationTypes';
import { IAppContext } from 'context/AppContextProvider';

export function getUserOrganization(context: IAppContext): OrganizationModel | null | undefined {
  return context.userOrganizations.find((x) => x.id === context.userInfo?.userOrganization);
}
