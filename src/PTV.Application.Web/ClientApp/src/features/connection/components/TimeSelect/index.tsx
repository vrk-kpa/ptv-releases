import React, { useCallback } from 'react';
import { UseControllerProps, useController } from 'react-hook-form';
import { Grid } from '@mui/material';
import { Dropdown, DropdownItem, DropdownProps } from 'suomifi-ui-components';
import ValidationMessage from 'components/ValidationMessage';
import { toFieldStatus } from 'utils/rhf';
import { toFieldId } from 'features/connection/utils/fieldid';
import './styles.css';
import { TimeSelectorType, generateTimespans, getDisplayValue } from './utils';

type TimeSelectProps = {
  type: TimeSelectorType;
  onChanged?: () => void;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function TimeSelect(props: DropdownProps & TimeSelectProps & UseControllerProps<any>): React.ReactElement {
  const { name, id, onChanged, ...rest } = props;

  const { field, fieldState } = useController(props);
  const { status, statusText } = toFieldStatus(fieldState);

  const renderItems = useCallback(() => {
    const elements: React.ReactElement[] = [];
    const timespans = generateTimespans(props.type);
    for (const ts of timespans) {
      const value = ts.toString();
      const displayValue = getDisplayValue(props.type, ts);
      elements.push(
        <DropdownItem key={value} value={value}>
          {displayValue}
        </DropdownItem>
      );
    }

    return elements;
  }, [props.type]);

  function onChange(newValue: string) {
    field.onChange(newValue);
    onChanged?.();
  }

  const items = renderItems();

  // Notes:
  // - If you have element next to this element, the long validation message
  // will push the next element
  // - We have cases where we might want to display longer status text without
  // wrapping it. E.g. see holidays where begin/end times are on different rows
  // - Right now there is no easy way to subscribe to the error information
  // per field basis (react hook form has useWatch for just the field value)
  // - Dropdown itself does not support status/statusText even though all the
  // other components do (might fix the whole issue?)

  return (
    <Grid container direction='column'>
      <Grid item>
        <Dropdown className='dropdown-custom' onChange={onChange} value={field.value} {...rest} id={toFieldId(field.name)}>
          {items}
        </Dropdown>
      </Grid>
      {status === 'error' && (
        <Grid item>
          <ValidationMessage message={statusText} />
        </Grid>
      )}
    </Grid>
  );
}
