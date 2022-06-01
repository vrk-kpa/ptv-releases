import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Chip, Text } from 'suomifi-ui-components';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { ChannelType } from 'types/enumTypes';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getConnectableChannelValue } from 'utils/serviceChannel';

const useStyles = makeStyles(() => ({
  title: {
    marginBottom: '10px',
  },
  chips: {
    marginBottom: '20px',
  },
  chip: {
    display: 'inline-block',
    marginRight: '10px',
    marginBottom: '10px',
  },
}));

type SummaryItemProps = {
  channels: ConnectableChannel[] | undefined;
  removeChannel: (channel: ConnectableChannel) => void;
};

const fallBackTitleKey = 'Ptv.Service.Form.ServiceChannelSelector.Summary.Chosen.Other.Title';

const TitleKeys = new Map<ChannelType, string>([
  ['ServiceLocation', 'Ptv.Service.Form.ServiceChannelSelector.Summary.Chosen.ServiceLocation.Title'],
  ['PrintableForm', 'Ptv.Service.Form.ServiceChannelSelector.Summary.Chosen.PritableForm.Title'],
  ['WebPage', 'Ptv.Service.Form.ServiceChannelSelector.Summary.Chosen.WebPage.Title'],
  ['EChannel', 'Ptv.Service.Form.ServiceChannelSelector.Summary.Chosen.EChannel.Title'],
  ['Phone', 'Ptv.Service.Form.ServiceChannelSelector.Summary.Chosen.Phone.Title'],
]);

function getTitleKey(channelType: ChannelType | null | undefined): string {
  if (!channelType) {
    return fallBackTitleKey;
  }

  return TitleKeys.get(channelType) || fallBackTitleKey;
}

export default function SummaryItem(props: SummaryItemProps): React.ReactElement | null {
  const { t } = useTranslation();
  const lang = useGetUiLanguage();
  const classes = useStyles();

  if (!props.channels || props.channels.length === 0) {
    return null;
  }

  const key = getTitleKey(props.channels[0].channelType);

  const chips = props.channels.map((ch) => {
    return (
      <div className={classes.chip} key={ch.id}>
        <Chip actionLabel={t('Ptv.Form.Chip.ActionLabel.Remove')} removable={true} onClick={() => props.removeChannel(ch)}>
          {getConnectableChannelValue(ch.languageVersions, lang, (lv) => lv.name, '')}
        </Chip>
      </div>
    );
  });

  return (
    <div>
      <div className={classes.title}>
        <Text smallScreen={true} variant='bold'>
          {t(key)}
        </Text>
      </div>

      <div className={classes.chips}>{chips}</div>
    </div>
  );
}
