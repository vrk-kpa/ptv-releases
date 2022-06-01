import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { TranslationDetailApiType } from 'types/api/translationApiTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

const getQueryKey = (id: string): string[] => ['translationOrderQuery', id];

type Options = UseQueryOptions<TranslationDetailApiType, HttpError, TranslationDetailApiType> | undefined;

export const useGetTranslationOrder = (id: string, options?: Options): UseQueryResult<TranslationDetailApiType, HttpError> => {
  return useQuery<TranslationDetailApiType, HttpError>(
    getQueryKey(id),
    () => get<TranslationDetailApiType>(`next/translation-orders/${id}`),
    options
  );
};
