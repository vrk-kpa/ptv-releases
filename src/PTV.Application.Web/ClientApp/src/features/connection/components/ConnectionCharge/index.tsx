import React from 'react';
import { Control, UseFormSetValue } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Heading, Paragraph } from 'suomifi-ui-components';
import { ConnectionFormModel, cLv } from 'types/forms/connectionFormTypes';
import { ComparisonView } from 'components/ComparisonView';
import { useFormMetaContext, useGetFieldName } from 'context/formMeta';
import { FormBlock } from 'features/connection/components/FormLayout';
import { toFieldId } from 'features/connection/utils/fieldid';
import { AdditionalInfo } from './AdditionalInfo';
import { ChargeSelector } from './ChargeSelector';

export type ConnectionChargeProps = {
  control: Control<ConnectionFormModel>;
  setValue: UseFormSetValue<ConnectionFormModel>;
};

export function ConnectionCharge(props: ConnectionChargeProps): React.ReactElement {
  const { t } = useTranslation();
  const meta = useFormMetaContext();
  const name = useGetFieldName();

  return (
    <div>
      <FormBlock marginTop='20px'>
        <ChargeSelector control={props.control} />
      </FormBlock>

      <FormBlock marginTop='20px'>
        <Heading variant='h3'>{t('Ptv.ConnectionDetails.Charge.AdditionalInfo.Title')}</Heading>
      </FormBlock>

      <FormBlock marginTop='20px'>
        <Paragraph>{t('Ptv.ConnectionDetails.Charge.AdditionalInfo.Description')}</Paragraph>
      </FormBlock>

      <FormBlock marginTop='20px'>
        <ComparisonView
          left={
            <AdditionalInfo
              control={props.control}
              name={name(cLv.chargeInfo)}
              id={toFieldId(name(cLv.chargeInfo))}
              language={meta.selectedLanguageCode}
              setValue={props.setValue}
            />
          }
          right={
            <AdditionalInfo
              control={props.control}
              name={name(cLv.chargeInfo, meta.compareLanguageCode)}
              id={toFieldId(name(cLv.chargeInfo, meta.compareLanguageCode))}
              language={meta.compareLanguageCode ?? meta.selectedLanguageCode}
              setValue={props.setValue}
            />
          }
        />
      </FormBlock>
    </div>
  );
}
