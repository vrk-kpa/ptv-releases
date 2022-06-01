import React from 'react';
import { Control, UseFormSetValue, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { RhfReadOnlyField } from 'fields';
import { Block, Heading, Paragraph } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { useFormMetaContext } from 'context/formMeta';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import { getKeyForServiceChargeType } from 'utils/translations';
import { ServiceChargeDescription } from './ServiceChargeDescription';
import ServiceChargeType from './ServiceChargeType';

type ServiceChargeProps = {
  tabLanguage: Language;
  name: string;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
};

export default function ServiceCharge(props: ServiceChargeProps): React.ReactElement {
  const { t } = useTranslation();
  const { mode } = useFormMetaContext();
  const hintKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Fees.Title.Description',
    'Ptv.Service.Form.Fees.Title.GdSelected.Description'
  );

  const generalDescription = useWatch({
    control: props.control,
    name: `${cService.generalDescription}`,
  });

  return (
    <div>
      <Heading variant='h4'>{t('Ptv.Service.Form.Fees.Title.Text')}</Heading>
      {!generalDescription?.chargeType && (
        <Block>
          {mode === 'edit' && (
            <Box mt={2}>
              <Paragraph>{t(hintKey)}</Paragraph>
            </Box>
          )}
          <Box mt={2}>
            <ServiceChargeType name={cService.chargeType} mode={mode} tabLanguage={props.tabLanguage} control={props.control} />
          </Box>
        </Block>
      )}
      {generalDescription?.chargeType && mode === 'view' && (
        <Box mt={2}>
          <RhfReadOnlyField
            labelText={t('Ptv.Service.Form.Field.FeeSelect.Label')}
            id='tbd'
            value={t(getKeyForServiceChargeType(generalDescription?.chargeType))}
          />
        </Box>
      )}
      <ServiceChargeDescription name={props.name} gd={generalDescription} control={props.control} setValue={props.setValue} />
    </div>
  );
}
