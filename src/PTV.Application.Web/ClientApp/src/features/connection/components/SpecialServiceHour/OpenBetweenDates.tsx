import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { TempDateField } from 'features/connection/components/TempDateField';
import { TimeSelect } from 'features/connection/components/TimeSelect';
import { toFieldId } from 'features/connection/utils/fieldid';

type OpenBetweenDatesProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  validate: () => void;
};

export function OpenBetweenDates(props: OpenBetweenDatesProps): React.ReactElement {
  const { t } = useTranslation();

  const fromDate = `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.openingHoursFrom}`;
  const toDate = `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.openingHoursTo}`;
  const fromTime = `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.timeFrom}`;
  const toTime = `${cC.specialOpeningHours}.${props.hourIndex}.${cHour.timeTo}`;

  return (
    <div>
      <Grid container>
        <Grid item>
          <FormBlock marginRight='15px'>
            <TempDateField
              id={toFieldId(fromDate)}
              name={fromDate}
              control={props.control}
              label={t('Ptv.Common.StartDay')}
              onChanged={props.validate}
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
            onChanged={props.validate}
          />
        </Grid>
      </Grid>

      <FormBlock marginTop='20px'>
        <Grid item container>
          <Grid item>
            <FormBlock marginRight='15px'>
              <TempDateField
                id={toFieldId(toDate)}
                name={toDate}
                control={props.control}
                label={t('Ptv.Common.EndDay')}
                onChanged={props.validate}
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
