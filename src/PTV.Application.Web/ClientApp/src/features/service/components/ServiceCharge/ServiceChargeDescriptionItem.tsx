import React, { FunctionComponent } from 'react';
import { Control, UseFormSetValue } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { Block, Text } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { useFormMetaContext } from 'context/formMeta';
import { getGdValueOrDefault } from 'utils/gd';
import { ServiceChargeDescriptionEdit } from './ServiceChargeDescriptionEdit';
import { ServiceChargeDescriptionView } from './ServiceChargeDescriptionView';

interface ServiceChargeDescriptionInterface {
  id: string;
  name: string;
  language: Language;
  gd: GeneralDescriptionModel | null | undefined;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
}

export const ServiceChargeDescriptionItem: FunctionComponent<ServiceChargeDescriptionInterface> = (props) => {
  const { mode } = useFormMetaContext();
  const { t } = useTranslation();

  if (mode === 'view') {
    if (props.gd) {
      const charge = getGdValueOrDefault(props.gd.languageVersions, props.language, (x) => x.charge, undefined);

      return (
        <Block>
          <Box mt={2}>
            <Text smallScreen variant='bold'>
              {t('Ptv.Service.Form.FromGD.Label')}
            </Text>
            <ServiceChargeDescriptionView
              id={props.id}
              name={props.name}
              fromGd
              value={charge}
              control={props.control}
              language={props.language}
            />
          </Box>
          <Box mt={2}>
            <Text smallScreen variant='bold'>
              {t('Ptv.Service.Form.FromService.Label')}
            </Text>
            <ServiceChargeDescriptionView id={props.id} name={props.name} control={props.control} language={props.language} />
          </Box>
        </Block>
      );
    }

    return <ServiceChargeDescriptionView id={props.id} name={props.name} control={props.control} language={props.language} />;
  }

  return <ServiceChargeDescriptionEdit {...props} id={props.id} name={props.name} control={props.control} setValue={props.setValue} />;
};
