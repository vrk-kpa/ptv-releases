import React, { FunctionComponent, useCallback, useMemo } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { CheckboxOption, RhfCheckboxGroup } from 'fields';
import { AreaType, Language, Mode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { getFieldId } from 'utils/fieldIds';

interface AreaTypeInterface {
  name: string;
  tabLanguage: Language;
  mode: Mode;
  control: Control<ServiceModel>;
}

export const ServiceAreaType: FunctionComponent<AreaTypeInterface> = (props: AreaTypeInterface): React.ReactElement => {
  const { t } = useTranslation();
  const id = getFieldId(props.name, props.tabLanguage, undefined);

  const items = useMemo(() => {
    const toValue = (input: AreaType): string => input;

    const rbs: CheckboxOption[] = [
      {
        value: toValue('Municipality'),
        text: t('Ptv.EnumTypes.AreaType.Municipality'),
      },
      {
        value: toValue('Province'),
        text: t('Ptv.EnumTypes.AreaType.Province'),
      },
      {
        value: toValue('BusinessRegions'),
        text: t('Ptv.EnumTypes.AreaType.BusinessRegions'),
      },
      {
        value: toValue('HospitalRegions'),
        text: t('Ptv.EnumTypes.AreaType.HospitalRegions'),
      },
    ];

    return rbs;
  }, [t]);

  const toFieldValue = useCallback((value: string): AreaType => {
    return value as AreaType;
  }, []);

  return (
    <RhfCheckboxGroup<AreaType>
      control={props.control}
      id={id}
      mode={props.mode}
      name={props.name}
      labelText={t('Ptv.Service.Form.Field.AreaType.Label')}
      items={items}
      hideInViewMode
      toFieldValue={toFieldValue}
    />
  );
};
