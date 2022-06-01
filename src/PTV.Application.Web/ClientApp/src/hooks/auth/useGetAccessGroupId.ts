import { AppEnvironment } from 'types/enumTypes';
import { usePublicEnumTypesQuery } from 'hooks/queries/usePublicEnumTypesQuery';

type useGetAccessGroupResult = {
  isLoading: boolean;
  error?: unknown;
  accessGroupId?: string | null;
};

export function useGetAccessGroupId(env: AppEnvironment): useGetAccessGroupResult {
  // Outside dev environment we don't pass access group id back to server
  // during login so there is no need to fetch access groups.
  const enumQuery = usePublicEnumTypesQuery({
    enabled: env === 'Dev',
  });

  if (enumQuery.isLoading || enumQuery.error) {
    return {
      isLoading: enumQuery.isLoading,
      error: enumQuery.error,
    };
  }

  const groups = enumQuery.data?.data?.UserAccessRightsGroups || [];
  const group = groups.find((x) => x.code === 'PTV_USER');

  return {
    isLoading: false,
    accessGroupId: group?.id,
  };
}
