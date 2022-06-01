import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { UserAccessRightsGroupData } from 'types/loginTypes';
import { ApiResponseWrapper, HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

type PublicEnumTypesQueryResult = UseQueryResult<ApiResponseWrapper<UserAccessRightsGroupData>, HttpError>;

type Options = UseQueryOptions<ApiResponseWrapper<UserAccessRightsGroupData>, HttpError, ApiResponseWrapper<UserAccessRightsGroupData>>;

export const usePublicEnumTypesQuery = (options?: Options | undefined): PublicEnumTypesQueryResult => {
  return useQuery<ApiResponseWrapper<UserAccessRightsGroupData>, HttpError>(
    'publicDataGetEnumTypes',
    () => get<ApiResponseWrapper<UserAccessRightsGroupData>>('publicData/GetEnumTypes'),
    options
  );
};
