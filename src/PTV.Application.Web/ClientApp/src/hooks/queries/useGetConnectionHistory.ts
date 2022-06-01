import { UseInfiniteQueryOptions, UseInfiniteQueryResult, useInfiniteQuery } from 'react-query';
import { ConnectionHistoryType } from 'types/api/connectionHistoryType';
import { InfiniteModel } from 'types/api/infiniteModel';
import { MainEntityType } from 'types/enumTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

export const useQueryKey = (entityType: MainEntityType, id: string): string[] => ['connectionHistoryQuery', entityType, id];

type Response = InfiniteModel<ConnectionHistoryType>;
type Options = UseInfiniteQueryOptions<Response, HttpError, Response> | undefined;

export const useGetConnectionHistory = (
  entityType: MainEntityType,
  id: string,
  options?: Options | undefined
): UseInfiniteQueryResult<Response, HttpError> => {
  return useInfiniteQuery<Response, HttpError>(
    useQueryKey(entityType, id),
    ({ pageParam = 0 }) => get<Response>(`next/${entityType}/connection-history/${id}/${pageParam}`),
    {
      ...options,
      getNextPageParam: (lastPage) => (lastPage.isMoreAvailable ? lastPage.page : undefined),
    }
  );
};
