import { UseMutationResult, useMutation } from 'react-query';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { Language } from 'types/enumTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { post } from 'utils/request';

type OptionalLanguage = Language | null | undefined;

export const useRestoreService = (id: string): UseMutationResult<ServiceApiModel, HttpError, OptionalLanguage, unknown> => {
  const mutation = useMutation<ServiceApiModel, HttpError, OptionalLanguage, unknown>((language: OptionalLanguage) => {
    const url = !language ? `next/service/restore/${id}` : `next/service/restore/${id}?language=${language}`;
    return post<ServiceApiModel>(url);
  });

  return mutation;
};
