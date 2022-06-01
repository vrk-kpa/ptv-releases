import React, { useState } from 'react';
import { Control, UseFormSetValue, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { makeStyles } from '@mui/styles';
import _ from 'lodash';
import { Heading, Paragraph } from 'suomifi-ui-components';
import { ServiceFormValues, ServiceModel, cService } from 'types/forms/serviceFormTypes';
import Paginator from 'components/Paginator';
import { getItemsForPage, getPageCount } from 'utils/pagination';
import ChannelGroup from './ChannelGroup';

const useStyles = makeStyles((theme) => ({
  paginator: {
    display: 'flex',
    justifyContent: 'center',
    marginTop: '20px',
  },
}));

type ConnectedServiceChannelsProps = {
  control: Control<ServiceModel>;
  namespace: string;
  setValue: UseFormSetValue<ServiceModel>;
  getFormValues: () => ServiceFormValues;
};

const RowsPerPage = 10;
const AstiGroupKey = 'Asti';

export default function ConnectedServiceChannels(props: ConnectedServiceChannelsProps): React.ReactElement | null {
  const { t } = useTranslation();
  const classes = useStyles();
  const [pageNumber, setPageNumber] = useState<number>(1);

  const allChannels = useWatch({ control: props.control, name: `${cService.connectedChannels}` });
  const responsibleOrganization = useWatch({ control: props.control, name: `${cService.responsibleOrganization}` });
  const serviceId = useWatch({ control: props.control, name: `${cService.id}` });
  const serviceStatus = useWatch({ control: props.control, name: `${cService.status}` });

  const pageCount = getPageCount(allChannels.length, RowsPerPage);
  const channels = getItemsForPage(allChannels, pageNumber, RowsPerPage);
  const hasMultiplePages = allChannels.length > RowsPerPage;

  const groups = _.groupBy(channels, (x) => {
    return x.isASTIConnection ? AstiGroupKey : x.channelType;
  });

  if (!serviceId) {
    return null;
  }

  function removeServiceChannel(serviceChannelUnificRootId: string) {
    const formValues = props.getFormValues();
    const channels = formValues.connectedChannels.filter((x) => x.unificRootId !== serviceChannelUnificRootId);
    props.setValue(`${cService.connectedChannels}`, channels);
  }

  return (
    <Box id={props.namespace + 'connected-channels'}>
      <Box mb={2}>
        <Heading variant='h3'>{t('Ptv.Service.Form.ConnectedServiceChannels.Title', { count: allChannels.length })}</Heading>
      </Box>
      <Box mb={2}>
        <Paragraph>{t('Ptv.Service.Form.ConnectedServiceChannels.Description')}</Paragraph>
      </Box>
      <ChannelGroup
        channels={groups['EChannel']}
        serviceStatus={serviceStatus}
        serviceOrganization={responsibleOrganization}
        getFormValues={props.getFormValues}
        removeServiceChannel={removeServiceChannel}
      />
      <ChannelGroup
        channels={groups['Phone']}
        serviceStatus={serviceStatus}
        serviceOrganization={responsibleOrganization}
        getFormValues={props.getFormValues}
        removeServiceChannel={removeServiceChannel}
      />
      <ChannelGroup
        channels={groups['PrintableForm']}
        serviceStatus={serviceStatus}
        serviceOrganization={responsibleOrganization}
        getFormValues={props.getFormValues}
        removeServiceChannel={removeServiceChannel}
      />
      <ChannelGroup
        channels={groups['ServiceLocation']}
        serviceStatus={serviceStatus}
        serviceOrganization={responsibleOrganization}
        getFormValues={props.getFormValues}
        removeServiceChannel={removeServiceChannel}
      />
      <ChannelGroup
        channels={groups['WebPage']}
        serviceStatus={serviceStatus}
        serviceOrganization={responsibleOrganization}
        getFormValues={props.getFormValues}
        removeServiceChannel={removeServiceChannel}
      />
      <ChannelGroup
        channels={groups[AstiGroupKey]}
        isAstiGroup={true}
        serviceStatus={serviceStatus}
        serviceOrganization={responsibleOrganization}
        getFormValues={props.getFormValues}
        removeServiceChannel={removeServiceChannel}
      />
      {hasMultiplePages && (
        <div className={classes.paginator}>
          <Paginator pageCount={pageCount} currentPage={pageNumber} onChange={(page) => setPageNumber(page)} />
        </div>
      )}
    </Box>
  );
}
