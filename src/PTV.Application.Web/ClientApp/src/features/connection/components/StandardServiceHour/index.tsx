import React from 'react';
import { Control, UseFormTrigger, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { VisualHeading } from 'components/VisualHeading';
import { FormBlock } from 'features/connection/components/FormLayout';
import { DatePeriod } from './DatePeriod';
import { OpeningType } from './OpeningType';
import { Title } from './Title';
import { Validity } from './Validity';
import { Weekdays } from './Weekdays';

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    flexGrow: 1,
    '& p.noTopMargin': {
      marginTop: 0,
    },
  },
}));

type StandardServiceHourProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function StandardServiceHour(props: StandardServiceHourProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  const openingType = useWatch({
    name: `${cC.standardOpeningHours}.${props.hourIndex}.${cHour.openingType}` as const,
    control: props.control,
  });

  const showWeekDays = openingType === 'DaysAndTimes';

  return (
    <div className={classes.root}>
      <Grid container direction='column'>
        <Grid item>
          <VisualHeading className='noTopMargin' variant='h4'>
            {t('Ptv.ConnectionDetails.StandardServiceHour.MainTitle')}
          </VisualHeading>
        </Grid>
        <Grid item>
          <FormBlock marginTop='20px'>
            <Title control={props.control} hourIndex={props.hourIndex} />
          </FormBlock>
        </Grid>
        <Grid item>
          <FormBlock marginTop='20px'>
            <Validity control={props.control} hourIndex={props.hourIndex} />
          </FormBlock>
        </Grid>
        <Grid item>
          <DatePeriod control={props.control} hourIndex={props.hourIndex} trigger={props.trigger} />
        </Grid>
        <Grid item>
          <FormBlock marginTop='20px'>
            <OpeningType control={props.control} hourIndex={props.hourIndex} />
          </FormBlock>
        </Grid>
        <Grid item>
          {showWeekDays && (
            <FormBlock marginTop='20px'>
              <Weekdays control={props.control} trigger={props.trigger} hourIndex={props.hourIndex} />
            </FormBlock>
          )}
        </Grid>
      </Grid>
    </div>
  );
}
