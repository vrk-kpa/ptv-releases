import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfRadioButtonGroup } from 'fields';
import { ConnectionFormModel, ValidityPeriod, cC, cHour } from 'types/forms/connectionFormTypes';
import { getStdHourFieldName, toFieldId } from 'features/connection/utils/fieldid';

const UntilFurtherNotice: ValidityPeriod = 'UntilFurtherNotice';
const BetweenDates: ValidityPeriod = 'BetweenDates';

type ValidityProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
};

export function Validity(props: ValidityProps): React.ReactElement {
  const { t } = useTranslation();

  const items = [
    {
      value: UntilFurtherNotice,
      text: t('Ptv.ConnectionDetails.StandardServiceHour.Validity.Option.UntilFurtherNotice'),
    },
    {
      value: BetweenDates,
      text: t('Ptv.ConnectionDetails.StandardServiceHour.Validity.Option.BetweenDates'),
    },
  ];

  const fieldName = getStdHourFieldName(props.hourIndex, cHour.validityPeriod);

  const deps = [
    `${cC.standardOpeningHours}.${props.hourIndex}.${cHour.openingHoursFrom}`,
    `${cC.standardOpeningHours}.${props.hourIndex}.${cHour.openingHoursTo}`,
  ];

  return (
    <RhfRadioButtonGroup<ValidityPeriod>
      control={props.control}
      id={toFieldId(fieldName)}
      name={fieldName}
      mode='edit'
      labelText={t('Ptv.ConnectionDetails.StandardServiceHour.Validity.Label')}
      items={items}
      rules={{ deps: deps }}
    />
  );
}
