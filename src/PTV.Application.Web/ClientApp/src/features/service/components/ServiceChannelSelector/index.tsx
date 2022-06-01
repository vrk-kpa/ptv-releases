import React, { useState } from 'react';
import { Control, UseFormSetValue, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { Button, Heading, Paragraph } from 'suomifi-ui-components';
import { ServiceFormValues, ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { useFormMetaContext } from 'context/formMeta';
import { canModifyConnections } from 'utils/serviceChannel';
import Note from './Note';
import ServiceChannelModal from './ServiceChannelModal';

type ServiceChannelSelectorProps = {
  control: Control<ServiceModel>;
  namespace: string;
  getFormValues: () => ServiceFormValues;
  setValue: UseFormSetValue<ServiceModel>;
};

export default function ServiceChannelSelector(props: ServiceChannelSelectorProps): React.ReactElement {
  const { t } = useTranslation();
  const serviceId = useWatch({ control: props.control, name: `${cService.id}` });
  const serviceStatus = useWatch({ control: props.control, name: `${cService.status}` });
  const { mode } = useFormMetaContext();

  const serviceHasBeenSaved = serviceId ? true : false;
  const [isOpen, setIsOpen] = useState<boolean>(false);

  const canModify = canModifyConnections(serviceId, serviceStatus);
  const disabled = mode !== 'view' || !canModify;

  return (
    <Box id={props.namespace + 'channel-selector'}>
      <Box mb={2}>
        <Heading variant='h2'>{t('Ptv.Service.Form.ServiceChannelSelector.Title')}</Heading>
      </Box>
      <Box mb={2}>
        <Paragraph>{t('Ptv.Service.Form.ServiceChannelSelector.Description')}</Paragraph>
      </Box>
      {canModify && (
        <Box mb={2}>
          <Button disabled={disabled} onClick={() => setIsOpen(true)}>
            {t('Ptv.Service.Form.ServiceChannelSelector.AddServiceChannels.Button.Label')}
          </Button>
        </Box>
      )}
      <Note serviceHasBeenSaved={serviceHasBeenSaved} />
      {serviceId && (
        <ServiceChannelModal isOpen={isOpen} close={() => setIsOpen(false)} getFormValues={props.getFormValues} setValue={props.setValue} />
      )}
    </Box>
  );
}
