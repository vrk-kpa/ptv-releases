import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { ConnectionApiModel } from 'types/api/connectionApiModel';
import { HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

const path = 'next/connection/for-service';
const makeKey = (serviceId: string, serviceChannelUnificRootId: string): unknown[] => [path, serviceId, serviceChannelUnificRootId];

type Result = UseQueryResult<ConnectionApiModel, HttpError>;
type Options = UseQueryOptions<ConnectionApiModel, HttpError, ConnectionApiModel> | undefined;

export const useGetConnectionForService = (
  serviceId: string,
  serviceChannelUnificRootId: string,
  options?: Options | undefined
): Result => {
  return useQuery<ConnectionApiModel, HttpError>(
    makeKey(serviceId, serviceChannelUnificRootId),
    () => {
      return get<ConnectionApiModel>(`${path}?serviceId=${serviceId}&serviceChannelUnificRootId=${serviceChannelUnificRootId}`);
    },
    options
  );
};
