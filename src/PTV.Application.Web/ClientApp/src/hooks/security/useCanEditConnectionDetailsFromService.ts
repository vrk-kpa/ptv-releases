import { useContext } from 'react';
import { PublishingStatus } from 'types/enumTypes';
import { OrganizationRoleModel } from 'types/organizationTypes';
import { UserInfo } from 'types/userInfoTypes';
import { AppContext } from 'context/AppContextProvider';
import { canAccess, canUpdate } from 'utils/security';

type ReturnValue = (serviceOrgId: string | null | undefined, serviceStatus: PublishingStatus) => boolean;

export function useCanEditConnectionDetailsFromService(): ReturnValue {
  const ctx = useContext(AppContext);

  return (...params) => {
    if (!ctx.userInfo || !canAccess(ctx.userInfo)) {
      return false;
    }
    return canEditConnectionDetailsFromService(ctx.userInfo, ...params, ctx.organizationRoles);
  };
}

function canEditConnectionDetailsFromService(
  userInfo: UserInfo,
  serviceOrgId: string | null | undefined,
  serviceStatus: PublishingStatus,
  organizationRoles: OrganizationRoleModel[]
): boolean {
  if (serviceStatus === 'Deleted' || serviceStatus === 'OldPublished' || serviceStatus === 'Removed') {
    return false;
  }

  return canUpdate(userInfo, 'relations', serviceOrgId, organizationRoles);
}
