import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import _ from 'lodash';
import { Text } from 'suomifi-ui-components';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';
import { getConnectableChannelValue } from 'utils/serviceChannel';

type ChannelItemTitleProps = {
  channel: ConnectableChannel;
};

const useStyles = makeStyles((theme) => ({
  right: {
    marginLeft: 'auto',
  },
  dot: {
    display: 'inline-block',
    height: '10px',
    width: '10px',
    borderRadius: '50%',
    backgroundColor: 'rgb(195, 57, 50)',
    marginRight: '4px',
    marginBottom: '2px',
  },
}));

export function ChannelItemTitle(props: ChannelItemTitleProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const translate = useTranslateLocalizedText();
  const lang = useGetUiLanguage();
  const org = props.channel.organization;
  const orgName = org ? translate(org.texts, org.name) : '???';
  const channelName = getConnectableChannelValue(props.channel.languageVersions, lang, (x) => x.name, '');
  const showWarning = !!_.find(props.channel.languageVersions, (x) => x?.status !== 'Published');

  return (
    <Grid container>
      <Grid item container alignItems='center'>
        <Grid item>{channelName}</Grid>
        {showWarning && (
          <Grid item className={classes.right}>
            <div className={classes.dot}></div>
          </Grid>
        )}
        {showWarning && (
          <Grid item>
            <Text smallScreen={true}>{t('Ptv.Service.Form.ConnectedServiceChannels.Item.MissingLanguageVersion.Label')}</Text>
          </Grid>
        )}
      </Grid>
      <Grid item container>
        <Grid item>
          <Text style={{ fontSize: '16px', color: '#5F686D' }}>{orgName}</Text>
        </Grid>
      </Grid>
    </Grid>
  );
}
