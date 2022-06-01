import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfTextInput } from 'fields';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cC, cFaxNumberLv } from 'types/forms/connectionFormTypes';
import { toFieldId } from 'features/connection/utils/fieldid';

type FaxNumberInputProps = {
  faxNumberIndex: number;
  control: Control<ConnectionFormModel>;
  language: Language;
};

export function FaxNumberInput(props: FaxNumberInputProps): React.ReactElement {
  const { t } = useTranslation();

  const fieldName = `${cC.faxNumbers}.${props.language}.${props.faxNumberIndex}.${cFaxNumberLv.number}`;
  const fieldId = toFieldId(fieldName);

  return (
    <RhfTextInput
      type='tel'
      control={props.control}
      name={fieldName}
      id={fieldId}
      mode='edit'
      labelText={t('Ptv.ConnectionDetails.FaxNumber.Label')}
      fullWidth={false}
      visualPlaceholder={t('Ptv.ConnectionDetails.FaxNumber.Placeholder')}
      hintText={t('Ptv.ConnectionDetails.FaxNumber.Hint')}
    />
  );
}
