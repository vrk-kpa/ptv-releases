import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { ChannelType, DomainEnumType, Language } from 'types/enumTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { post } from 'utils/request';

export type ConnectableChannelSearchQuery = {
  language: Language;
  id?: string | null | undefined;
  organizationId?: string | null | undefined;
  name: string;
  pageNumber: number;
  pageSize: number;
  sortData: SortData[];
  type: DomainEnumType;
  channelType: ChannelType | null | undefined;
};

export type SortData = {
  column: string;
  order: number;
  sortDirection: 'Asc' | 'Desc';
};

export type ConnectableChannelSearchResult = {
  count: number;
  pageNumber: number;
  pageSize: number;
  maxPageCount: number;
  moreAvailable: boolean;
  skip: number;
  items: ConnectableChannel[];
};

const apiPath = 'next/channel/connectable-channels';
const makeKey = (params: ConnectableChannelSearchQuery): unknown[] => [apiPath, params];

function postSearch(params: ConnectableChannelSearchQuery): Promise<ConnectableChannelSearchResult> {
  return post<ConnectableChannelSearchResult>(apiPath, params);
}

type Result = UseQueryResult<ConnectableChannelSearchResult, HttpError>;
type Options = UseQueryOptions<ConnectableChannelSearchResult, HttpError, ConnectableChannelSearchResult>;

export const useGetConnectableChannels = (parameters: ConnectableChannelSearchQuery, options?: Options): Result => {
  return useQuery<ConnectableChannelSearchResult, HttpError>(makeKey(parameters), () => postSearch(parameters), options);
};
