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

type FaxNumberDialCodeProps = {
  faxNumberIndex: number;
  control: Control<ConnectionFormModel>;
  language: Language;
};

export function FaxNumberDialCode(props: FaxNumberDialCodeProps): React.ReactElement {
  const { t } = useTranslation();

  const { field, fieldState } = useController({
    control: props.control,
    name: `${cC.faxNumbers}.${props.language}.${props.faxNumberIndex}.${cPhoneNumberLv.dialCodeId}`,
  });

  const error = fieldState.error?.message;

  function onDialCodeChange(dialCode: DialCode | null) {
    field.onChange(dialCode?.id);
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
