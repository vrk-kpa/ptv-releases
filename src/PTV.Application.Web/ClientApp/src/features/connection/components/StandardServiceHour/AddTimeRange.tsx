import React from 'react';
import { Control, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Button } from 'suomifi-ui-components';
import { ConnectionFormModel, cC, cDaily, cHour } from 'types/forms/connectionFormTypes';
import ValidationMessage from 'components/ValidationMessage';
import { toFieldStatus } from 'utils/rhf';
import { FormBlock } from 'features/connection/components/FormLayout';
import { toFieldId } from 'features/connection/utils/fieldid';
import { MaxDailyOpeningTimes } from 'features/connection/validation/standardHours';

type AddTimeRangeProps = {
  hourIndex: number;
  dayIndex: number;
  control: Control<ConnectionFormModel>;
  timeRangeCount: number;
  add: () => void;
};

export function AddTimeRange(props: AddTimeRangeProps): React.ReactElement {
  const { t } = useTranslation();
  const id = toFieldId(`${cC.standardOpeningHours}.${props.hourIndex}.${cHour.dailyOpeningTimes}.${props.dayIndex}.add-time-range`);

  const { fieldState } = useController({
    control: props.control,
    name: `${cC.standardOpeningHours}.${props.hourIndex}.${cHour.dailyOpeningTimes}.${props.dayIndex}.${cDaily.day}` as const,
  });

  const { status, statusText } = toFieldStatus(fieldState);
  const hasError = status === 'error';
  const disabled = hasError || props.timeRangeCount >= MaxDailyOpeningTimes;
  return (
    <>
      <Button
        id={id}
        variant='secondary'
        onClick={props.add}
        disabled={disabled}
        aria-labelledby={t('Ptv.ConnectionDetails.StandardServiceHour.TimeRanges.AddNew.Description')}
      >
        {t('Ptv.ConnectionDetails.StandardServiceHour.TimeRanges.AddNew.Label')}
      </Button>
      {hasError && (
        <FormBlock marginTop='10px'>
          <ValidationMessage message={statusText} />
        </FormBlock>
      )}
    </>
  );
}
