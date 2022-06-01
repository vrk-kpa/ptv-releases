import React from 'react';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { Message } from 'components/Message';

export default function Note({ serviceHasBeenSaved }: { serviceHasBeenSaved: boolean }): React.ReactElement {
  const { t } = useTranslation();

  const msg = serviceHasBeenSaved
    ? t('Ptv.Service.Form.ServiceChannelSelector.Note.Info')
    : t('Ptv.Service.Form.ServiceChannelSelector.Note.Error');
  const msgType = serviceHasBeenSaved ? 'info' : 'error';
  return (
    <Box mb={2}>
      <Message type={msgType}>{msg}</Message>
    </Box>
  );
}
