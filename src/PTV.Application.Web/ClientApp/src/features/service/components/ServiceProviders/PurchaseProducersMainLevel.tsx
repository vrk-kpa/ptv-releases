import React, { useState } from 'react';
import { Control, UseFormSetValue, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { Expander, ExpanderContent, ExpanderTitleButton } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cLv, cService } from 'types/forms/serviceFormTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { useFormMetaContext } from 'context/formMeta';
import { toFieldStatus } from 'utils/rhf';
import { DuplicateOrganizations } from './DuplicateOrganizations';
import { ProducerAdditionalInformation } from './ProducerAdditionalInformation';
import ProducerOrganizationSelector from './ProducerOrganizationSelector';

type PurchaseProducersMainLevelProps = {
  tabLanguage: Language;
  control: Control<ServiceModel>;
  responsibleOrganizationIds: string[];
  setValue: UseFormSetValue<ServiceModel>;
};

export function PurchaseProducersMainLevel(props: PurchaseProducersMainLevelProps): React.ReactElement | null {
  const { mode } = useFormMetaContext();
  const { t } = useTranslation();
  const [open, setOpen] = useState(false);

  const { field, fieldState } = useController({
    control: props.control,
    name: `${cService.purchaseProducers}`,
  });

  const { status, statusText } = toFieldStatus(fieldState);

  function onOpenChange() {
    setOpen((expanded) => !expanded);
  }

  function selectOrganizations(organizations: OrganizationModel[]) {
    field.onChange(organizations);
  }

  if (mode === 'view') {
    return (
      <Box>
        <ProducerOrganizationSelector
          id={cService.purchaseProducers}
          selectedOrganizations={field.value}
          label={t('Ptv.Service.Form.Field.ServiceProviders.PurchaseProducers.Organizations.Label')}
          placeholder={t('Ptv.Service.Form.Field.ServiceProviders.PurchaseProducers.Organizations.Placeholder')}
          selectOrganizations={selectOrganizations}
          responsibleOrganizationIds={props.responsibleOrganizationIds}
        />
        <ProducerAdditionalInformation
          language={props.tabLanguage}
          control={props.control}
          name={cLv.purchaseProducers}
          description={t('Ptv.Service.Form.Field.ServiceProviders.PurchaseProducers.AdditionalInformation.Description')}
          setValue={props.setValue}
        />
      </Box>
    );
  }

  const count = field.value.length;

  return (
    <Box>
      <Expander id={cService.purchaseProducers} open={open} onOpenChange={onOpenChange}>
        <ExpanderTitleButton asHeading='h3'>
          <span>{t('Ptv.Service.Form.Field.ServiceProducers.PurchaseProducers.Title') + ` (${count})`}</span>
        </ExpanderTitleButton>
        <ExpanderContent>
          <ProducerOrganizationSelector
            id={cService.purchaseProducers}
            selectedOrganizations={field.value}
            label={t('Ptv.Service.Form.Field.ServiceProviders.PurchaseProducers.Organizations.Label')}
            placeholder={t('Ptv.Service.Form.Field.ServiceProviders.PurchaseProducers.Organizations.Placeholder')}
            selectOrganizations={selectOrganizations}
            responsibleOrganizationIds={props.responsibleOrganizationIds}
          />
          <DuplicateOrganizations status={status} statusText={statusText} />
          <ProducerAdditionalInformation
            language={props.tabLanguage}
            control={props.control}
            name={cLv.purchaseProducers}
            description={t('Ptv.Service.Form.Field.ServiceProviders.PurchaseProducers.AdditionalInformation.Description')}
            setValue={props.setValue}
          />
        </ExpanderContent>
      </Expander>
    </Box>
  );
}
