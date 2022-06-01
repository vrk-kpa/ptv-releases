import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfTextarea } from 'fields';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cAddLv, cAddress, cC } from 'types/forms/connectionFormTypes';
import { ComparisonView } from 'components/ComparisonView';
import { useFormMetaContext } from 'context/formMeta';
import { toFieldId } from 'features/connection/utils/fieldid';

type ForeignAdditionalInfoProps = {
  addressIndex: number;
  control: Control<ConnectionFormModel>;
};

export function ForeignAdditionalInfo(props: ForeignAdditionalInfoProps): React.ReactElement {
  const { t } = useTranslation();
  const meta = useFormMetaContext();

  function getFieldName(language: Language): string {
    return `${cC.addresses}.${props.addressIndex}.${cAddress.languageVersions}.${language}.${cAddLv.foreignAddress}`;
  }

  function renderRightSide(): React.ReactElement | null {
    if (!meta.compareLanguageCode) return null;

    const fieldName = getFieldName(meta.compareLanguageCode);

    return (
      <RhfTextarea
        control={props.control}
        name={fieldName}
        id={toFieldId(fieldName)}
        mode='edit'
        labelText={t('Ptv.ConnectionDetails.Address.ForeignText.Label')}
        visualPlaceholder={t('Ptv.ConnectionDetails.Address.ForeignText.Placeholder')}
      />
    );
  }

  const fieldName = getFieldName(meta.selectedLanguageCode);

  return (
    <ComparisonView
      left={
        <RhfTextarea
          control={props.control}
          name={fieldName}
          id={toFieldId(fieldName)}
          mode='edit'
          labelText={t('Ptv.ConnectionDetails.Address.ForeignText.Label')}
          visualPlaceholder={t('Ptv.ConnectionDetails.Address.ForeignText.Placeholder')}
        />
      }
      right={renderRightSide()}
    />
  );
}
