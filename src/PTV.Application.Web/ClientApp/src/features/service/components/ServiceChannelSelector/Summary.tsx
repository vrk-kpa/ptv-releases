import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import _ from 'lodash';
import { Button } from 'suomifi-ui-components';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { ChannelType } from 'types/enumTypes';
import SummaryItem from './SummaryItem';

const useStyles = makeStyles(() => ({
  button: {
    marginBottom: '20px',
  },
}));

type SummaryProps = {
  back: () => void;
  removeChannel: (channel: ConnectableChannel) => void;
  channels: ConnectableChannel[];
};

export default function Summary(props: SummaryProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  const groups = _.groupBy(props.channels, (x): ChannelType | null | undefined => x.channelType);

  return (
    <div>
      <div className={classes.button}>
        <Button onClick={props.back} icon='arrowLeft' variant='secondaryNoBorder'>
          {t('Ptv.Service.Form.ServiceChannelSelector.Summary.ChooseMore.Button.Label')}
        </Button>
      </div>

      <SummaryItem removeChannel={props.removeChannel} channels={groups['EChannel']} />
      <SummaryItem removeChannel={props.removeChannel} channels={groups['Phone']} />
      <SummaryItem removeChannel={props.removeChannel} channels={groups['PrintableForm']} />
      <SummaryItem removeChannel={props.removeChannel} channels={groups['ServiceLocation']} />
      <SummaryItem removeChannel={props.removeChannel} channels={groups['WebPage']} />
    </div>
  );
}
