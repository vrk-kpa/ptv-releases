import React from 'react';
import { Control, useWatch } from 'react-hook-form';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { OpenHoursBetweenDates } from './OpenHoursBetweenDates';
import { OpenHoursDay } from './OpenHoursDay';

type OpenHoursSelectorProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  onChanged: () => void;
};

export function OpenHoursSelector(props: OpenHoursSelectorProps): React.ReactElement | null {
  const validityPeriod = useWatch({
    control: props.control,
    name: `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.validityPeriod}`,
  });

  if (!validityPeriod) {
    return null;
  }

  return (
    <div>
      {validityPeriod === 'BetweenDates' && (
        <FormBlock marginTop='10px'>
          <OpenHoursBetweenDates control={props.control} hourIndex={props.hourIndex} onChanged={props.onChanged} />
        </FormBlock>
      )}
      {validityPeriod === 'Day' && (
        <FormBlock marginTop='10px'>
          <OpenHoursDay control={props.control} hourIndex={props.hourIndex} onChanged={props.onChanged} />
        </FormBlock>
      )}
    </div>
  );
}
