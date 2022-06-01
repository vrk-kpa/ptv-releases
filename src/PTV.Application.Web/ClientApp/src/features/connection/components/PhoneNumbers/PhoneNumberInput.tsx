import React from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfTextInput } from 'fields';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cC, cPhoneNumberLv } from 'types/forms/connectionFormTypes';
import { toFieldId } from 'features/connection/utils/fieldid';

type PhoneNumberInputProps = {
  phoneNumberIndex: number;
  control: Control<ConnectionFormModel>;
  onChanged: () => void;
  language: Language;
};

export function PhoneNumberInput(props: PhoneNumberInputProps): React.ReactElement {
  const { t } = useTranslation();

  const dialCodeType = useWatch({
    name: `${cC.phoneNumbers}.${props.language}.${props.phoneNumberIndex}.${cPhoneNumberLv.dialCodeType}`,
    control: props.control,
  });

  const hintKey =
    dialCodeType === 'Normal'
      ? 'Ptv.ConnectionDetails.PhoneNumber.Number.WithDialCode.Hint'
      : 'Ptv.ConnectionDetails.PhoneNumber.Number.WithoutDialCode.Hint';
  const name = `${cC.phoneNumbers}.${props.language}.${props.phoneNumberIndex}.${cPhoneNumberLv.number}`;
  const id = toFieldId(name);

  return (
    <RhfTextInput
      type='tel'
      control={props.control}
      name={name}
      id={id}
      mode='edit'
      labelText={t('Ptv.ConnectionDetails.PhoneNumber.Number.Label')}
      visualPlaceholder={t('Ptv.ConnectionDetails.PhoneNumber.Number.Placeholder')}
      hintText={t(hintKey)}
      onChanged={props.onChanged}
    />
  );
}
