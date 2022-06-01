import { UseMutationOptions, UseMutationResult, useMutation } from 'react-query';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { HttpError } from 'types/miscellaneousTypes';
import { post } from 'utils/request';

const apiPath = 'next/connection/connect-service-to-channels';

type Result = UseMutationResult<ConnectableChannel[], HttpError, Parameters, unknown>;
type Options = UseMutationOptions<ConnectableChannel[], HttpError, unknown | undefined>;

export type Parameters = {
  serviceId: string;
  serviceChannelUnificRootIds: string[];
};

export function useAddConnections(options?: Options | undefined): Result {
  const mutation = useMutation<ConnectableChannel[], HttpError, Parameters, unknown>((data: Parameters) => {
    return post<ConnectableChannel[]>(apiPath, data);
  }, options);

  return mutation;
}
