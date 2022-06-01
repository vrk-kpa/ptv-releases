import React from 'react';
import { Control, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { DialCode } from 'types/enumItemType';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cC, cPhoneNumberLv } from 'types/forms/connectionFormTypes';
import ValidationMessage from 'components/ValidationMessage';
import { DialCodeSelector } from 'features/connection/components/DialCodeSelector';
import { toFieldId } from 'features/connection/utils/fieldid';

type PhoneNumberDialCodeProps = {
  phoneNumberIndex: number;
  control: Control<ConnectionFormModel>;
  onChanged: () => void;
  language: Language;
};

export function PhoneNumberDialCode(props: PhoneNumberDialCodeProps): React.ReactElement {
  const { t } = useTranslation();

  const { field, fieldState } = useController({
    control: props.control,
    name: `${cC.phoneNumbers}.${props.language}.${props.phoneNumberIndex}.${cPhoneNumberLv.dialCodeId}`,
  });

  const error = fieldState.error?.message;

  function onDialCodeChange(dialCode: DialCode | null) {
    field.onChange(dialCode?.id);
    props.onChanged();
  }

  return (
    <Grid>
      <Grid item>
        <DialCodeSelector id={toFieldId(field.name)} selectedDialCodeId={field.value} onChange={onDialCodeChange} />
      </Grid>
      {error && (
        <Grid item>
          <ValidationMessage message={t(error)} />
        </Grid>
      )}
    </Grid>
  );
}
