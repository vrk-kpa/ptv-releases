import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { ServiceChannel } from 'types/api/serviceChannelModel';
import { HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

const apiPath = 'next/channel/latest-version';
const makeKey = (unificRootId: string, includeConnections: boolean) => [apiPath, unificRootId, includeConnections];

type Result = UseQueryResult<ServiceChannel, HttpError>;
type Options = UseQueryOptions<ServiceChannel, HttpError, ServiceChannel> | undefined;

function getByUnificRootId(unificRootId: string, includeConnections: boolean): Promise<ServiceChannel> {
  return get<ServiceChannel>(`${apiPath}?unificRootId=${unificRootId}&includeConnections=${includeConnections}`);
}

export function useGetPublishedChannelLatestVersionByUnificRootId(
  unificRootId: string,
  includeConnections: boolean,
  options?: Options | undefined
): Result {
  return useQuery<ServiceChannel, HttpError>(
    makeKey(unificRootId, includeConnections),
    () => getByUnificRootId(unificRootId, includeConnections),
    options
  );
}
