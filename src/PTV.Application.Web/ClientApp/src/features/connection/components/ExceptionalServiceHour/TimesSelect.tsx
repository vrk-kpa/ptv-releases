import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { TimeSelect } from 'features/connection/components/TimeSelect';

type TimesSelectProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  onChanged: () => void;
};

export function TimesSelect(props: TimesSelectProps): React.ReactElement {
  const { t } = useTranslation();
  const fromFieldName = `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.timeFrom}`;
  const toFieldName = `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.timeTo}`;
  return (
    <div>
      <FormBlock display='inline' marginRight='15px'>
        <TimeSelect
          control={props.control}
          name={fromFieldName}
          type='StartTime'
          visualPlaceholder={t('Ptv.Common.Choose')}
          labelText={t('Ptv.ConnectionDetails.HolidayServiceHour.StartTime.Label')}
          onChanged={props.onChanged}
        />
      </FormBlock>

      <TimeSelect
        control={props.control}
        name={toFieldName}
        type='EndTime'
        visualPlaceholder={t('Ptv.Common.Choose')}
        labelText={t('Ptv.ConnectionDetails.HolidayServiceHour.EndTime.Label')}
        onChanged={props.onChanged}
      />
    </div>
  );
}
