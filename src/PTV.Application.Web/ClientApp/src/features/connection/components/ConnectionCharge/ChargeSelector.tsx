import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { NoValue, RhfRadioButtonGroup } from 'fields';
import { ChargeType } from 'types/enumTypes';
import { ConnectionFormModel, cC } from 'types/forms/connectionFormTypes';
import { toFieldId } from 'features/connection/utils/fieldid';

type Option = {
  value: ChargeType | typeof NoValue;
  text: string;
};

export type ChargeSelectorProps = {
  control: Control<ConnectionFormModel>;
};

export function ChargeSelector(props: ChargeSelectorProps): React.ReactElement {
  const { t } = useTranslation();

  const items: Option[] = [
    {
      value: 'Charged',
      text: t('Ptv.Service.ChargeType.Charged'),
    },
    {
      value: 'Free',
      text: t('Ptv.Service.ChargeType.Free'),
    },
    {
      value: 'Other',
      text: t('Ptv.Service.ChargeType.Other'),
    },
  ];

  return (
    <RhfRadioButtonGroup<ChargeType>
      control={props.control}
      name={cC.chargeType}
      id={toFieldId(cC.chargeType)}
      mode='edit'
      items={items}
      labelText={t('Ptv.ConnectionDetails.Charge.Selector.Label')}
    />
  );
}
