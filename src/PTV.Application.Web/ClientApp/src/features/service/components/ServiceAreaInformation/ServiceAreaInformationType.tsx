import React, { useMemo } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RadioOption, RhfRadioButtonGroup } from 'fields';
import { AreaInformationType, Language, Mode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { getFieldId } from 'utils/fieldIds';

type AreaInformationTypeProps = {
  name: string;
  mode: Mode;
  tabLanguage: Language;
  control: Control<ServiceModel>;
};

export function ServiceAreaInformationType(props: AreaInformationTypeProps): React.ReactElement {
  const { t } = useTranslation();
  const id = getFieldId(props.name, props.tabLanguage, undefined);

  const items = useMemo(() => {
    function toValue(input: AreaInformationType): string {
      return input;
    }

    const rbs: RadioOption[] = [
      {
        value: toValue('WholeCountry'),
        text: t('Ptv.Service.Form.Field.AreaInformationType.WholeCountry'),
      },
      {
        value: toValue('WholeCountryExceptAlandIslands'),
        text: t('Ptv.Service.Form.Field.AreaInformationType.WholeCountryExceptAlandIslands'),
      },
      {
        value: toValue('AreaType'),
        text: t('Ptv.Service.Form.Field.AreaInformationType.AreaType'),
      },
    ];

    return rbs;
  }, [t]);

  return (
    <RhfRadioButtonGroup<AreaInformationType>
      control={props.control}
      id={id}
      mode={props.mode}
      name={props.name}
      items={items}
      labelText={t('Ptv.Service.Form.Field.AreaInformation.Title')}
      hintText={t('Ptv.Service.Form.Field.AreaInformation.Description')}
    />
  );
}
