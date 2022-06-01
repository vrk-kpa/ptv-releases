import React, { useMemo } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfRadioButtonGroup } from 'fields';
import { ConnectionFormModel, ExceptionalHourValidityPeriod, cHour } from 'types/forms/connectionFormTypes';
import { getExceptionalHourFieldName, toFieldId } from 'features/connection/utils/fieldid';

const Day: ExceptionalHourValidityPeriod = 'Day';
const BetweenDates: ExceptionalHourValidityPeriod = 'BetweenDates';

type ValidityProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  onChanged: () => void;
};

export function Validity(props: ValidityProps): React.ReactElement {
  const { t } = useTranslation();

  const items = useMemo(() => {
    return [
      {
        value: Day,
        text: t('Ptv.ConnectionDetails.ExceptionalServiceHour.Validity.Option.Day.Label'),
      },
      {
        value: BetweenDates,
        text: t('Ptv.ConnectionDetails.ExceptionalServiceHour.Validity.Option.BetweenDates.Label'),
      },
    ];
  }, [t]);

  const fieldName = getExceptionalHourFieldName(props.hourIndex, cHour.validityPeriod);

  return (
    <RhfRadioButtonGroup<ExceptionalHourValidityPeriod>
      control={props.control}
      id={toFieldId(fieldName)}
      name={fieldName}
      mode='edit'
      labelText={t('Ptv.ConnectionDetails.ExceptionalServiceHour.Validity.Label')}
      items={items}
      onChanged={props.onChanged}
    />
  );
}
