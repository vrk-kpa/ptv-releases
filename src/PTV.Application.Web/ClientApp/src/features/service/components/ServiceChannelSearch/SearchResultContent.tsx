import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Paragraph, Text } from 'suomifi-ui-components';
import { ConnectionApiModel } from 'types/api/connectionApiModel';
import { Language } from 'types/enumTypes';
import { ServerErrorBox } from 'components/ErrorBox/ServerErrorBox';
import LoadingIndicator from 'components/LoadingIndicator';
import { useGetPublishedChannelLatestVersionByUnificRootId } from 'hooks/queries/useGetPublishedChannelLatestVersionByUnificRootId';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { toDateAndTime } from 'utils/date';
import { getChannelValue } from 'utils/serviceChannel';
import { ConnectedServices } from './ConnectedServices';
import { SearchResultLanguageVersionContent } from './SearchResultLanguageVersionContent';
import { channelConnectionContainsGeneralData, channelLanguageVersionContainsData } from './utils';

const useStyles = makeStyles((theme) => ({
  loadingIndicator: {
    display: 'flex',
    flex: 1,
    justifyContent: 'center',
    alignItems: 'flex-start',
  },
  languageVersions: {
    marginTop: '10px',
  },
  languageVersion: {
    marginBottom: '20px',
    paddingLeft: '15px',
    borderLeftColor: '#a5abb0',
    borderLeft: '4px solid',
  },
  block: {
    marginBottom: '20px',
  },
  services: {
    marginTop: '10px',
  },
  additionalInformation: {
    paddingLeft: '15px',
    borderLeftColor: '#a5abb0',
    borderLeft: '4px solid',
    marginBottom: '20px',
  },
}));

type SearchResultContentProps = {
  unificRootId: string;
  channelConnection?: ConnectionApiModel;
  showConnectedServices: boolean;
};

export default function SearchResultContent(props: SearchResultContentProps): React.ReactElement | null {
  const { t } = useTranslation();
  const uiLang = useGetUiLanguage();
  const classes = useStyles();

  const query = useGetPublishedChannelLatestVersionByUnificRootId(props.unificRootId, true);

  if (query.isLoading || !query.data) {
    return (
      <div className={classes.loadingIndicator}>
        <LoadingIndicator />
      </div>
    );
  }

  if (query.error) {
    return <ServerErrorBox httpError={query.error} />;
  }

  const channel = query.data;
  const connectedChannelsContainServiceSpecificData =
    props.channelConnection?.languageVersions &&
    (Object.keys(props.channelConnection?.languageVersions) as Language[]).some(
      (channelLang: Language) =>
        channelLanguageVersionContainsData(props.channelConnection?.languageVersions[channelLang]) ||
        (props.channelConnection && channelConnectionContainsGeneralData(props.channelConnection, channelLang))
    );

  return (
    <div>
      {connectedChannelsContainServiceSpecificData && (
        <div className={classes.additionalInformation}>
          <Text style={{ fontSize: '14px', fontWeight: '600' }}>{t('Ptv.ConnectionDetails.Expanders.ContainsAdditionalInformation')}</Text>
        </div>
      )}

      <div className={classes.block}>
        <Text variant='bold'>{t('Ptv.Service.Form.ServiceChannelSearch.SearchResult.LastModified.Title')}</Text>
        <Paragraph>{`${toDateAndTime(channel.modified)} - ${channel.modifiedBy}`}</Paragraph>
      </div>

      <div className={classes.block}>
        <Text variant='bold'>{t('Ptv.Service.Form.ServiceChannelSearch.SearchResult.Summary.Title')}</Text>
        <Paragraph>{getChannelValue(channel.languageVersions, uiLang, (lv) => lv.shortDescription, '')}</Paragraph>
      </div>

      <div className={classes.block}>
        <Text variant='bold'>{t('Ptv.Service.Form.ServiceChannelSearch.SearchResult.LanguageVersions.Title')}</Text>
        <div className={classes.languageVersions}>
          <SearchResultLanguageVersionContent serviceChannel={channel} channelConnection={props.channelConnection} />
        </div>
      </div>

      {props.showConnectedServices && (
        <div className={classes.block}>
          <Text variant='bold'>
            {t('Ptv.Service.Form.ServiceChannelSearch.SearchResult.ConnectedChannels.Title', { count: channel.connections.length })}
          </Text>
          <div className={classes.services}>
            <ConnectedServices channel={channel} />
          </div>
        </div>
      )}
    </div>
  );
}
