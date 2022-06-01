import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { AnnotationToolRequest, OntologyTermsResponse } from 'types/annotationToolTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { post } from 'utils/request';

type Result = UseQueryResult<OntologyTermsResponse[], HttpError>;
type Options = UseQueryOptions<OntologyTermsResponse[], HttpError, OntologyTermsResponse[]> | undefined;

const getUseAnnotationToolQueryKey = (payload: AnnotationToolRequest | null) => [['ontology', 'annotations'], { payload }];

export const useAnnotationToolQuery = (payload: AnnotationToolRequest | null, options?: Options): Result => {
  return useQuery<OntologyTermsResponse[], HttpError>(
    getUseAnnotationToolQueryKey(payload),
    () =>
      post<OntologyTermsResponse[]>('next/ontology/annotations', {
        ...payload,
      }),
    options
  );
};
