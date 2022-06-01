import { UseMutationResult, UseQueryOptions, UseQueryResult, useMutation, useQuery } from 'react-query';
import { GeneralDescriptionType, Language, ServiceType } from 'types/enumTypes';
import { HttpError, LocalizedText } from 'types/miscellaneousTypes';
import { post } from 'utils/request';

export type GdSearchQuery = {
  name: string;
  pageNumber: number;
  sortLanguage: Language;
  serviceType?: ServiceType | undefined;
  generalDescriptionType?: GeneralDescriptionType | undefined;
};

export type GdSearchResult = {
  count: number;
  pageNumber: number;
  maxPageCount: number;
  moreAvailable: boolean;
  skip: number;
  items: GdSearchItem[];
};

export type GdSearchItem = {
  id: string;
  unificRootId: string;
  serviceType: ServiceType;
  generalDescriptionType: GeneralDescriptionType;
  names: LocalizedText;
};

const apiPath = 'next/gd/search';
const makeKey = (params: GdSearchQuery): unknown[] => [apiPath, params];

function postSearch(params: GdSearchQuery): Promise<GdSearchResult> {
  return post<GdSearchResult>(apiPath, params);
}

type Result = UseQueryResult<GdSearchResult, HttpError>;
type Options = UseQueryOptions<GdSearchResult, HttpError, GdSearchResult>;

export const useSearchGeneralDescriptions = (parameters: GdSearchQuery, options?: Options): Result => {
  return useQuery<GdSearchResult, HttpError>(makeKey(parameters), () => postSearch(parameters), options);
};

export function useExecuteSearchGeneralDescriptions(): UseMutationResult<GdSearchResult, HttpError, GdSearchQuery, unknown> {
  const mutation = useMutation<GdSearchResult, HttpError, GdSearchQuery, unknown>((parameters: GdSearchQuery) => {
    return postSearch(parameters);
  });

  return mutation;
}
