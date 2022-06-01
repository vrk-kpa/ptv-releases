import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfTextInput } from 'fields';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cHourLv } from 'types/forms/connectionFormTypes';
import { ComparisonView } from 'components/ComparisonView';
import { useFormMetaContext } from 'context/formMeta';
import { getExceptionalHourLvFieldName, toFieldId } from 'features/connection/utils/fieldid';

type TitleProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
};

export function Title(props: TitleProps): React.ReactElement {
  const { t } = useTranslation();
  const meta = useFormMetaContext();

  function getFieldName(lang: Language): string {
    return getExceptionalHourLvFieldName(props.hourIndex, lang, cHourLv.additionalInformation);
  }

  function renderRight(): React.ReactElement | null {
    if (!meta.compareLanguageCode) {
      return null;
    }

    const fieldName = getFieldName(meta.compareLanguageCode);

    return (
      <RhfTextInput
        control={props.control}
        name={fieldName}
        id={toFieldId(fieldName)}
        mode='edit'
        labelText={t('Ptv.ConnectionDetails.ExceptionalServiceHour.AdditionalInformation.Label')}
        visualPlaceholder={t('Ptv.ConnectionDetails.ExceptionalServiceHour.AdditionalInformation.Placeholder')}
        optionalText={t('Ptv.Common.Optional')}
      />
    );
  }

  const fieldName = getFieldName(meta.selectedLanguageCode);

  return (
    <ComparisonView
      left={
        <RhfTextInput
          control={props.control}
          name={fieldName}
          id={toFieldId(fieldName)}
          mode='edit'
          labelText={t('Ptv.ConnectionDetails.ExceptionalServiceHour.AdditionalInformation.Label')}
          visualPlaceholder={t('Ptv.ConnectionDetails.ExceptionalServiceHour.AdditionalInformation.Placeholder')}
          optionalText={t('Ptv.Common.Optional')}
        />
      }
      right={renderRight()}
    />
  );
}
