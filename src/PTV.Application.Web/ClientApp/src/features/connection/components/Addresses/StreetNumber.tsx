import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfTextInput } from 'fields';
import { ConnectionFormModel, cAddress, cC } from 'types/forms/connectionFormTypes';
import { toFieldId } from 'features/connection/utils/fieldid';

type StreetNumberProps = {
  addressIndex: number;
  control: Control<ConnectionFormModel>;
  isOptional: boolean;
};

export function StreetNumber(props: StreetNumberProps): React.ReactElement {
  const { t } = useTranslation();

  const fieldName = `${cC.addresses}.${props.addressIndex}.${cAddress.streetNumber}`;
  const optionalText = props.isOptional ? t('Ptv.Common.Optional') : undefined;

  return (
    <RhfTextInput
      control={props.control}
      name={fieldName}
      id={toFieldId(fieldName)}
      mode='edit'
      labelText={t('Ptv.ConnectionDetails.Address.StreetNumber.Label')}
      optionalText={optionalText}
      visualPlaceholder={t('Ptv.ConnectionDetails.Address.StreetNumber.Placeholder')}
    />
  );
}
