import React from 'react';
import { Control, UseFormTrigger, useWatch } from 'react-hook-form';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { ConnectionFormModel, cAddress, cC } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { AdditionalInfo } from './AdditionalInfo';
import { AddressTypeSelector } from './AddressTypeSelector';
import { CountrySelector } from './CountrySelector';
import { ForeignAdditionalInfo } from './ForeignAdditionalInfo';
import { PoBox } from './PoBox';
import { PostalCodeAndOfficeSelector } from './PostalCodeAndOfficeSelector';
import { StreetAddress } from './StreetAddress';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    flexGrow: 1,
    flexDirection: 'column',
  },
}));

export type AddressProps = {
  addressIndex: number;
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

function Address(props: AddressProps): React.ReactElement {
  const classes = useStyles();

  const addressType = useWatch({
    name: `${cC.addresses}.${props.addressIndex}.${cAddress.type}`,
    control: props.control,
  });

  return (
    <div className={classes.root}>
      <Grid container>
        <Grid item>
          <Grid item>
            <AddressTypeSelector control={props.control} addressIndex={props.addressIndex} trigger={props.trigger} />
          </Grid>
        </Grid>
      </Grid>
      {addressType === 'Street' && <StreetAddress {...props} />}
      {addressType === 'PostOfficeBox' && (
        <div>
          <FormBlock marginTop='20px'>
            <Grid container>
              <Grid item sm={8} xs={12}>
                <PoBox control={props.control} addressIndex={props.addressIndex} />
              </Grid>
            </Grid>
          </FormBlock>
          <FormBlock marginTop='20px'>
            <Grid container wrap='wrap' justifyContent='space-between'>
              <Grid item sm={7} xs={12}>
                <PostalCodeAndOfficeSelector {...props} />
              </Grid>
            </Grid>
          </FormBlock>
          <FormBlock marginTop='20px'>
            <AdditionalInfo control={props.control} addressIndex={props.addressIndex} />
          </FormBlock>
        </div>
      )}
      {addressType === 'Foreign' && (
        <div>
          <FormBlock marginTop='20px'>
            <ForeignAdditionalInfo control={props.control} addressIndex={props.addressIndex} />
          </FormBlock>
          <FormBlock marginTop='20px' width='50%'>
            <CountrySelector control={props.control} addressIndex={props.addressIndex} />
          </FormBlock>
        </div>
      )}
    </div>
  );
}

export { Address };
