import React from 'react';
import { Control } from 'react-hook-form';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { FaxNumberDialCode } from './FaxNumberDialCode';
import { FaxNumberInput } from './FaxNumberInput';
import { FaxNumberPreview } from './FaxNumberPreview';

const useStyles = makeStyles(() => ({
  formBlock: {
    marginTop: '20px',
  },
}));

type FaxNumberProps = {
  faxNumberIndex: number;
  control: Control<ConnectionFormModel>;
  language: Language;
};

export function FaxNumber(props: FaxNumberProps): React.ReactElement {
  const classes = useStyles();

  return (
    <Grid>
      <Grid item>
        <FaxNumberDialCode control={props.control} faxNumberIndex={props.faxNumberIndex} language={props.language} />
      </Grid>
      <Grid item>
        <FormBlock className={classes.formBlock}>
          <FaxNumberInput control={props.control} faxNumberIndex={props.faxNumberIndex} language={props.language} />
        </FormBlock>
      </Grid>
      <Grid item>
        <FormBlock className={classes.formBlock}>
          <FaxNumberPreview control={props.control} phoneNumberIndex={props.faxNumberIndex} language={props.language} />
        </FormBlock>
      </Grid>
    </Grid>
  );
}
