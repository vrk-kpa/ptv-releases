import React from 'react';
import { Control, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { ConnectionFormModel, cC, cHour } from 'types/forms/connectionFormTypes';
import { VisualHeading } from 'components/VisualHeading';
import { FormBlock } from 'features/connection/components/FormLayout';
import { OpenHoursSelector } from './OpenHoursSelector';
import { Title } from './Title';
import { Validity } from './Validity';
import { ValidityDatesSelector } from './ValidityDatesSelector';

const useStyles = makeStyles(() => ({
  root: {
    '& p.noTopMargin': {
      marginTop: 0,
    },
  },
}));

type ExceptionalServiceHourProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function ExceptionalServiceHour(props: ExceptionalServiceHourProps): React.ReactElement {
  const { t } = useTranslation();

  const classes = useStyles();

  function reValidate() {
    props.trigger([
      `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.openingHoursFrom}`,
      `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.openingHoursTo}`,
      `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.timeFrom}`,
      `${cC.exceptionalOpeningHours}.${props.hourIndex}.${cHour.timeTo}`,
    ]);
  }

  return (
    <div className={classes.root}>
      <Grid container direction='column'>
        <Grid item>
          <VisualHeading className='noTopMargin' variant='h4'>
            {t('Ptv.ConnectionDetails.ExceptionalServiceHour.MainTitle')}
          </VisualHeading>
        </Grid>

        <Grid item>
          <FormBlock marginTop='20px'>
            <Validity control={props.control} hourIndex={props.hourIndex} onChanged={reValidate} />
          </FormBlock>
        </Grid>
        <Grid item>
          <FormBlock marginTop='10px'>
            <ValidityDatesSelector control={props.control} hourIndex={props.hourIndex} onChanged={reValidate} />
          </FormBlock>
        </Grid>
        <Grid item>
          <FormBlock marginTop='20px'>
            <OpenHoursSelector control={props.control} hourIndex={props.hourIndex} onChanged={reValidate} />
          </FormBlock>
        </Grid>
        <Grid item>
          <FormBlock marginTop='20px'>
            <Title control={props.control} hourIndex={props.hourIndex} />
          </FormBlock>
        </Grid>
      </Grid>
    </div>
  );
}
