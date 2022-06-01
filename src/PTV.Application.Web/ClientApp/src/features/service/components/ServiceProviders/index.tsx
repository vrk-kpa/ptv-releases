import React from 'react';
import { Control, UseFormSetValue, useWatch } from 'react-hook-form';
import Box from '@mui/material/Box';
import { Language } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { OtherProducersMainLevel } from './OtherProducersMainLevel';
import { PurchaseProducersMainLevel } from './PurchaseProducersMainLevel';
import { SelfProducersMainLevel } from './SelfProducersMainLevel';
import { getDefaultSelfProducers } from './utils';

interface ServiceProvidersInformationInterface {
  tabLanguage: Language;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
  namespace: string;
}

export default function ServiceProviders(props: ServiceProvidersInformationInterface): React.ReactElement {
  const responsibleOrganization = useWatch({ control: props.control, name: `${cService.responsibleOrganization}` });
  const otherResponsibleOrganizations = useWatch({ control: props.control, name: `${cService.otherResponsibleOrganizations}` });
  const defaultSelfProducers = getDefaultSelfProducers(responsibleOrganization, otherResponsibleOrganizations);
  const responsibleOrganizationIds = defaultSelfProducers.map((x) => x.id);

  return (
    <Box>
      <Box mb={2}>
        <SelfProducersMainLevel
          control={props.control}
          tabLanguage={props.tabLanguage}
          setValue={props.setValue}
          defaultSelfProducers={defaultSelfProducers}
          namespace={props.namespace}
        />
        <PurchaseProducersMainLevel
          control={props.control}
          tabLanguage={props.tabLanguage}
          responsibleOrganizationIds={responsibleOrganizationIds}
          setValue={props.setValue}
        />
        <OtherProducersMainLevel
          control={props.control}
          tabLanguage={props.tabLanguage}
          responsibleOrganizationIds={responsibleOrganizationIds}
          namespace={props.namespace}
          setValue={props.setValue}
        />
      </Box>
    </Box>
  );
}
