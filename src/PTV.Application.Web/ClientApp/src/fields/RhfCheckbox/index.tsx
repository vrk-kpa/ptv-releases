import React from 'react';
import { UseControllerProps, useController } from 'react-hook-form';
import { RhfReadOnlyField } from 'fields';
import { Checkbox, CheckboxProps } from 'suomifi-ui-components';
import { Mode } from 'types/enumTypes';

type RhfCheckboxProps = CheckboxProps & {
  id: string;
  name: string;
  children: React.ReactNode;
  hideInViewMode?: boolean;
  mode: Mode;
  afterClick?: () => void;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function RhfCheckbox(props: RhfCheckboxProps & UseControllerProps<any>): React.ReactElement | null {
  const { field } = useController(props);
  const { control, afterClick, hideInViewMode, mode, ...rest } = props;

  function onClick() {
    field.onChange(!field.value);
    afterClick?.();
  }

  if (mode === 'view') {
    return hideInViewMode ? null : <RhfReadOnlyField value={props.children} id={props.id} />;
  }

  return (
    <Checkbox {...rest} id={props.id} checked={field.value} onClick={onClick}>
      {props.children}
    </Checkbox>
  );
}
