import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { TimeSelect } from 'features/connection/components/TimeSelect';

type TimesSelectProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  validate: () => void;
};

export function TimesSelect(props: TimesSelectProps): React.ReactElement {
  const { t } = useTranslation();
  const fromFieldName = `${cC.holidayOpeningHours}.${props.hourIndex}.${cHour.from}`;
  const toFieldName = `${cC.holidayOpeningHours}.${props.hourIndex}.${cHour.to}`;

  return (
    <div>
      <FormBlock display='inline' marginRight='15px'>
        <TimeSelect
          control={props.control}
          name={fromFieldName}
          type='StartTime'
          onChanged={props.validate}
          labelText={t('Ptv.ConnectionDetails.HolidayServiceHour.StartTime.Label')}
        />
      </FormBlock>

      <TimeSelect
        control={props.control}
        name={toFieldName}
        type='EndTime'
        labelText={t('Ptv.ConnectionDetails.HolidayServiceHour.EndTime.Label')}
        onChanged={props.validate}
      />
    </div>
  );
}
