import React from 'react';
import { Control, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { Button } from 'suomifi-ui-components';
import { ConnectionFormModel, cC, cDaily, cHour } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { TimeSelect } from 'features/connection/components/TimeSelect';
import { getTimeRangeFromFieldName, getTimeRangeToFieldName, toFieldId } from 'features/connection/utils/fieldid';

type TimeRangeProps = {
  dayIndex: number;
  hourIndex: number;
  timeIndex: number;
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
  remove: () => void;
};

export function TimeRange(props: TimeRangeProps): React.ReactElement {
  const { t } = useTranslation();
  const fromFieldName = getTimeRangeFromFieldName(props.hourIndex, props.dayIndex, props.timeIndex);
  const toFieldName = getTimeRangeToFieldName(props.hourIndex, props.dayIndex, props.timeIndex);

  function triggerValidation() {
    props.trigger(undefined);
  }

  return (
    <Grid container alignItems='baseline'>
      <Grid item>
        <FormBlock marginRight='15px'>
          <TimeSelect
            control={props.control}
            name={fromFieldName}
            type='StartTime'
            onChanged={triggerValidation}
            labelText={t('Ptv.ConnectionDetails.StandardServiceHour.StartTime.Label')}
          />
        </FormBlock>
      </Grid>
      <Grid item>
        <TimeSelect
          control={props.control}
          name={toFieldName}
          type='EndTime'
          onChanged={triggerValidation}
          labelText={t('Ptv.ConnectionDetails.StandardServiceHour.EndTime.Label')}
        />
      </Grid>
      <Grid item>
        <Button
          id={toFieldId(
            `${cC.standardOpeningHours}.${props.hourIndex}.${cHour.dailyOpeningTimes}.${props.dayIndex}.${cDaily.times}.${props.timeIndex}.remove-time-range`
          )}
          variant='secondaryNoBorder'
          icon='remove'
          onClick={props.remove}
        >
          {t('Ptv.Common.Remove')}
        </Button>
      </Grid>
    </Grid>
  );
}
