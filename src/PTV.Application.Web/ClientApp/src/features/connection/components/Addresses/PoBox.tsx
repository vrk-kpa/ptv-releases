import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfTextInput } from 'fields';
import { ConnectionFormModel, cAddLv, cAddress, cC } from 'types/forms/connectionFormTypes';
import { useFormMetaContext } from 'context/formMeta';
import { toFieldId } from 'features/connection/utils/fieldid';

type PoBoxProps = {
  addressIndex: number;
  control: Control<ConnectionFormModel>;
};

export function PoBox(props: PoBoxProps): React.ReactElement {
  const { t } = useTranslation();
  const meta = useFormMetaContext();

  const fieldName = `${cC.addresses}.${props.addressIndex}.${cAddress.languageVersions}.${meta.selectedLanguageCode}.${cAddLv.poBox}`;

  return (
    <RhfTextInput
      control={props.control}
      name={fieldName}
      id={toFieldId(fieldName)}
      mode='edit'
      labelText={t('Ptv.ConnectionDetails.Address.PoBox.Label')}
      visualPlaceholder={t('Ptv.ConnectionDetails.Address.PoBox.Placeholder')}
      hintText={t('Ptv.ConnectionDetails.Address.PoBox.Hint')}
    />
  );
}
