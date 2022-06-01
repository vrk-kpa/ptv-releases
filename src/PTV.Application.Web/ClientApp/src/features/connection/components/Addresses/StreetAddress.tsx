import React, { useState } from 'react';
import { useController, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid, ThemeOptions } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { StreetModel } from 'types/api/streetModel';
import { cAddress, cC } from 'types/forms/connectionFormTypes';
import { FormNotification, FormNotificationTypes } from 'components/FormNotification';
import { useFormMetaContext } from 'context/formMeta';
import { useGetValidateAddress } from 'hooks/queries/address/usetGetValidateAddress';
import { getTextByLangPriority } from 'utils/translations';
import { FormBlock } from 'features/connection/components/FormLayout';
import { AdditionalInfo } from './AdditionalInfo';
import { AddressProps } from './Address';
import { PostalCodeAndOfficeSelector } from './PostalCodeAndOfficeSelector';
import { StreetNameInput } from './StreetNameInput';
import { StreetNumber } from './StreetNumber';

const useStyles = makeStyles<ThemeOptions>((theme) => ({
  addressNotificationInfo: {
    marginTop: '20px',
    marginBottom: '20px',
  },
  addressWrapper: {
    marginTop: '20px',
  },
}));

function StreetAddress(props: AddressProps): React.ReactElement {
  const classes = useStyles();
  const { t } = useTranslation();
  const meta = useFormMetaContext();
  const [addressNotification, setAddressNotification] = useState(false);

  const { field } = useController({
    name: `${cC.addresses}.${props.addressIndex}.${cAddress.street}`,
    control: props.control,
  });

  const streetName = useWatch({ control: props.control, name: `${cC.addresses}.${props.addressIndex}.${cAddress.streetName}` });
  const streetNumber = useWatch({ control: props.control, name: `${cC.addresses}.${props.addressIndex}.${cAddress.streetNumber}` });
  const postalCode = useWatch({ control: props.control, name: `${cC.addresses}.${props.addressIndex}.${cAddress.postalCode}` });

  const handleDataUpdate = (newData: StreetModel): void => {
    if (!newData || (newData && !newData.isValid)) {
      setAddressNotification(true);
      field.onChange({ names: streetName, municipalityCode: '', isValid: false, streetNumbers: [] } as StreetModel);
    } else if (newData && newData.isValid) {
      setAddressNotification(false);
      field.onChange(newData);
    }
  };

  const handleDataError = (): void => {
    field.onChange(null);
  };

  const { isLoading, data } = useGetValidateAddress(
    {
      streetName: getTextByLangPriority(meta.selectedLanguageCode, streetName) || '',
      streetNumber,
      postalCode: postalCode || '',
    },
    {
      enabled: !!getTextByLangPriority(meta.selectedLanguageCode, streetName) && !!postalCode,
      refetchOnMount: true,
      onSuccess: handleDataUpdate,
      onError: handleDataError,
    }
  );

  return (
    <FormBlock className={classes.addressWrapper}>
      <Grid container spacing={1}>
        <Grid item md={6} xs={12}>
          <StreetNameInput {...props} />
        </Grid>
        <Grid item md={6} xs={12}>
          <StreetNumber isOptional={true} {...props} />
        </Grid>
        <Grid item sm={12}>
          <PostalCodeAndOfficeSelector {...props} />
        </Grid>
        {addressNotification && !isLoading && (
          <Grid className={classes.addressNotificationInfo} item xs={12}>
            <FormNotification
              notificationType={FormNotificationTypes.Warning}
              text={t('Ptv.ConnectionDetails.Address.Notification.AddressNotInDatabase')}
            ></FormNotification>
          </Grid>
        )}
        {!addressNotification && !isLoading && data && (
          <Grid className={classes.addressNotificationInfo} item xs={12}>
            <FormNotification
              notificationType={FormNotificationTypes.Info}
              text={t('Ptv.ConnectionDetails.Address.Notification.AddressFound')}
            ></FormNotification>
          </Grid>
        )}
        <Grid item xs={12}>
          <AdditionalInfo {...props} />
        </Grid>
      </Grid>
    </FormBlock>
  );
}

export { StreetAddress };
