import React, { useCallback, useMemo } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RadioOption, RhfRadioButtonGroup } from 'fields';
import { ChargeType, Language, Mode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { getFieldId } from 'utils/fieldIds';

const NoValue = 'no-value';

type ServiceChargeTypeProps = {
  name: string;
  mode: Mode;
  tabLanguage: Language;
  control: Control<ServiceModel>;
};

export default function ServiceChargeType(props: ServiceChargeTypeProps): React.ReactElement {
  const { t } = useTranslation();
  const id = getFieldId(props.name, props.tabLanguage, undefined);

  const items = useMemo(() => {
    const toValue = (value: ChargeType): string => value;
    const rbs: RadioOption[] = [
      {
        value: toValue('Charged'),
        text: t('Ptv.Service.ChargeType.Charged'),
      },
      {
        value: toValue('Free'),
        text: t('Ptv.Service.ChargeType.Free'),
      },
    ];
    return rbs;
  }, [t]);

  const toFieldValue = useCallback((str: string): ChargeType | null => {
    return str === NoValue ? null : (str as ChargeType);
  }, []);

  const toRadioButtonValue = useCallback((value: ChargeType | null): string => {
    return value ? value : NoValue;
  }, []);

  return (
    <RhfRadioButtonGroup<ChargeType | null>
      control={props.control}
      id={id}
      name={props.name}
      mode={props.mode}
      items={items}
      toFieldValue={toFieldValue}
      toRadioButtonValue={toRadioButtonValue}
      labelText={t('Ptv.Service.Form.Field.FeeSelect.Label')}
    />
  );
}
