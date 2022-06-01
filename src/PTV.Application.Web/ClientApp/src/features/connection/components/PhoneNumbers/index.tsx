import React from 'react';
import { Control, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Paragraph } from 'suomifi-ui-components';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { ComparisonView } from 'components/ComparisonView';
import { VisualHeading } from 'components/VisualHeading';
import { useFormMetaContext } from 'context/formMeta';
import { FormBlock } from 'features/connection/components/FormLayout';
import { PhoneNumberList } from './PhoneNumberList';

type PhoneNumbersProps = {
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function PhoneNumbers(props: PhoneNumbersProps): React.ReactElement {
  const meta = useFormMetaContext();
  const { t } = useTranslation();

  function renderRight(): React.ReactElement | undefined {
    if (meta.compareLanguageCode) {
      return <PhoneNumberList control={props.control} trigger={props.trigger} language={meta.compareLanguageCode} />;
    }

    return undefined;
  }

  return (
    <div>
      <VisualHeading id='phone-numbers-heading' variant='h4'>
        {t('Ptv.ConnectionDetails.PhoneNumbers.Title')}
      </VisualHeading>
      <FormBlock marginTop='20px' marginBottom='20px'>
        <Paragraph>{t('Ptv.ConnectionDetails.PhoneNumbers.Description')}</Paragraph>
      </FormBlock>

      <ComparisonView
        left={<PhoneNumberList control={props.control} trigger={props.trigger} language={meta.selectedLanguageCode} />}
        right={renderRight()}
      />
    </div>
  );
}
