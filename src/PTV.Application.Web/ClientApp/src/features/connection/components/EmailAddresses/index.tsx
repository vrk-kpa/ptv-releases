import React from 'react';
import { Control, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Paragraph } from 'suomifi-ui-components';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { ComparisonView } from 'components/ComparisonView';
import { VisualHeading } from 'components/VisualHeading';
import { useFormMetaContext } from 'context/formMeta';
import { FormBlock } from 'features/connection/components/FormLayout';
import { EmailAddressList } from './EmailAddressList';

type EmailAddressesProps = {
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function EmailAddresses(props: EmailAddressesProps): React.ReactElement {
  const meta = useFormMetaContext();
  const { t } = useTranslation();

  function renderRight(): React.ReactElement | undefined {
    if (meta.compareLanguageCode) {
      return <EmailAddressList control={props.control} language={meta.compareLanguageCode} trigger={props.trigger} />;
    }

    return undefined;
  }

  return (
    <div>
      <VisualHeading id='email-addresses-heading' variant='h4'>
        {t('Ptv.ConnectionDetails.EmailAddresses.Title')}
      </VisualHeading>
      <FormBlock marginTop='20px' marginBottom='20px'>
        <Paragraph>{t('Ptv.ConnectionDetails.EmailAddresses.Description')}</Paragraph>
      </FormBlock>

      <ComparisonView
        left={<EmailAddressList control={props.control} language={meta.selectedLanguageCode} trigger={props.trigger} />}
        right={renderRight()}
      />
    </div>
  );
}
