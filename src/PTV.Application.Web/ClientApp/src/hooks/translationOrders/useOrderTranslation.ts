import { UseMutationResult, useMutation } from 'react-query';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { Language, MainEntityType } from 'types/enumTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { post } from 'utils/request';

export type TranslationOrder = {
  source: Language;
  targets: Language[];
  subscriber: string;
  email: string;
  additionalInformation: string | null | undefined;
};

export const useOrderTranslation = (
  id: string,
  entityType: MainEntityType
): UseMutationResult<ServiceApiModel, HttpError, TranslationOrder, unknown> => {
  const mutation = useMutation<ServiceApiModel, HttpError, TranslationOrder, unknown>((params: TranslationOrder) => {
    const url = `next/translation-orders/${entityType}/${id}`;
    return post<ServiceApiModel>(url, params);
  });

  return mutation;
};
