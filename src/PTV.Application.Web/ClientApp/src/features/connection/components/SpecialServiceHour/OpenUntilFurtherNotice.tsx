import React from 'react';
import { Control, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import ValidationMessage from 'components/ValidationMessage';
import { toFieldStatus } from 'utils/rhf';
import { FormBlock } from 'features/connection/components/FormLayout';
import { TimeSelect } from 'features/connection/components/TimeSelect';
import { toFieldId } from 'features/connection/utils/fieldid';
import { DaySelect } from './DaySelect';

type OpenUntilFurtherNoticeProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  validate: () => void;
};

export function OpenUntilFurtherNotice(props: OpenUntilFurtherNoticeProps): React.ReactElement {
  const { t } = useTranslation();

  const { field: fromDayField, fieldState: fromDayFieldState } = useController({
    control: props.control,
    name: `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.dayFrom}`,
  });

  const { field: toDayField } = useController({
    control: props.control,
    name: `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.dayTo}`,
  });

  const { status: dayFromStatus, statusText: dayFromStatusText } = toFieldStatus(fromDayFieldState);

  const fromTime = `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.timeFrom}`;
  const toTime = `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.timeTo}`;

  function onFromDayChange(newValue: string) {
    fromDayField.onChange(newValue);
    props.validate();
  }

  function onToDayChange(newValue: string) {
    toDayField.onChange(newValue);
    props.validate();
  }

  return (
    <div>
      <Grid container>
        <Grid item>
          <FormBlock marginRight='15px'>
            <DaySelect name={fromDayField.name} label={t('Ptv.Common.StartDay')} value={fromDayField.value} onChange={onFromDayChange} />
          </FormBlock>
        </Grid>
        <Grid item>
          <TimeSelect
            id={toFieldId(fromTime)}
            name={fromTime}
            control={props.control}
            labelText={t('Ptv.Common.Time')}
            type='StartTime'
            onChanged={props.validate}
          />
        </Grid>
      </Grid>

      {dayFromStatus === 'error' && <ValidationMessage message={dayFromStatusText} />}

      <FormBlock marginTop='20px'>
        <Grid container>
          <Grid item>
            <FormBlock marginRight='15px'>
              <DaySelect
                name={toDayField.name}
                label={t('Ptv.Common.EndDay')}
                value={toDayField.value || 'Sunday'}
                onChange={onToDayChange}
              />
            </FormBlock>
          </Grid>
          <Grid item>
            <TimeSelect
              id={toFieldId(toTime)}
              name={toTime}
              control={props.control}
              labelText={t('Ptv.Common.Time')}
              type='EndTime'
              onChanged={props.validate}
            />
          </Grid>
        </Grid>
      </FormBlock>
    </div>
  );
}
