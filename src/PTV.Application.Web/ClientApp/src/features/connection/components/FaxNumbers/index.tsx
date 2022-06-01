import React from 'react';
import { Control, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Paragraph } from 'suomifi-ui-components';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { ComparisonView } from 'components/ComparisonView';
import { VisualHeading } from 'components/VisualHeading';
import { useFormMetaContext } from 'context/formMeta';
import { FormBlock } from 'features/connection/components/FormLayout';
import { FaxNumberList } from './FaxNumberList';

type FaxNumbersProps = {
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function FaxNumbers(props: FaxNumbersProps): React.ReactElement {
  const meta = useFormMetaContext();
  const { t } = useTranslation();

  function renderRight(): React.ReactElement | undefined {
    if (meta.compareLanguageCode) {
      return <FaxNumberList control={props.control} trigger={props.trigger} language={meta.compareLanguageCode} />;
    }

    return undefined;
  }

  return (
    <div>
      <VisualHeading id='fax-numbers-heading' variant='h4'>
        {t('Ptv.ConnectionDetails.FaxNumbers.Title')}
      </VisualHeading>
      <FormBlock marginTop='20px' marginBottom='20px'>
        <Paragraph>{t('Ptv.ConnectionDetails.FaxNumbers.Description')}</Paragraph>
      </FormBlock>

      <ComparisonView
        left={<FaxNumberList control={props.control} trigger={props.trigger} language={meta.selectedLanguageCode} />}
        right={renderRight()}
      />
    </div>
  );
}
