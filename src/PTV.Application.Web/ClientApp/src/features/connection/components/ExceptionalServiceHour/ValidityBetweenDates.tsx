import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { TempDateField } from 'features/connection/components/TempDateField';
import { TimeSelect } from 'features/connection/components/TimeSelect';
import { toFieldId } from 'features/connection/utils/fieldid';

type ValidityBetweenDatesProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  onChanged: () => void;
};

export function ValidityBetweenDates(props: ValidityBetweenDatesProps): React.ReactElement {
  const { t } = useTranslation();

  const fromDay = `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.openingHoursFrom}`;
  const toDay = `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.openingHoursTo}`;
  const fromTime = `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.timeFrom}`;
  const toTime = `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.timeTo}`;

  return (
    <div>
      <Grid container>
        <Grid item container>
          <Grid item>
            <FormBlock marginRight='15px'>
              <TempDateField
                id={toFieldId(fromDay)}
                name={fromDay}
                control={props.control}
                label={t('Ptv.Common.StartDay')}
                onChanged={props.onChanged}
              />
            </FormBlock>
          </Grid>
          <Grid item>
            <TimeSelect
              id={toFieldId(fromTime)}
              name={fromTime}
              control={props.control}
              labelText={t('Ptv.Common.Time')}
              type='StartTime'
              onChanged={props.onChanged}
            />
          </Grid>
        </Grid>
      </Grid>

      <Grid container>
        <Grid item container>
          <Grid item>
            <FormBlock marginRight='15px'>
              <TempDateField
                id={toFieldId(toDay)}
                name={toDay}
                control={props.control}
                label={t('Ptv.Common.EndDay')}
                onChanged={props.onChanged}
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
              onChanged={props.onChanged}
            />
          </Grid>
        </Grid>
      </Grid>
    </div>
  );
}
