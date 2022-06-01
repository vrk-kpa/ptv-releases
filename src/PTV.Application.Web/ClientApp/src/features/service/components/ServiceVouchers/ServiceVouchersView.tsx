import React from 'react';
import { Control, useFieldArray, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { RhfReadOnlyField } from 'fields';
import { Block, Heading } from 'suomifi-ui-components';
import { Language, VoucherType } from 'types/enumTypes';
import { ServiceModel, cLv, cService, cVoucher } from 'types/forms/serviceFormTypes';
import { OptionalFieldByWatch } from 'components/OptionalFieldByWatch';
import { VisualHeading } from 'components/VisualHeading';
import { getFieldId } from 'utils/fieldIds';
import { isServiceVoucherNoUrl, isServiceVoucherWithUrl } from 'features/service/utils';
import { ServiceVoucherLinkView } from './ServiceVoucherLinkView';

interface ServiceVouchersInterface {
  namespace: string;
  tabLanguage: Language;
  control: Control<ServiceModel>;
}

export default function ServiceVouchersView(props: ServiceVouchersInterface): React.ReactElement {
  const { t } = useTranslation();

  const additionalInformationName = `${cService.languageVersions}.${props.tabLanguage}.${cLv.voucher}.${cVoucher.info}`;
  const additionalInformationfieldValue = useWatch({
    control: props.control,
    name: `${cService.languageVersions}.${props.tabLanguage}.${cLv.voucher}.${cVoucher.info}`,
  });

  const { fields: linkFields } = useFieldArray({
    control: props.control,
    name: `${cService.languageVersions}.${props.tabLanguage}.${cLv.voucher}.${cVoucher.links}`,
  });

  const voucherTypefieldId = getFieldId(cService.voucherType, props.tabLanguage, undefined);
  const voucherType = useWatch({ control: props.control, name: `${cService.voucherType}` });

  return (
    <Box>
      <Box mb={2}>
        <Heading variant='h4'>{t('Ptv.Service.Form.Field.ServiceVouchers.Title')}</Heading>
      </Box>
      <Box mb={2}>
        <RhfReadOnlyField
          labelText={t('Ptv.Service.Form.Field.ServiceVoucherType.Label')}
          id={voucherTypefieldId}
          value={t(`Ptv.Service.Form.Field.ServiceVoucherType.${voucherType}`)}
        />
      </Box>
      <OptionalFieldByWatch<VoucherType> control={props.control} fieldName={cService.voucherType} shouldRender={isServiceVoucherNoUrl}>
        <RhfReadOnlyField
          value={additionalInformationfieldValue}
          id={additionalInformationName}
          labelText={t('Ptv.Service.Form.Field.ServiceVouchers.AdditionalInformation.Label')}
        />
      </OptionalFieldByWatch>
      <OptionalFieldByWatch<VoucherType> control={props.control} fieldName={cService.voucherType} shouldRender={isServiceVoucherWithUrl}>
        <Box>
          <Box mb={2}>
            <VisualHeading variant='h4'>{t('Ptv.Service.Form.Field.ServiceVoucherLinks.Label')}</VisualHeading>
          </Box>
          <Block>
            {linkFields.map((item, index) => (
              <ServiceVoucherLinkView key={item.id} language={props.tabLanguage} value={item} index={index} />
            ))}
          </Block>
        </Box>
      </OptionalFieldByWatch>
    </Box>
  );
}
