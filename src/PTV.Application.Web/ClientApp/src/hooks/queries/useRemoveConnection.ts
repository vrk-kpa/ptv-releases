import { UseMutationOptions, UseMutationResult, useMutation } from 'react-query';
import { HttpError } from 'types/miscellaneousTypes';
import { httpDelete } from 'utils/request';

const apiPath = 'next/connection/remove-from-service-side';

type Result = UseMutationResult<unknown, HttpError, Parameters, unknown>;
type Options = UseMutationOptions<unknown, HttpError, Parameters> | undefined;

export type Parameters = {
  serviceId: string;
  serviceChannelUnificRootId: string;
};

export function useExecuteRemoveConnection(options?: Options | undefined): Result {
  const mutation = useMutation<unknown, HttpError, Parameters, unknown>((p: Parameters) => {
    const path = `${apiPath}?serviceId=${p.serviceId}&serviceChannelUnificRootId=${p.serviceChannelUnificRootId}`;
    return httpDelete(path);
  }, options);

  return mutation;
}
