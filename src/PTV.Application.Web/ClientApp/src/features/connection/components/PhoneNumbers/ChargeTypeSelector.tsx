import React, { useMemo } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RadioOption, RhfRadioButtonGroup } from 'fields';
import { ChargeType, Language } from 'types/enumTypes';
import { ConnectionFormModel, cC, cPhoneNumberLv } from 'types/forms/connectionFormTypes';
import { toFieldId } from 'features/connection/utils/fieldid';

export type ChargeTypeProps = {
  phoneNumberIndex: number;
  control: Control<ConnectionFormModel>;
  language: Language;
};

export function ChargeTypeSelector(props: ChargeTypeProps): React.ReactElement {
  const { t } = useTranslation();

  const name = `${cC.phoneNumbers}.${props.language}.${props.phoneNumberIndex}.${cPhoneNumberLv.chargeType}`;
  const id = toFieldId(name);

  const items = useMemo(() => {
    const rbs: RadioOption[] = [
      {
        value: 'Charged',
        text: t('Ptv.Service.ChargeType.Charged'),
        hintText: t('Ptv.ConnectionDetails.PhoneNumber.ChargeType.Charged.Hint'),
      },
      {
        value: 'Free',
        text: t('Ptv.Service.ChargeType.Free'),
        hintText: t('Ptv.ConnectionDetails.PhoneNumber.ChargeType.Free.Hint'),
      },
      {
        value: 'Other',
        text: t('Ptv.Service.ChargeType.Other'),
        hintText: t('Ptv.ConnectionDetails.PhoneNumber.ChargeType.Other.Hint'),
      },
    ];
    return rbs;
  }, [t]);

  return (
    <RhfRadioButtonGroup<ChargeType>
      control={props.control}
      name={name}
      id={id}
      mode='edit'
      items={items}
      labelText={t('Ptv.ConnectionDetails.Charge.Selector.Label')}
    />
  );
}
