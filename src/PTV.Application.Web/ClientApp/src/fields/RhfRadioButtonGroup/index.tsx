import React, { ReactElement, ReactNode, cloneElement, isValidElement, useMemo, useState } from 'react';
import { UseControllerProps, useController } from 'react-hook-form';
import { styled } from '@mui/material/styles';
import { RhfReadOnlyField } from 'fields';
import { Label, RadioButton, RadioButtonGroup } from 'suomifi-ui-components';
import { Mode } from 'types/enumTypes';

export type RadioOption = {
  value: string;
  text: string;
  hintText?: string;
};

type RhfRadioButtonGroupProps<T> = {
  id: string;
  name: string;
  labelText: string;
  tooltipComponent?: ReactElement;
  hintText?: string;
  items: RadioOption[];
  disabled?: boolean;
  mode: Mode;
  labelMode?: 'hidden' | 'visible';
  toFieldValue?: (str: string) => T;
  toRadioButtonValue?: (value: T) => string;
  onChanged?: (value: T) => void;
};

const StyledDiv = styled('div')(() => ({
  '& .fi-label-text.custom-label': {
    display: 'inline',
    verticalAlign: 'middle',
    marginBottom: '10px',
    '& .fi-label-text_label-span': {
      display: 'inline',
    },
  },
}));

export const NoValue = 'no-value';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function RhfRadioButtonGroup<T>(props: RhfRadioButtonGroupProps<T> & UseControllerProps<any>): React.ReactElement {
  const { field } = useController(props);
  const { control, toFieldValue, toRadioButtonValue, onChanged, mode, tooltipComponent, id, ...rest } = props;
  const [wrapperlRef, setWrapperlRef] = useState<HTMLDivElement | null>(null);

  function getTooltipComponent(tooltipComponent: ReactElement | undefined): ReactNode {
    if (isValidElement(tooltipComponent)) {
      return cloneElement(tooltipComponent, {
        anchorElement: wrapperlRef,
      });
    }
    return null;
  }

  function onRadioChange(newValue: string) {
    const value = toFieldValue ? toFieldValue(newValue) : (newValue as unknown as T);
    field.onChange(value);
    onChanged?.(value);
  }

  const radiobuttons = useMemo(() => {
    return props.items.map((item: RadioOption, index: number) => (
      <RadioButton
        id={index === 0 ? props.id : undefined}
        key={item.value}
        value={item.value}
        hintText={item.hintText}
        disabled={props.disabled}
      >
        {item.text}
      </RadioButton>
    ));
  }, [props.items, props.id, props.disabled]);

  const value = toRadioButtonValue ? toRadioButtonValue(field.value) : field.value;

  if (mode === 'view') {
    const selectedItem = props.items.find((x) => x.value === field.value);
    const selectedValue = selectedItem?.text || '';
    return <RhfReadOnlyField tooltipComponent={tooltipComponent} labelText={props.labelText} id={props.id} value={selectedValue} />;
  }

  return (
    <StyledDiv ref={(ref) => setWrapperlRef(ref)}>
      <Label className='custom-label'>{props.labelText}</Label>
      {!!tooltipComponent && getTooltipComponent(tooltipComponent)}
      <RadioButtonGroup {...rest} labelMode='hidden' {...field} onChange={onRadioChange} value={value}>
        {radiobuttons}
      </RadioButtonGroup>
    </StyledDiv>
  );
}
