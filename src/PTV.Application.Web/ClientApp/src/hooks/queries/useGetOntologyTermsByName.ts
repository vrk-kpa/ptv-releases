import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { OntologyTermsResponse } from 'types/annotationToolTypes';
import { Language } from 'types/enumTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

type Result = UseQueryResult<OntologyTermsResponse[], HttpError>;
type Options = UseQueryOptions<OntologyTermsResponse[], HttpError, OntologyTermsResponse[]> | undefined;

type SearchParameters = {
  name: string;
  language: Language;
};

const getQueryKey = (payload: SearchParameters) => ['ontology/search-by-name', { payload }];

export const useGetOntologyTermsByName = (params: SearchParameters, options?: Options): Result => {
  const queryString = new URLSearchParams(params);
  return useQuery<OntologyTermsResponse[], HttpError>(
    getQueryKey(params),
    () => get<OntologyTermsResponse[]>(`next/ontology/search-by-name?${queryString.toString()}`),
    options
  );
};
