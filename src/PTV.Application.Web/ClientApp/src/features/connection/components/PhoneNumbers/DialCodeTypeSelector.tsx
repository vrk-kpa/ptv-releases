import React, { useMemo } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RadioOption, RhfRadioButtonGroup } from 'fields';
import { Language, PhoneNumberDialCodeType } from 'types/enumTypes';
import { ConnectionFormModel, cC, cPhoneNumberLv } from 'types/forms/connectionFormTypes';
import { toFieldId } from 'features/connection/utils/fieldid';

export type DialCodeTypeSelectorProps = {
  phoneNumberIndex: number;
  control: Control<ConnectionFormModel>;
  onChanged: () => void;
  language: Language;
};

export function DialCodeTypeSelector(props: DialCodeTypeSelectorProps): React.ReactElement {
  const { t } = useTranslation();

  const name = `${cC.phoneNumbers}.${props.language}.${props.phoneNumberIndex}.${cPhoneNumberLv.dialCodeType}`;
  const id = toFieldId(name);

  const items = useMemo(() => {
    const options: RadioOption[] = [
      {
        value: 'Normal',
        text: t('Ptv.PhoneNumber.DialCodeType.Normal.Label'),
        hintText: t('Ptv.PhoneNumber.DialCodeType.Normal.Hint'),
      },
      {
        value: 'NationalWithoutDialCode',
        text: t('Ptv.PhoneNumber.DialCodeType.NationalWithoutDialCode.Label'),
        hintText: t('Ptv.PhoneNumber.DialCodeType.NationalWithoutDialCode.Hint'),
      },
    ];
    return options;
  }, [t]);

  return (
    <RhfRadioButtonGroup<PhoneNumberDialCodeType>
      control={props.control}
      name={name}
      id={id}
      mode='edit'
      items={items}
      labelText={t('Ptv.ConnectionDetails.PhoneNumber.DialCodeType.Label')}
      onChanged={props.onChanged}
    />
  );
}
