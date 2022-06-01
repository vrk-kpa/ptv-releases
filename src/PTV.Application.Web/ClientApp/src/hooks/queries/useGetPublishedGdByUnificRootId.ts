import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { GdApiModel } from 'types/api/gdApiModel';
import { HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

const apiPath = 'next/gd/published';
const makeKey = (unificRootId: string) => [apiPath, unificRootId];

type Result = UseQueryResult<GdApiModel, HttpError>;
type Options = UseQueryOptions<GdApiModel, HttpError, GdApiModel> | undefined;

function getById(id: string): Promise<GdApiModel> {
  return get<GdApiModel>(`${apiPath}?unificRootId=${id}`);
}

export function useGetPublishedGdByUnificRootId(unificRootId: string, options?: Options | undefined): Result {
  return useQuery<GdApiModel, HttpError>(makeKey(unificRootId), () => getById(unificRootId), options);
}
