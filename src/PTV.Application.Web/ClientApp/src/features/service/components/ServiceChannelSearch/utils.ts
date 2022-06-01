import _ from 'lodash';
import { ConnectionApiModel } from 'types/api/connectionApiModel';
import { ChannelType, Language } from 'types/enumTypes';
import { ConnectionFormLvModel } from 'types/forms/connectionFormTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { ConnectableChannelSearchQuery, ConnectableChannelSearchResult, SortData } from 'hooks/queries/useGetConnectableChannels';
import { hasItems } from 'utils/languageVersions';

const PageSize = 20;

export type State = {
  enabled: boolean;
  searchText: string;
  channelType: ChannelType | undefined;
  pageNumber: number;
  organization: OrganizationModel | null | undefined;
  showLatestFirst: boolean;
  showSuggestedChannels: boolean;
  result: ConnectableChannelSearchResult | null | undefined;
};

export function getInitialState(organization: OrganizationModel | null | undefined): State {
  return {
    enabled: false,
    searchText: '',
    channelType: undefined,
    pageNumber: 0,
    organization: organization,
    showLatestFirst: false,
    showSuggestedChannels: false,
    result: undefined,
  };
}

export function getSearchParameters(parameters: State, serviceId: string, language: Language): ConnectableChannelSearchQuery {
  return {
    language: language,
    id: serviceId,
    organizationId: parameters.organization?.id,
    name: parameters.searchText,
    pageNumber: parameters.pageNumber,
    pageSize: PageSize,
    sortData: getSortData(parameters.showLatestFirst, parameters.showSuggestedChannels),
    type: 'Services',
    channelType: parameters.channelType,
  };
}

function getSortData(latestFirst: boolean, showSuggestedChannels: boolean): SortData[] {
  const data: SortData[] = [];

  if (latestFirst) {
    data.push({
      column: 'Modified',
      order: 1,
      sortDirection: 'Desc',
    });
  }

  if (showSuggestedChannels) {
    data.push({
      column: 'IsFromGD',
      order: 2,
      sortDirection: 'Desc',
    });
  }

  return data;
}

export function getNumberOfPages(searchResult: ConnectableChannelSearchResult | undefined | null): number {
  if (!searchResult) {
    return 0;
  }

  const perPage = searchResult.pageSize;
  const total = searchResult.count;
  if (perPage === 0 || total === 0) {
    return 0;
  }

  return Math.ceil(total / perPage);
}

export function getCurrentPageNumber(searchResult: ConnectableChannelSearchResult | undefined | null): number {
  if (searchResult?.pageNumber) {
    // +1 because the api returns zero for the first page unlike gd search which returns 1
    return searchResult.pageNumber + 1;
  }

  return 1;
}

export function channelLanguageVersionContainsData(languageVersion: ConnectionFormLvModel | undefined): boolean {
  return !_.isEmpty(languageVersion?.description) || !_.isEmpty(languageVersion?.charge.info);
}

export function channelConnectionContainsGeneralData(connection: ConnectionApiModel, lang: Language): boolean {
  return (
    hasItems(connection.emails, lang) ||
    hasItems(connection.phoneNumbers, lang) ||
    hasItems(connection.webPages, lang) ||
    hasItems(connection.faxNumbers, lang) ||
    connection.addresses.length > 0 ||
    connection.digitalAuthorizations.length > 0
  );
}
