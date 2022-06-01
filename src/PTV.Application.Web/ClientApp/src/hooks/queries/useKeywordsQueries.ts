import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { OntologyTermsListRequest, OntologyTermsResponse } from 'types/annotationToolTypes';
import { OntologyTermTreeType } from 'types/classificationItemsTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { get, post } from 'utils/request';

type TermListResult = UseQueryResult<OntologyTermsResponse[], HttpError>;
type TermListOptions = UseQueryOptions<OntologyTermsResponse[], HttpError, OntologyTermsResponse[]> | undefined;

const getUseFetchTermsListQueryKey = (payload: OntologyTermsListRequest | null) => ['ontologyTermsList', { payload }];

export const useFetchTermsListQuery = (payload: OntologyTermsListRequest, options: TermListOptions): TermListResult => {
  return useQuery<OntologyTermsResponse[], HttpError>(
    getUseFetchTermsListQueryKey(payload),
    () => get<OntologyTermsResponse[]>(`next/ontology/search?expression=${payload.searchValue}&language=${payload.language}`),
    options
  );
};

type TermTreeResult = UseQueryResult<OntologyTermTreeType[], HttpError>;
type TermTreeOptions = UseQueryOptions<OntologyTermTreeType[], HttpError, OntologyTermTreeType[]> | undefined;

const getUseFetchTermTreeQueryKey = (ids: string[]) => ['ontologyTermTree', ids];

export const useFetchTermTreeQuery = (ids: string[], options: TermTreeOptions): TermTreeResult => {
  return useQuery<OntologyTermTreeType[], HttpError>(
    getUseFetchTermTreeQueryKey(ids),
    () => post<OntologyTermTreeType[]>(`next/ontology/getByIds`, ids),
    options
  );
};
