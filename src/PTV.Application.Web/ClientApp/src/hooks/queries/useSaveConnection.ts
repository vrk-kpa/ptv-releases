import { UseMutationOptions, UseMutationResult, useMutation } from 'react-query';
import { ConnectionApiModel } from 'types/api/connectionApiModel';
import { HttpError } from 'types/miscellaneousTypes';
import { post } from 'utils/request';

const apiPath = 'next/connection/save-connection';

type Result = UseMutationResult<ConnectionApiModel, HttpError, ConnectionApiModel, unknown>;
type Options = UseMutationOptions<ConnectionApiModel, HttpError, unknown | undefined>;

export function useSaveConnection(options?: Options | undefined): Result {
  const mutation = useMutation<ConnectionApiModel, HttpError, ConnectionApiModel, unknown>((data: ConnectionApiModel) => {
    return post<ConnectionApiModel>(apiPath, data);
  }, options);

  return mutation;
}
