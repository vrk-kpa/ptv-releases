import { useContext } from 'react';
import { AppContext } from 'context/AppContextProvider';
import { canAccess, canCreate } from 'utils/security';

type UseCanCreateServiceProps = {
  hasOtherModifiedVersion: boolean;
  responsibleOrgId: string | null | undefined;
};

export function useCanCreateService(props: UseCanCreateServiceProps): boolean {
  const ctx = useContext(AppContext);

  if (!ctx.userInfo) return false;
  if (!canAccess(ctx.userInfo)) return false;
  if (props.hasOtherModifiedVersion) return false;

  return canCreate(ctx.userInfo, 'services', props.responsibleOrgId, ctx.organizationRoles);
}
