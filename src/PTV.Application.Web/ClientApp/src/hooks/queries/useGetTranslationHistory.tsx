import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { TranslationHistoryApiType } from 'types/api/translationApiTypes';
import { Language, MainEntityType } from 'types/enumTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

export const useQueryKey = (entityType: MainEntityType, id: string, language: Language): string[] => [
  'translationHistoryQuery',
  entityType,
  id,
  language,
];

type Response = TranslationHistoryApiType[];
type Options = UseQueryOptions<Response, HttpError, Response> | undefined;

export const useGetTranslationHistory = (
  entityType: MainEntityType,
  id: string,
  language: Language,
  options?: Options
): UseQueryResult<Response, HttpError> => {
  return useQuery<Response, HttpError>(
    useQueryKey(entityType, id, language),
    () => get<Response>(`next/translation-orders/${entityType}/${id}/${language}`),
    options
  );
};
