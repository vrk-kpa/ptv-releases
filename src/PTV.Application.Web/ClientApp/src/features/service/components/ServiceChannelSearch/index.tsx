import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import Grid from '@mui/material/Grid';
import { makeStyles } from '@mui/styles';
import { Button } from 'suomifi-ui-components';
import { valueOrDefault } from 'utils';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { ChannelType, PublishingStatus } from 'types/enumTypes';
import { OrganizationModel } from 'types/organizationTypes';
import Paginator from 'components/Paginator';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { useGetConnectableChannels } from 'hooks/queries/useGetConnectableChannels';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getUserOrganization } from 'utils/userInfo';
import OrganizationSelector from './OrganizationSelector';
import SearchBox from './SearchBox';
import SearchCount from './SearchCount';
import SearchResults from './SearchResults';
import ServiceChannelTypeSelector from './ServiceChannelTypeSelector';
import { State } from './utils';
import * as utils from './utils';

const useStyles = makeStyles(() => ({
  searchResults: {
    paddingTop: '10px',
  },
  description: {
    marginBottom: '17px',
  },
  item: {
    marginBottom: '8px',
  },
  searchBox: {
    marginBottom: '8px',
    marginRight: '8px',
  },
  serviceType: {
    marginBottom: '8px',
    marginRight: '8px',
  },
  org: {
    marginBottom: '8px',
    maxWidth: '245px',
  },
  searchParameters: {
    paddingLeft: '10px',
    paddingRight: '10px',
    backgroundColor: 'rgb(247, 247, 248)',
    borderTop: '1px solid rgb(200, 205, 208)',
    borderLeft: '1px solid rgb(200, 205, 208)',
    borderRight: '1px solid rgb(200, 205, 208)',
  },
  searchFunctions: {
    paddingBottom: '18px',
    paddingTop: '10px',
    backgroundColor: 'rgb(247, 247, 248)',
    borderLeft: '1px solid rgb(200, 205, 208)',
    borderRight: '1px solid rgb(200, 205, 208)',
  },
  searchButton: {
    marginRight: '15px',
    marginLeft: '10px',
  },
  searchCount: {
    paddingLeft: '15px',
    paddingTop: '10px',
    paddingBottom: '10px',
    backgroundColor: 'rgb(247, 247, 248)',
    border: '1px solid rgb(200, 205, 208)',
  },
  paginator: {
    marginTop: '20px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  },
}));

export type ServiceChannelSearchProps = {
  serviceId: string | null | undefined;
  serviceStatus: PublishingStatus;
  serviceResponsibleOrgId: string | null | undefined;
  selectedChannels: ConnectableChannel[];
  suggestedChannels: string[];
  serviceHasGeneralDescription: boolean;
  toggleSelectChannel: (channel: ConnectableChannel) => void;
};

export default function ServiceChannelSearch(props: ServiceChannelSearchProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const lang = useGetUiLanguage();
  const appContext = useAppContextOrThrow();

  const userOrganization = getUserOrganization(appContext);

  const initialState = utils.getInitialState(userOrganization);
  const [state, setState] = useState<State>(initialState);

  const params = utils.getSearchParameters(state, props.serviceId || '', lang);
  const query = useGetConnectableChannels(params, {
    enabled: state.enabled,
  });

  function onChannelTypeChange(newValue: ChannelType | undefined) {
    setState((previous) => ({ ...previous, channelType: newValue }));
  }

  function onOrganizationChange(org: OrganizationModel | null | undefined) {
    setState((previous) => ({ ...previous, organization: org }));
  }

  function onShowSuggestedChannelsChange(value: boolean) {
    setState((previous) => ({ ...previous, showSuggestedChannels: value }));
  }

  function onShowLatestFirstChange(value: boolean) {
    setState((previous) => ({ ...previous, showLatestFirst: value }));
  }

  function onResetSearchState() {
    setState(utils.getInitialState(userOrganization));
  }

  function onSearchTextChange(value: string) {
    setState((previous) => ({ ...previous, searchText: value }));
  }

  function onSearch(pageNumber = 0) {
    setState((previous) => ({ ...previous, enabled: true, pageNumber: pageNumber }));
  }

  function goToPage(page: number) {
    // Pagination works with pages 1...n but api with pages from 0...n
    onSearch(page - 1);
  }

  if (query.data && !query.error && state.enabled) {
    // Store the result in local state. Otherwise when we disabled the query,
    // react-query sets query state to idle and data is gone.
    setState((previous) => ({ ...previous, enabled: false, result: query.data }));
  }

  const count = valueOrDefault(state.result?.count, 0);
  const items = valueOrDefault(state.result?.items, []);
  const pageCount = utils.getNumberOfPages(state.result);
  const currentPage = utils.getCurrentPageNumber(state.result);

  return (
    <div>
      <div>
        <Grid className={classes.searchParameters} container spacing={1}>
          <Grid item xs={12} sm={12}>
            <SearchBox value={state.searchText} onChange={onSearchTextChange} />
          </Grid>
          <Grid item xs={12} sm={12} md={5}>
            <ServiceChannelTypeSelector channelType={state.channelType} onChange={onChannelTypeChange} />
          </Grid>
          <Grid item xs={12} sm={12} md={7}>
            <OrganizationSelector onSelect={onOrganizationChange} selected={state.organization} />
          </Grid>
        </Grid>
        <Grid className={classes.searchFunctions} container spacing={1}>
          <Grid item className={classes.searchButton}>
            <Button onClick={() => onSearch()} key='search'>
              {t('Ptv.Service.Form.ServiceChannelSearch.Search.Button.Label')}
            </Button>
          </Grid>
          <Grid item>
            <Button onClick={onResetSearchState} icon='remove' key='remove' variant='secondaryNoBorder'>
              {t('Ptv.Service.Form.ServiceChannelSearch.Reset.Button.Label')}
            </Button>
          </Grid>
        </Grid>
        <Grid container direction='column' spacing={1}>
          <Grid item className={classes.searchCount}>
            <SearchCount
              showLatestFirst={state.showLatestFirst}
              showSuggestedChannels={state.showSuggestedChannels}
              onShowLatestFirst={onShowLatestFirstChange}
              onShowSuggestedChannels={onShowSuggestedChannelsChange}
              serviceHasGeneralDescription={props.serviceHasGeneralDescription}
              count={count}
            />
          </Grid>
          <Grid item className={classes.searchResults}>
            <SearchResults
              suggestedChannels={props.suggestedChannels}
              selectedItems={props.selectedChannels}
              toggleSelectChannel={props.toggleSelectChannel}
              isLoading={query.isLoading}
              items={items}
              serviceResponsibleOrgId={props.serviceResponsibleOrgId}
              serviceStatus={props.serviceStatus}
            />
          </Grid>
        </Grid>
      </div>
      <div className={classes.paginator}>
        <Paginator currentPage={currentPage} pageCount={pageCount} onChange={goToPage} />
      </div>
    </div>
  );
}
