import React from 'react';
import { UseControllerProps, useController } from 'react-hook-form';
import { makeStyles } from '@mui/styles';
import { ViewValueList } from 'fields';
import { Checkbox } from 'suomifi-ui-components';
import { Mode } from 'types/enumTypes';
import { Fieldset } from 'fields/Fieldset/Fieldset';
import { Legend } from 'fields/Legend/Legend';

export type CheckboxOption = {
  value: string;
  text: string;
  hintText?: string;
};

type RhfCheckboxGroupProps<T> = {
  id: string;
  name: string;
  labelText: string;
  items: CheckboxOption[];
  hideInViewMode?: boolean;
  mode: Mode;
  toFieldValue: (str: string) => T;
};

const useStyles = makeStyles(() => ({
  checkboxLayout: {
    '&.fi-checkbox': {
      marginBottom: '10px',
    },
  },
}));

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function RhfCheckboxGroup<T>(props: RhfCheckboxGroupProps<T> & UseControllerProps<any>): React.ReactElement | null {
  const { field } = useController(props);
  const fieldValue = field.value as unknown[];

  const classes = useStyles();

  if (props.mode === 'view') {
    const values = props.items.reduce((acc: string[], curr) => (field.value.includes(curr.value) ? [...acc, curr.text] : acc), []);
    return <ViewValueList id={props.id} labelText={props.labelText} values={values} hideInViewMode={props.hideInViewMode} />;
  }

  const handleOnClick = (newValue: string) => {
    const value = props.toFieldValue(newValue);
    if (field.value.includes(value)) {
      field.onChange(fieldValue.filter((x) => x !== value));
    } else {
      field.onChange([...fieldValue, value]);
    }
  };

  return (
    <Fieldset>
      <Legend>{props.labelText}</Legend>
      {props.items.map((item) => {
        const checked = field.value.includes(item.value);
        return (
          <Checkbox
            key={item.value}
            className={classes.checkboxLayout}
            value={item.value}
            onClick={() => handleOnClick(item.value)}
            checked={checked}
            hintText={item.hintText}
          >
            {item.text}
          </Checkbox>
        );
      })}
    </Fieldset>
  );
}
