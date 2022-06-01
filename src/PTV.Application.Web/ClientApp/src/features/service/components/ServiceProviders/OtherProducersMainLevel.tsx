import React, { useState } from 'react';
import { Control, UseFormSetValue, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { Expander, ExpanderContent, ExpanderTitleButton } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cLv, cService } from 'types/forms/serviceFormTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { useFormMetaContext } from 'context/formMeta';
import { withNamespace } from 'utils/fieldIds';
import { toFieldStatus } from 'utils/rhf';
import { DuplicateOrganizations } from './DuplicateOrganizations';
import { ProducerAdditionalInformation } from './ProducerAdditionalInformation';
import ProducerOrganizationSelector from './ProducerOrganizationSelector';

type OtherProducersMainLevelProps = {
  tabLanguage: Language;
  control: Control<ServiceModel>;
  responsibleOrganizationIds: string[];
  namespace: string;
  setValue: UseFormSetValue<ServiceModel>;
};

export function OtherProducersMainLevel(props: OtherProducersMainLevelProps): React.ReactElement | null {
  const { mode } = useFormMetaContext();
  const { t } = useTranslation();
  const [open, setOpen] = useState(false);

  const { field, fieldState } = useController({
    control: props.control,
    name: `${cService.otherProducers}`,
  });

  const { status, statusText } = toFieldStatus(fieldState);

  function onOpenChange() {
    setOpen((expanded) => !expanded);
  }

  function selectOrganizations(organizations: OrganizationModel[]) {
    field.onChange(organizations);
  }

  const count = field.value.length;
  const fieldId = withNamespace(props.namespace, cService.otherProducers);

  if (mode === 'view') {
    return (
      <Box>
        <ProducerOrganizationSelector
          id={fieldId}
          selectedOrganizations={field.value}
          label={t('Ptv.Service.Form.Field.ServiceProviders.OtherProducers.Organizations.Label')}
          placeholder={t('Ptv.Service.Form.Field.ServiceProviders.OtherProducers.Organizations.Placeholder')}
          selectOrganizations={selectOrganizations}
          responsibleOrganizationIds={props.responsibleOrganizationIds}
        />
        <ProducerAdditionalInformation
          control={props.control}
          language={props.tabLanguage}
          name={cLv.otherProducers}
          description={t('Ptv.Service.Form.Field.ServiceProviders.OtherProducers.AdditionalInformation.Description')}
          setValue={props.setValue}
        />
      </Box>
    );
  }

  return (
    <Box>
      <Expander id={cService.otherProducers} open={open} onOpenChange={onOpenChange}>
        <ExpanderTitleButton asHeading='h3'>
          <span>{t('Ptv.Service.Form.Field.ServiceProducers.OtherProducers.Title') + ` (${count})`}</span>
        </ExpanderTitleButton>
        <ExpanderContent>
          <ProducerOrganizationSelector
            id={fieldId}
            selectedOrganizations={field.value}
            label={t('Ptv.Service.Form.Field.ServiceProviders.OtherProducers.Organizations.Label')}
            placeholder={t('Ptv.Service.Form.Field.ServiceProviders.OtherProducers.Organizations.Placeholder')}
            selectOrganizations={selectOrganizations}
            responsibleOrganizationIds={props.responsibleOrganizationIds}
          />
          <DuplicateOrganizations status={status} statusText={statusText} />
          <ProducerAdditionalInformation
            control={props.control}
            language={props.tabLanguage}
            name={cLv.otherProducers}
            description={t('Ptv.Service.Form.Field.ServiceProviders.OtherProducers.AdditionalInformation.Description')}
            setValue={props.setValue}
          />
        </ExpanderContent>
      </Expander>
    </Box>
  );
}
