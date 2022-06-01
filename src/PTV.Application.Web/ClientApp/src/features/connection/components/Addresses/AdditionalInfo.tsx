import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfTextarea } from 'fields';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cAddLv, cAddress, cC } from 'types/forms/connectionFormTypes';
import { ComparisonView } from 'components/ComparisonView';
import { useFormMetaContext } from 'context/formMeta';
import { toFieldId } from 'features/connection/utils/fieldid';

type AdditionalInfoProps = {
  addressIndex: number;
  control: Control<ConnectionFormModel>;
};

export function AdditionalInfo(props: AdditionalInfoProps): React.ReactElement {
  const { t } = useTranslation();
  const meta = useFormMetaContext();

  function getFieldName(language: Language): string {
    return `${cC.addresses}.${props.addressIndex}.${cAddress.languageVersions}.${language}.${cAddLv.additionalInformation}`;
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
        labelText={t('Ptv.ConnectionDetails.Address.AdditionalInformation.Label')}
        optionalText={t('Ptv.Common.Optional')}
        hintText={t('Ptv.ConnectionDetails.Address.AdditionalInformation.Hint')}
        visualPlaceholder={t('Ptv.ConnectionDetails.Address.AdditionalInformation.Placeholder')}
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
          labelText={t('Ptv.ConnectionDetails.Address.AdditionalInformation.Label')}
          optionalText={t('Ptv.Common.Optional')}
          hintText={t('Ptv.ConnectionDetails.Address.AdditionalInformation.Hint')}
          visualPlaceholder={t('Ptv.ConnectionDetails.Address.AdditionalInformation.Placeholder')}
        />
      }
      right={renderRightSide()}
    />
  );
}
