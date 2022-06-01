import { UseMutationResult, useMutation } from 'react-query';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { ValidationModel } from 'types/api/validationModel';
import { HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

export const useServiceValidation = (): UseMutationResult<ValidationModel<ServiceApiModel>, HttpError, ServiceApiModel, unknown> => {
  const mutation = useMutation<ValidationModel<ServiceApiModel>, HttpError, ServiceApiModel, unknown>((service: ServiceApiModel) => {
    return get<ValidationModel<ServiceApiModel>>(`next/service/validate/${service.id}`);
  });

  return mutation;
};
