import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Text } from 'suomifi-ui-components';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { PublishingStatus } from 'types/enumTypes';
import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { useCanEditConnectionDetailsFromService } from 'hooks/security/useCanEditConnectionDetailsFromService';
import { useCanRemoveConnectionFromService } from 'hooks/security/useCanRemoveConnectionFromService';
import { getKeyForChannelType } from 'utils/translations';
import ChannelItem from './ChannelItem';

const useStyles = makeStyles(() => ({
  title: {
    marginTop: '20px',
    marginBottom: '10px',
  },
}));

type ChannelGroupProps = {
  serviceStatus: PublishingStatus;
  serviceOrganization: OrganizationModel | null | undefined;
  channels: ConnectableChannel[] | undefined;
  isAstiGroup?: boolean;
  getFormValues: () => ServiceFormValues;
  removeServiceChannel: (serviceChannelUnificRootId: string) => void;
};

export default function ChannelGroup(props: ChannelGroupProps): React.ReactElement | null {
  const { t } = useTranslation();
  const classes = useStyles();
  const canRemove = useCanRemoveConnectionFromService();
  const canEdit = useCanEditConnectionDetailsFromService();

  if (!props.channels) {
    return null;
  }

  function getTitleKey(channels: ConnectableChannel[]): string {
    if (props.isAstiGroup) {
      return 'Ptv.Service.Form.ConnectedServiceChannels.Groups.Asti.Title';
    }

    return getKeyForChannelType(channels[0].channelType);
  }

  const serviceOrg = props.serviceOrganization;

  return (
    <div>
      <div className={classes.title}>
        <Text variant='bold'>{t(getTitleKey(props.channels))}</Text>
      </div>

      {props.channels.map((channel) => (
        <ChannelItem
          key={channel.id}
          channel={channel}
          canRemove={canRemove(serviceOrg?.id, props.serviceStatus, channel)}
          canEdit={canEdit(serviceOrg?.id, props.serviceStatus)}
          getFormValues={props.getFormValues}
          removeServiceChannel={props.removeServiceChannel}
        />
      ))}
    </div>
  );
}
