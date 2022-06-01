import React, { FunctionComponent } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { RhfTextEditorView } from 'fields';
import { Block } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ChargeModel, cCharge } from 'types/forms/chargeType';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { ServiceChargeDescriptionGdView } from './ServiceChangeDescriptionGdView';

interface ServiceChargeDescriptionViewInterface {
  id: string;
  name: string;
  value?: ChargeModel;
  fromGd?: boolean;
  control: Control<ServiceModel>;
  language: Language;
}

export const ServiceChargeDescriptionView: FunctionComponent<ServiceChargeDescriptionViewInterface> = (props) => {
  const { t } = useTranslation();

  const infoFieldName = `${props.name}.${cCharge.info}`;

  if (props.fromGd) {
    return <ServiceChargeDescriptionGdView id={props.id} name={props.name} value={props.value} />;
  }

  return (
    <Block>
      <Box mt={2}>
        <RhfTextEditorView
          control={props.control}
          id={infoFieldName}
          labelText={t('Ptv.Service.Form.Field.FeeExtraInfo.Label')}
          tooltipText={t('Ptv.Service.Form.Field.FeeExtraInfo.Tooltip')}
          name={infoFieldName}
        />
      </Box>
    </Block>
  );
};
