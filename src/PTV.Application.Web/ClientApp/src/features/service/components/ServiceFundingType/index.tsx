import React, { useMemo } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RadioOption, RhfRadioButtonGroup } from 'fields';
import { FundingType, Language, Mode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { getFieldId } from 'utils/fieldIds';

type ServiceFundingTypeProps = {
  name: string;
  tabLanguage: Language;
  mode: Mode;
  control: Control<ServiceModel>;
};

export default function ServiceFundingType(props: ServiceFundingTypeProps): React.ReactElement | null {
  const { t } = useTranslation();
  const id = getFieldId(props.name, props.tabLanguage, undefined);

  const items = useMemo(() => {
    const toValue = (value: FundingType): string => value;
    const rbs: RadioOption[] = [
      {
        value: toValue('PubliclyFunded'),
        text: t('Ptv.Service.FundingType.PubliclyFunded'),
      },
      {
        value: toValue('MarketFunded'),
        text: t('Ptv.Service.FundingType.MarketFunded'),
      },
    ];

    return rbs;
  }, [t]);

  return (
    <RhfRadioButtonGroup
      control={props.control}
      name={props.name}
      id={id}
      mode={props.mode}
      items={items}
      labelText={t('Ptv.Service.Form.Field.FundingType.Label')}
    />
  );
}
