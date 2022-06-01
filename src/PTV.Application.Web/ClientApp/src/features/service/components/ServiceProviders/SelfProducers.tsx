import React from 'react';
import { Control, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { CheckboxOption, ViewValueList } from 'fields';
import { Checkbox } from 'suomifi-ui-components';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { useFormMetaContext } from 'context/formMeta';
import { Fieldset } from 'fields/Fieldset/Fieldset';
import { Legend } from 'fields/Legend/Legend';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';

interface SelfProducersInterface {
  name: string;
  defaultItems: OrganizationModel[];
  control: Control<ServiceModel>;
}

export function SelfProducers(props: SelfProducersInterface): React.ReactElement | null {
  const { mode } = useFormMetaContext();
  const translate = useTranslateLocalizedText();
  const { t } = useTranslation();

  const { field } = useController({ control: props.control, name: `${cService.selfProducers}` });

  const toItem = (org: OrganizationModel): CheckboxOption => {
    const text = translate(org.texts, org.name);
    return {
      value: org.id,
      text: text,
    };
  };

  function handleOnClick(organizationId: string) {
    const org = props.defaultItems.find((x) => x.id === organizationId);
    if (!org) return;

    const existing = field.value.find((x) => x.id === organizationId);
    if (existing) {
      field.onChange(field.value.filter((x) => x.id !== organizationId));
    } else {
      field.onChange([...field.value, org]);
    }
  }

  const allValues = props.defaultItems.map((x) => toItem(x));

  if (mode === 'view') {
    const values = field.value.map((x) => translate(x.texts));
    return (
      <ViewValueList
        id={cService.selfProducers}
        labelText={t('Ptv.Service.Form.Field.ServiceProviders.SelfProducers.Organizations.Label')}
        values={values}
      />
    );
  }

  return (
    <Box mt='20px' mb='10px'>
      <Fieldset>
        <Legend>{t('Ptv.Service.Form.Field.ServiceProviders.SelfProducers.Organizations.Label')}</Legend>
        {allValues.map((item) => {
          const checked = !!field.value.find((x) => x.id === item.value);
          return (
            <Box mb={1} key={item.value}>
              <Checkbox value={item.value} onClick={() => handleOnClick(item.value)} checked={checked}>
                {item.text}
              </Checkbox>
            </Box>
          );
        })}
      </Fieldset>
    </Box>
  );
}
