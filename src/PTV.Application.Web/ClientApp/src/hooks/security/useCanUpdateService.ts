import { useContext } from 'react';
import { PublishingStatus } from 'types/enumTypes';
import { AppContext } from 'context/AppContextProvider';
import { canAccess, canUpdate } from 'utils/security';
import { isArchivedOrRemoved } from 'utils/status';

type CanUpdateResult = {
  // User can restore entity, archive entity, permanently remove it, revert to draft...
  canChangeServiceStatus: boolean;
  // User can update the content of the entity, add language versions, ...
  canEdit: boolean;
};

type UseCanUpdateServiceProps = {
  status: PublishingStatus;
  hasOtherModifiedVersion: boolean;
  responsibleOrgId: string | null | undefined;
};

export function useCanUpdateService(props: UseCanUpdateServiceProps): CanUpdateResult {
  const ctx = useContext(AppContext);

  if (!ctx.userInfo || !canAccess(ctx.userInfo) || props.hasOtherModifiedVersion) {
    return { canChangeServiceStatus: false, canEdit: false };
  }

  const isUpdateable = canUpdate(ctx.userInfo, 'services', props.responsibleOrgId, ctx.organizationRoles);

  return {
    canChangeServiceStatus: isUpdateable,
    canEdit: isUpdateable && !isArchivedOrRemoved(props.status),
  };
}
