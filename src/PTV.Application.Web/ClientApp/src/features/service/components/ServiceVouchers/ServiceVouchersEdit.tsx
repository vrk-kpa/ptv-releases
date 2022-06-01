import React from 'react';
import { Control, UseFormSetValue, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { Heading, Paragraph } from 'suomifi-ui-components';
import { Language, VoucherType } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { ComparisonView } from 'components/ComparisonView';
import { OptionalFieldByWatch } from 'components/OptionalFieldByWatch';
import { useFormMetaContext } from 'context/formMeta';
import { isServiceVoucherNoUrl, isServiceVoucherWithUrl } from 'features/service/utils';
import { ServiceVoucherAdditionalInfo } from './ServiceVoucherAdditionalInfo';
import { ServiceVoucherLinks } from './ServiceVoucherLinks';
import { ServiceVoucherType } from './ServiceVoucherType';

interface ServiceVouchersInterface {
  namespace: string;
  tabLanguage: Language;
  control: Control<ServiceModel>;
  enabledLanguages: Language[];
  trigger: UseFormTrigger<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
}

export default function ServiceVouchersEdit(props: ServiceVouchersInterface): React.ReactElement {
  const { t } = useTranslation();
  const { mode, compareLanguageCode } = useFormMetaContext();

  function renderRightSideLinks(): React.ReactElement | undefined {
    if (compareLanguageCode) {
      return (
        <ServiceVoucherLinks control={props.control} tabLanguage={compareLanguageCode} trigger={props.trigger} setValue={props.setValue} />
      );
    }

    return undefined;
  }

  function renderRightSideInfo(): React.ReactElement | undefined {
    if (compareLanguageCode) {
      return <ServiceVoucherAdditionalInfo control={props.control} tabLanguage={compareLanguageCode} setValue={props.setValue} />;
    }

    return undefined;
  }

  return (
    <Box>
      <Box mb={2}>
        <Heading variant='h4'>{t('Ptv.Service.Form.Field.ServiceVouchers.Title')}</Heading>
      </Box>
      <Box mb={2}>
        <Paragraph>{t('Ptv.Service.Form.Field.ServiceVouchers.Description')}</Paragraph>
      </Box>
      <Box mb={2}>
        <ServiceVoucherType
          control={props.control}
          name={cService.voucherType}
          tabLanguage={props.tabLanguage}
          mode={mode}
          enabledLanguages={props.enabledLanguages}
        />
      </Box>
      <OptionalFieldByWatch<VoucherType> control={props.control} fieldName={cService.voucherType} shouldRender={isServiceVoucherNoUrl}>
        <ComparisonView
          left={<ServiceVoucherAdditionalInfo control={props.control} tabLanguage={props.tabLanguage} setValue={props.setValue} />}
          right={renderRightSideInfo()}
        />
      </OptionalFieldByWatch>
      <OptionalFieldByWatch<VoucherType> control={props.control} fieldName={cService.voucherType} shouldRender={isServiceVoucherWithUrl}>
        <ComparisonView
          left={
            <ServiceVoucherLinks
              control={props.control}
              tabLanguage={props.tabLanguage}
              trigger={props.trigger}
              setValue={props.setValue}
            />
          }
          right={renderRightSideLinks()}
        />
      </OptionalFieldByWatch>
    </Box>
  );
}
