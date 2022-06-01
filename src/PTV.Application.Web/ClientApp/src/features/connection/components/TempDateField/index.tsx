import React from 'react';
import { Control, UseControllerProps, useController } from 'react-hook-form';
import { Grid, TextField } from '@mui/material';
import { Text } from 'suomifi-ui-components';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import ValidationMessage from 'components/ValidationMessage';
import { toLocalDateTime } from 'utils/date';
import { toFieldStatus } from 'utils/rhf';
import { FormBlock } from 'features/connection/components/FormLayout';

type TempDateFieldProps = {
  control: Control<ConnectionFormModel>;
  id: string;
  label: string;
  onChanged?: (newValue: string) => void;
};

// This component should be replaced with date picker
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function TempDateField(props: TempDateFieldProps & UseControllerProps<any>): React.ReactElement {
  const { field, fieldState } = useController({ name: props.name, control: props.control });
  const { statusText } = toFieldStatus(fieldState);

  function onChange(evt: React.ChangeEvent<HTMLInputElement>) {
    field.onChange(evt.target.value);
    props.onChanged?.(evt.target.value);
  }

  const date = field.value ? toLocalDateTime(field.value).toISODate() : '';

  return (
    <Grid container direction='column'>
      <Grid item>
        <Text smallScreen={true} variant='bold'>
          {props.label}
        </Text>
      </Grid>
      <Grid item>
        <FormBlock marginTop='10px'>
          <TextField id={props.id} type='date' value={date} onChange={onChange} />
        </FormBlock>
      </Grid>
      <Grid item>
        <ValidationMessage message={statusText} />
      </Grid>
    </Grid>
  );
}
