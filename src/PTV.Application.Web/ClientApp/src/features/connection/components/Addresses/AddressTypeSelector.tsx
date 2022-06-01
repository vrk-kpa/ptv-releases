import React, { useMemo } from 'react';
import { Control, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RadioOption, RhfRadioButtonGroup } from 'fields';
import { AddressType } from 'types/enumTypes';
import { ConnectionFormModel, cAddress, cC } from 'types/forms/connectionFormTypes';
import { toFieldId } from 'features/connection/utils/fieldid';

export type AddressTypeSelectorProps = {
  addressIndex: number;
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function AddressTypeSelector(props: AddressTypeSelectorProps): React.ReactElement {
  const { t } = useTranslation();

  const name = `${cC.addresses}.${props.addressIndex}.${cAddress.type}`;

  const items = useMemo(() => {
    const rbs: RadioOption[] = [
      {
        value: 'Street',
        text: t('Ptv.ConnectionDetails.AddressType.Street.Label'),
      },
      {
        value: 'PostOfficeBox',
        text: t('Ptv.ConnectionDetails.AddressType.PostOfficeBox.Label'),
      },
      {
        value: 'Foreign',
        text: t('Ptv.ConnectionDetails.AddressType.Foreign.Label'),
      },
    ];
    return rbs;
  }, [t]);

  return (
    <RhfRadioButtonGroup<AddressType>
      control={props.control}
      name={name}
      id={toFieldId(name)}
      mode='edit'
      items={items}
      labelText={t('Ptv.ConnectionDetails.AddressType.Label')}
      rules={{ deps: [`${cC.addresses}.${props.addressIndex}`] }}
    />
  );
}
