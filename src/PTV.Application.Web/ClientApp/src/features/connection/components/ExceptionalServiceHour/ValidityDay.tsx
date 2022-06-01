import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { TempDateField } from 'features/connection/components/TempDateField';
import { toFieldId } from 'features/connection/utils/fieldid';

type ValidityDayProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  onChanged: () => void;
};

export function ValidityDay(props: ValidityDayProps): React.ReactElement {
  const { t } = useTranslation();

  const fromDay = `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.openingHoursFrom}`;

  return (
    <TempDateField
      id={toFieldId(fromDay)}
      name={fromDay}
      control={props.control}
      label={t('Ptv.ConnectionDetails.ExceptionalServiceHour.TimePeriod.Day.Label')}
      onChanged={props.onChanged}
    />
  );
}
