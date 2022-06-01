import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { ApiResponseWrapper, HttpError } from 'types/miscellaneousTypes';
import { UserInfo } from 'types/userInfoTypes';
import { post } from 'utils/request';

const makeKey = () => 'commonGetUserInfo';

type Result = UseQueryResult<ApiResponseWrapper<UserInfo>, HttpError>;
type Options = UseQueryOptions<ApiResponseWrapper<UserInfo>, HttpError, ApiResponseWrapper<UserInfo>> | undefined;

export const useGetUserInfo = (options?: Options | undefined): Result => {
  return useQuery<ApiResponseWrapper<UserInfo>, HttpError>(
    makeKey(),
    () => {
      return post<ApiResponseWrapper<UserInfo>>('common/GetUserInfo');
    },
    {
      ...options,
      // Do not cache anything as she could end up with wrong
      // roles when she logs out and logs in using different role
      cacheTime: 0,
    }
  );
};
