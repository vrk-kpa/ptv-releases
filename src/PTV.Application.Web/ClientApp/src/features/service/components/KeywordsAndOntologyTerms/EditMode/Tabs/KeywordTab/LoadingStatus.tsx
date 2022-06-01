import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { Text } from 'suomifi-ui-components';
import LoadingIndicator from 'components/LoadingIndicator';

export function LoadingStatus(): React.ReactElement {
  const { t } = useTranslation();
  return (
    <Grid container justifyContent='center' alignItems='center' direction='column'>
      <Grid item>
        <LoadingIndicator />
      </Grid>
      <Grid item>
        <Text>{t('Ptv.Service.Form.Field.FreeKeywords.Checking.Message')}</Text>
      </Grid>
    </Grid>
  );
}
