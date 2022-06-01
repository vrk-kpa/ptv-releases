import { UseMutationResult, useMutation } from 'react-query';
import { PublishingCommandModel } from 'types/api/publishingType';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { HttpError } from 'types/miscellaneousTypes';
import { post } from 'utils/request';

export type PublishServiceParameters = {
  serviceId: string;
  data: PublishingCommandModel;
};

export const usePublishService = (): UseMutationResult<ServiceApiModel, HttpError, PublishServiceParameters, unknown> => {
  const mutation = useMutation<ServiceApiModel, HttpError, PublishServiceParameters, unknown>((parameters: PublishServiceParameters) =>
    post<ServiceApiModel>(`next/service/publish/${parameters.serviceId}`, parameters.data)
  );

  return mutation;
};
