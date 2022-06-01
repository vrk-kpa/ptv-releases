import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfTextInput } from 'fields';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cC, cEmailLv } from 'types/forms/connectionFormTypes';
import { toFieldId } from 'features/connection/utils/fieldid';

type EmailProps = {
  emailIndex: number;
  control: Control<ConnectionFormModel>;
  language: Language;
};

export function Email(props: EmailProps): React.ReactElement {
  const { t } = useTranslation();

  const fieldName = `${cC.emails}.${props.language}.${props.emailIndex}.${cEmailLv.value}`;
  const fieldId = toFieldId(fieldName);

  return (
    <RhfTextInput
      control={props.control}
      name={fieldName}
      id={fieldId}
      mode='edit'
      labelText={t('Ptv.ConnectionDetails.EmailAddress.Label')}
      fullWidth={false}
      visualPlaceholder={t('Ptv.ConnectionDetails.EmailAddress.Placeholder')}
    />
  );
}
