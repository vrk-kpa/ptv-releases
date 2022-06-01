import React from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { Text } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cC } from 'types/forms/connectionFormTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { getDialCodeOrDefault } from 'utils/phoneNumbers';

type FaxNumberPreviewProps = {
  phoneNumberIndex: number;
  control: Control<ConnectionFormModel>;
  language: Language;
};

export function FaxNumberPreview(props: FaxNumberPreviewProps): React.ReactElement {
  const { t } = useTranslation();
  const appContext = useAppContextOrThrow();

  const faxNumber = useWatch({
    name: `${cC.faxNumbers}.${props.language}.${props.phoneNumberIndex}`,
    control: props.control,
  });

  const dialCode = getDialCodeOrDefault(faxNumber.dialCodeId, appContext.staticData.dialCodes);
  const preview = dialCode ? `(${dialCode.code}) ${faxNumber.number}` : `${faxNumber.number}`;

  return (
    <Grid container direction='column'>
      <Grid item>
        <Text smallScreen={true} variant='bold'>
          {t('Ptv.ConnectionDetails.FaxNumber.Preview.Label')}
        </Text>
      </Grid>
      <Grid item>
        <Text smallScreen={true}>{preview}</Text>
      </Grid>
    </Grid>
  );
}
