import { UseMutationResult, useMutation } from 'react-query';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { HttpError } from 'types/miscellaneousTypes';
import { post } from 'utils/request';

export const useSaveService = (): UseMutationResult<ServiceApiModel, HttpError, ServiceApiModel, unknown> => {
  const mutation = useMutation<ServiceApiModel, HttpError, ServiceApiModel, unknown>((parameters: ServiceApiModel) =>
    post<ServiceApiModel>('next/service/save', parameters)
  );
  return mutation;
};
