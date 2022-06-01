import React, { FunctionComponent } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { Heading, Paragraph } from 'suomifi-ui-components';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { useFormMetaContext } from 'context/formMeta';
import { LifeEventsSelect } from './LifeEventsSelect';

interface LifeEventsInterface {
  gdItems: string[];
  control: Control<ServiceModel>;
}

export const ServiceLifeEvents: FunctionComponent<LifeEventsInterface> = ({ gdItems, control }) => {
  const { t } = useTranslation();
  const { mode } = useFormMetaContext();
  const labelText =
    mode === 'edit' ? t('Ptv.Service.Form.Field.LifeEvents.Select.Label') : t('Ptv.Service.Form.Field.LifeEvents.Selection.Label');

  return (
    <Box>
      <Heading variant='h4'>{t(`Ptv.Service.Form.Field.LifeEvents.Label`)}</Heading>
      {mode === 'edit' && (
        <Box mt={2}>
          <Paragraph>{t(`Ptv.Service.Form.Field.LifeEvents.Description`)}</Paragraph>
        </Box>
      )}
      <Box mt={2}>
        <LifeEventsSelect control={control} labelText={labelText} gdItems={gdItems} />
      </Box>
    </Box>
  );
};
