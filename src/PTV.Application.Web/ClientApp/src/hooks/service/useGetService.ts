import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

export const useServiceQueryKey = (id: string): string[] => ['serviceQuery', id];

type GetServiceOptions = UseQueryOptions<ServiceApiModel, HttpError, ServiceApiModel> | undefined;

export const useGetService = (id: string, options?: GetServiceOptions): UseQueryResult<ServiceApiModel, HttpError> => {
  return useQuery<ServiceApiModel, HttpError>(useServiceQueryKey(id), () => get<ServiceApiModel>(`next/service/${id}`), options);
};
