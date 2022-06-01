import React, { useState } from 'react';
import { UseFormSetValue } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useQueryClient } from 'react-query';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { ModalContent, ModalTitle } from 'suomifi-ui-components';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { ServiceFormValues, ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { Message } from 'components/Message';
import PtvModal from 'components/PtvModal';
import { useAddConnections } from 'hooks/queries/useAddConnections';
import ServiceChannelSearch from 'features/service/components/ServiceChannelSearch';
import ServiceChannelModalFooter from './ServiceChannelModalFooter';
import Summary from './Summary';
import { ViewType } from './utils';

const useStyles = makeStyles(() => ({
  hidden: {
    display: 'none',
  },
  error: {
    marginTop: '20px',
  },
}));

type ServiceChannelModalProps = {
  isOpen: boolean;
  close: () => void;
  getFormValues: () => ServiceFormValues;
  setValue: UseFormSetValue<ServiceModel>;
};

export default function ServiceChannelModal(props: ServiceChannelModalProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const [selectedChannels, setSelectedChannels] = useState<ConnectableChannel[]>([]);
  const [activeView, setActiveView] = useState<ViewType>('SearchView');
  const query = useAddConnections();
  const queryClient = useQueryClient();

  const serviceModel = props.getFormValues();
  const suggestedChannels = serviceModel.generalDescription ? serviceModel.generalDescription.channels.map((x) => x.unificRootId) : [];
  const hasGeneralDescription = !!serviceModel.generalDescription;

  function toggleSelectChannel(channel: ConnectableChannel) {
    const existing = selectedChannels.find((x) => x.id === channel.id);
    if (existing) {
      setSelectedChannels(selectedChannels.filter((x) => x.id !== channel.id));
    } else {
      setSelectedChannels(selectedChannels.concat([channel]));
    }
  }

  function switchActiveView() {
    const newMode = activeView === 'SearchView' ? 'SummaryView' : 'SearchView';
    setActiveView(newMode);
  }

  function closeAndDiscardChanges() {
    setActiveView('SearchView');
    setSelectedChannels([]);
    query.reset();
    props.close();
  }

  function onSavedSuccessfully(data: ConnectableChannel[]) {
    query.reset();
    props.close();
    setActiveView('SearchView');
    setSelectedChannels([]);
    props.setValue(`${cService.connectedChannels}`, data);
    queryClient.invalidateQueries(['next/connection/for-service'], { active: true });
  }

  function saveChanges() {
    query.mutate(
      {
        serviceId: serviceModel.id || '',
        serviceChannelUnificRootIds: selectedChannels.map((x) => x.unificRootId),
      },
      {
        onSuccess: onSavedSuccessfully,
      }
    );
  }

  const searchViewClassName = clsx({ [classes.hidden]: activeView === 'SummaryView' });
  const summaryViewClassName = clsx({ [classes.hidden]: activeView === 'SearchView' });

  return (
    <PtvModal appElementId='root' scrollable={true} visible={props.isOpen} onEscKeyDown={closeAndDiscardChanges}>
      <ModalContent>
        <ModalTitle>{t('Ptv.Service.Form.ServiceChannelSelector.Modal.Title')}</ModalTitle>
        <div className={searchViewClassName}>
          <ServiceChannelSearch
            suggestedChannels={suggestedChannels}
            toggleSelectChannel={toggleSelectChannel}
            selectedChannels={selectedChannels}
            serviceId={serviceModel.id}
            serviceStatus={serviceModel.status}
            serviceResponsibleOrgId={serviceModel.responsibleOrganization?.id}
            serviceHasGeneralDescription={hasGeneralDescription}
          />
        </div>
        <div className={summaryViewClassName}>
          <Summary removeChannel={toggleSelectChannel} channels={selectedChannels} back={switchActiveView} />
        </div>
        {query.error && (
          <Message className={classes.error} type='error'>
            {t('Ptv.Error.ServerError.Generic', { statusCode: query.error.response?.status })}
          </Message>
        )}
      </ModalContent>
      <ServiceChannelModalFooter
        activeView={activeView}
        selectedChannelsCount={selectedChannels.length}
        closeAndDiscardChanges={closeAndDiscardChanges}
        saveChanges={saveChanges}
        showSummary={switchActiveView}
        isLoading={query.isLoading}
      />
    </PtvModal>
  );
}
