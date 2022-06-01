import React from 'react';
import { Control, UseFormSetValue } from 'react-hook-form';
import { Box } from '@mui/material';
import { Language } from 'types/enumTypes';
import { ServiceFormValues, ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { FormBlock } from 'components/formLayout/FormBlock';
import ConnectedServiceChannels from 'features/service/components/ConnectedServiceChannels';
import ServiceChannelSelector from 'features/service/components/ServiceChannelSelector';

type ServiceChannelsFormProps = {
  language: Language;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
  getFormValues: () => ServiceFormValues;
};

export function ServiceChannelsForm(props: ServiceChannelsFormProps): React.ReactElement {
  const namespace = `${cService.languageVersions}.${props.language}`;
  return (
    <Box>
      <FormBlock mt={0}>
        <ServiceChannelSelector
          control={props.control}
          getFormValues={props.getFormValues}
          setValue={props.setValue}
          namespace={namespace}
        />
      </FormBlock>
      <FormBlock>
        <ConnectedServiceChannels
          control={props.control}
          getFormValues={props.getFormValues}
          setValue={props.setValue}
          namespace={namespace}
        />
      </FormBlock>
    </Box>
  );
}
