import { useContext } from 'react';
import { PublishingStatus } from 'types/enumTypes';
import { AppContext } from 'context/AppContextProvider';
import { canAccess, canPublish } from 'utils/security';

type CanPublishServiceProps = {
  status: PublishingStatus;
  hasOtherModifiedVersion: boolean;
  responsibleOrgId: string | null | undefined;
};

export function useCanPublishService(props: CanPublishServiceProps): boolean {
  const ctx = useContext(AppContext);

  if (!ctx.userInfo) return false;
  if (!canAccess(ctx.userInfo)) return false;

  if (props.status === 'Deleted' || props.status === 'OldPublished' || props.status === 'Removed') {
    return false;
  }

  if (props.hasOtherModifiedVersion) return false;

  return canPublish(ctx.userInfo, 'services', props.responsibleOrgId, ctx.organizationRoles);
}
