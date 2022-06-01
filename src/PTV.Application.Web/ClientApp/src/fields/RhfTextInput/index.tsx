import React, { ReactElement, ReactNode, cloneElement, isValidElement, useState } from 'react';
import { UseControllerProps, useController } from 'react-hook-form';
import { styled } from '@mui/material/styles';
import { RhfReadOnlyField } from 'fields';
import { Label, TextInput, TextInputProps } from 'suomifi-ui-components';
import { Mode } from 'types/enumTypes';
import { toFieldStatus } from 'utils/rhf';

type RhfTextInputProps = TextInputProps & {
  name: string;
  id: string;
  mode: Mode;
  tooltipComponent?: ReactElement;
  asLink?: boolean;
  onChanged?: () => void;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const RhfTextInput = styled((props: RhfTextInputProps & UseControllerProps<any>): React.ReactElement => {
  const { control, onChanged, onChange, mode, asLink, tooltipComponent, className, optionalText, ...rest } = props;
  const { field, fieldState } = useController(props);
  const { status, statusText } = toFieldStatus(fieldState);
  const [wrapperRef, setWrapperlRef] = useState<HTMLDivElement | null>(null);

  function onTextChange(newValue: string | number | undefined) {
    field.onChange(newValue);
    props.onChanged?.();
  }

  function getTooltipComponent(tooltipComponent: ReactElement | undefined): ReactNode {
    if (isValidElement(tooltipComponent)) {
      return cloneElement(tooltipComponent, {
        anchorElement: wrapperRef,
      });
    }
    return null;
  }

  if (mode === 'view') {
    return (
      <RhfReadOnlyField tooltipComponent={tooltipComponent} value={field.value} labelText={props.labelText} id={props.id} asLink={asLink} />
    );
  }

  return (
    <div className={className} ref={(ref) => setWrapperlRef(ref)}>
      <Label className='custom-label' optionalText={optionalText}>
        {props.labelText}
      </Label>
      {!!tooltipComponent && getTooltipComponent(tooltipComponent)}
      <TextInput fullWidth={true} {...rest} {...field} status={status} labelMode='hidden' statusText={statusText} onChange={onTextChange} />
    </div>
  );
})(() => ({
  '& .fi-label-text.custom-label': {
    display: 'inline',
    verticalAlign: 'middle',
    '& .fi-label-text_label-span': {
      display: 'inline',
    },
  },
  '& .fi-text-input': {
    marginTop: '10px',
  },
}));
