import React from 'react';
import { Control, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { VisualHeading } from 'components/VisualHeading';
import { FormBlock } from 'features/connection/components/FormLayout';
import { OpeningType } from './OpeningType';
import { Title } from './Title';

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    flexGrow: 1,
    '& p.noTopMargin': {
      marginTop: 0,
    },
  },
}));

type SpecialServiceHourProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  trigger: UseFormTrigger<any>;
};

export function SpecialServiceHour(props: SpecialServiceHourProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  return (
    <div className={classes.root}>
      <Grid container direction='column'>
        <Grid item>
          <VisualHeading className='noTopMargin' variant='h4'>
            {t('Ptv.ConnectionDetails.SpecialServiceHour.MainTitle')}
          </VisualHeading>
        </Grid>
        <Grid item>
          <FormBlock marginTop='30px'>
            <Title control={props.control} hourIndex={props.hourIndex} />
          </FormBlock>
        </Grid>
        <Grid item>
          <FormBlock marginTop='20px'>
            <OpeningType control={props.control} hourIndex={props.hourIndex} trigger={props.trigger} />
          </FormBlock>
        </Grid>
      </Grid>
    </div>
  );
}
