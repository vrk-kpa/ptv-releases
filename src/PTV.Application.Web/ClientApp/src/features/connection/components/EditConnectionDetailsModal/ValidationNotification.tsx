import React from 'react';
import { Control, FieldErrors, useFormState } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import ValidationMessage from 'components/ValidationMessage';
import { hasNonEmptyKeys } from 'utils/objects';
import { FormBlock } from 'features/connection/components/FormLayout';

type ValidationNotificationProps = {
  control: Control<ConnectionFormModel>;
};

export function ValidationNotification(props: ValidationNotificationProps): React.ReactElement | null {
  const { t } = useTranslation();

  // Note: you need to deconstruct invidivual things,
  // see https://react-hook-form.com/api/useformstate/
  const { errors } = useFormState<ConnectionFormModel>({
    control: props.control,
  });

  if (!hasNonEmptyKeys(errors)) return null;

  const key = getErrorKey(errors);

  return (
    <FormBlock marginTop='10px'>
      <ValidationMessage message={t(key)} />
    </FormBlock>
  );
}

function getErrorKey(errors: FieldErrors<ConnectionFormModel>): string {
  if (errors.openingHours) return 'Ptv.ConnectionDetails.ValidationNotification.OpeningHours';
  if (errors.emails) return 'Ptv.ConnectionDetails.ValidationNotification.Emails';
  if (errors.addresses) return 'Ptv.ConnectionDetails.ValidationNotification.Addresses';
  if (errors.phoneNumbers) return 'Ptv.ConnectionDetails.ValidationNotification.PhoneNumbers';
  if (errors.webPages) return 'Ptv.ConnectionDetails.ValidationNotification.WebPages';
  if (errors.faxNumbers) return 'Ptv.ConnectionDetails.ValidationNotification.FaxNumbers';
  return 'Ptv.ConnectionDetails.ValidationNotification.CheckFormGeneral';
}
