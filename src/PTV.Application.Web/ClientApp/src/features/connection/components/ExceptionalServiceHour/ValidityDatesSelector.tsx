import React from 'react';
import { Control, useWatch } from 'react-hook-form';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { ValidityBetweenDates } from './ValidityBetweenDates';
import { ValidityDay } from './ValidityDay';

type ValidityDatesSelectorProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  onChanged: () => void;
};

export function ValidityDatesSelector(props: ValidityDatesSelectorProps): React.ReactElement | null {
  const validityPeriod = useWatch({
    control: props.control,
    name: `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.validityPeriod}`,
  });

  if (validityPeriod === 'BetweenDates') {
    return (
      <FormBlock marginTop='15px'>
        <ValidityBetweenDates control={props.control} hourIndex={props.hourIndex} onChanged={props.onChanged} />
      </FormBlock>
    );
  }

  if (validityPeriod === 'Day') {
    return (
      <FormBlock marginTop='15px'>
        <ValidityDay control={props.control} hourIndex={props.hourIndex} onChanged={props.onChanged} />
      </FormBlock>
    );
  }

  return null;
}
