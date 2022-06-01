import { UseInfiniteQueryOptions, UseInfiniteQueryResult, useInfiniteQuery } from 'react-query';
import { EntityHistoryType } from 'types/api/entityHistoryType';
import { InfiniteModel } from 'types/api/infiniteModel';
import { MainEntityType } from 'types/enumTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

export const makeQueryKey = (entityType: MainEntityType, id: string): string[] => ['editHistoryQuery', entityType, id];

type Response = InfiniteModel<EntityHistoryType>;
type Options = UseInfiniteQueryOptions<Response, HttpError, Response> | undefined;

export const useGetEditHistory = (
  entityType: MainEntityType,
  id: string,
  options?: Options | undefined
): UseInfiniteQueryResult<Response, HttpError> => {
  return useInfiniteQuery<Response, HttpError>(
    makeQueryKey(entityType, id),
    ({ pageParam = 0 }) => get<Response>(`next/${entityType}/edit-history/${id}/${pageParam}`),
    {
      ...options,
      getNextPageParam: (lastPage) => (lastPage.isMoreAvailable ? lastPage.page : undefined),
    }
  );
};
