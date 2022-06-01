import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { HttpError, LocalizedText } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

export type StreetSearchModelQuery = {
  searchText: string;
  onlyValid?: boolean;
};

export type StreetNameResult = {
  items: LocalizedText[];
};

const apiPath = 'next/address/search-street-names';
const makeKey = (params: StreetSearchModelQuery): unknown[] => [apiPath, params];

function getSearch(params: StreetSearchModelQuery): Promise<StreetNameResult> {
  const queryString = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    queryString.append(key, value.toString());
  });
  return get<StreetNameResult>(`${apiPath}?${queryString.toString()}`);
}

type Result = UseQueryResult<StreetNameResult, HttpError>;
type Options = UseQueryOptions<StreetNameResult, HttpError, StreetNameResult>;

export const useGetStreetNames = (parameters: StreetSearchModelQuery, options?: Options): Result => {
  return useQuery<StreetNameResult, HttpError>(makeKey(parameters), () => getSearch(parameters), options);
};

export const getAutocompleteQueryParams = (searchText: string): StreetSearchModelQuery => {
  return {
    searchText: searchText,
    onlyValid: true,
  };
};
