import { useContext } from 'react';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { PublishingStatus } from 'types/enumTypes';
import { OrganizationRoleModel } from 'types/organizationTypes';
import { UserInfo } from 'types/userInfoTypes';
import { AppContext } from 'context/AppContextProvider';
import { canAccess, canCreate, canCreateOwn, isPtvAdmin } from 'utils/security';

type ReturnValue = (serviceOrgId: string | null | undefined, serviceStatus: PublishingStatus, channel: ConnectableChannel) => boolean;

export function useCanAddConnectionFromService(): ReturnValue {
  const ctx = useContext(AppContext);

  return (...params) => {
    if (!ctx.userInfo || !canAccess(ctx.userInfo)) {
      return false;
    }
    return canAddConnectionFromService(ctx.userInfo, ...params, ctx.organizationRoles);
  };
}

function canAddConnectionFromService(
  userInfo: UserInfo,
  serviceOrgId: string | null | undefined,
  serviceStatus: PublishingStatus,
  channel: ConnectableChannel,
  organizationRoles: OrganizationRoleModel[]
): boolean {
  if (serviceStatus === 'Deleted' || serviceStatus === 'OldPublished' || serviceStatus === 'Removed') {
    return false;
  }

  if (isPtvAdmin(userInfo)) {
    return canCreate(userInfo, 'relations', serviceOrgId, organizationRoles);
  }

  // Service and channel belong to user's organization
  if (userInfo.userOrganization === serviceOrgId && userInfo.userOrganization === channel.organization?.id) {
    return canCreateOwn(userInfo, 'relations');
  }

  // Service belongs to user's organization and channel is open
  if (userInfo.userOrganization === serviceOrgId) {
    if (channel.connectionType === 'CommonForAll') {
      return canCreateOwn(userInfo, 'relations');
    }
  }

  return false;
}
