import React from 'react';
import { UseControllerProps, useController } from 'react-hook-form';
import { RhfReadOnlyField } from 'fields';
import { Textarea, TextareaProps } from 'suomifi-ui-components';
import { Mode } from 'types/enumTypes';
import { toFieldStatus } from 'utils/rhf';

type RhfTextareaProps = TextareaProps & {
  id: string;
  name: string;
  mode: Mode;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function RhfTextarea(props: RhfTextareaProps & UseControllerProps<any>): React.ReactElement {
  const { field, fieldState } = useController(props);
  const { control, ...rest } = props;
  const { status, statusText } = toFieldStatus(fieldState);

  if (props.mode === 'view') {
    return <RhfReadOnlyField value={field.value} id={props.id} labelText={props.labelText} />;
  }

  return <Textarea fullWidth={true} {...field} {...rest} status={status} statusText={statusText} />;
}
