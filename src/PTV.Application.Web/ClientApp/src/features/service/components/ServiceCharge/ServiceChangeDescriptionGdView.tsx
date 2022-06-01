import React from 'react';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { Block, Text } from 'suomifi-ui-components';
import { ChargeModel, cCharge } from 'types/forms/chargeType';
import { TextEditorView } from 'components/TextEditorView';

interface ServiceChargeDescriptionGdViewProps {
  id: string;
  name: string;
  value?: ChargeModel;
}

export function ServiceChargeDescriptionGdView(props: ServiceChargeDescriptionGdViewProps): React.ReactElement {
  const { t } = useTranslation();

  return (
    <Block>
      <Box mt={2}>
        <Text smallScreen variant='bold'>
          {t('Ptv.Service.Form.Field.FeeExtraInfo.Label')}
        </Text>

        <TextEditorView id={`${props.id}.${cCharge.info}`} value={props.value?.info} />
      </Box>
    </Block>
  );
}
