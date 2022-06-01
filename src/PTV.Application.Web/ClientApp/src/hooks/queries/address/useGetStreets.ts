import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { StreetModel } from 'types/api/streetModel';
import { Language } from 'types/enumTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

export type StreetSearchModelQuery = {
  searchText: string;
  postalCode: string;
  language: Language;
  offset: number;
  onlyValid?: boolean;
};

export type StreetSearchResult = {
  maxPageCount: number;
  pageNumber: number;
  moreAvailable: boolean;
  skip: number;
  items: StreetModel[];
};

const apiPath = 'next/address/search-street-names';
const makeKey = (params: StreetSearchModelQuery): unknown[] => [apiPath, params];

function getSearch(params: StreetSearchModelQuery): Promise<StreetSearchResult> {
  const queryString = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    queryString.append(key, value.toString());
  });
  return get<StreetSearchResult>(`${apiPath}?${queryString.toString()}`);
}

type Result = UseQueryResult<StreetSearchResult, HttpError>;
type Options = UseQueryOptions<StreetSearchResult, HttpError, StreetSearchResult>;

export const useGetStreets = (parameters: StreetSearchModelQuery, options?: Options): Result => {
  return useQuery<StreetSearchResult, HttpError>(makeKey(parameters), () => getSearch(parameters), options);
};
