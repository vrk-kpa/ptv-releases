import React from 'react';
import { Control, UseFormSetValue, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { Heading } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceFormValues, ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { FormBlock } from 'components/formLayout/FormBlock';
import { FormDivider } from 'components/formLayout/FormDivider';
import { useFormMetaContext } from 'context/formMeta';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import OrganizationSelector from 'features/service/components/OrganizationSelector';
import OtherOrganizationSelector from 'features/service/components/OtherOrganizationSelector';
import ServiceFundingType from 'features/service/components/ServiceFundingType';

type ServiceOrganizationFormProps = {
  language: Language;
  enabledLanguages: Language[];
  control: Control<ServiceModel>;
  namespace: string;
  setValue: UseFormSetValue<ServiceModel>;
  trigger: UseFormTrigger<ServiceModel>;
  getFormValues: () => ServiceFormValues;
};

export function ServiceOrganizationForm(props: ServiceOrganizationFormProps): React.ReactElement {
  const { t } = useTranslation();
  const { userOrganizations } = useAppContextOrThrow();
  const { mode } = useFormMetaContext();

  return (
    <>
      <Box>
        <FormBlock>
          <OrganizationSelector
            userOrganizations={userOrganizations}
            control={props.control}
            setValue={props.setValue}
            language={props.language}
            namespace={props.namespace}
            enabledLanguages={props.enabledLanguages}
          />
        </FormBlock>
        <FormBlock mt={3}>
          <OtherOrganizationSelector
            control={props.control}
            setValue={props.setValue}
            namespace={props.namespace}
            enabledLanguages={props.enabledLanguages}
          />
        </FormBlock>
        <FormDivider my={3} />
        <FormBlock>
          <Heading variant='h4'>{t('Ptv.Service.Form.Field.FundingType.Title')}</Heading>
        </FormBlock>
        <FormBlock>
          <ServiceFundingType control={props.control} name={cService.fundingType} tabLanguage={props.language} mode={mode} />
        </FormBlock>
        <FormDivider my={3} />
        <FormBlock>
          <Heading id={`${props.namespace}.serviceProducers`} tabIndex={0} variant='h4'>
            {t('Ptv.Service.Form.Field.ServiceProducers.Title.Text')}
          </Heading>
        </FormBlock>
      </Box>
    </>
  );
}
