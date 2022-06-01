import React, { useRef } from 'react';
import { Control, UseFormSetValue, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Editor } from 'draft-js';
import { RhfTextEditor } from 'fields';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cC } from 'types/forms/connectionFormTypes';
import { cLv } from 'types/forms/serviceFormTypes';
import { GuidedCorrectionItem } from 'types/qualityAgentResponses';
import { useFormMetaContext } from 'context/formMeta';
import { useGetSkippedIssues } from 'context/qualityAgent';
import { useGetQualityIssues } from 'context/qualityAgent/useGetQualityIssues';
import { setFocusToEditor } from 'utils/draftjs';
import { QualityIssues, filterRelatedIssues, getDescriptionSelector } from 'features/qualityAgent';
import { handleRhfRichTextFieldChange } from 'features/qualityAgent/utility';

type AdditionalInfoProps = {
  id: string;
  name: string;
  control: Control<ConnectionFormModel>;
  setValue: UseFormSetValue<ConnectionFormModel>;
  language: Language;
};

export function AdditionalInfo(props: AdditionalInfoProps): React.ReactElement {
  const { t } = useTranslation();
  const { selectedLanguageCode, mode } = useFormMetaContext();
  const qualityIssues = useGetQualityIssues();
  const skippedIssues = useGetSkippedIssues();
  const relatedIssues =
    mode === 'edit'
      ? filterRelatedIssues(
          qualityIssues,
          getDescriptionSelector('Relations', 'ChargeTypeAdditionalInfo', selectedLanguageCode),
          skippedIssues
        )
      : [];
  const displayWarning = relatedIssues.length > 0;
  const fieldValue = useWatch({ control: props.control, name: `${cC.languageVersions}.${props.language}.${cLv.charge}` });
  const editorRef = useRef<Editor>(null);

  return (
    <>
      <RhfTextEditor
        forwardedRef={editorRef}
        control={props.control}
        name={props.name}
        id={props.id}
        labelText={t('Ptv.ConnectionDetails.Charge.AdditionalInfo.Field.Label')}
        optionalText={t('Ptv.Common.Optional')}
        placeHolder={t('Ptv.ConnectionDetails.Charge.AdditionalInfo.Field.Placeholder')}
        maxCharacters={500}
        mode='edit'
      />
      {displayWarning && (
        <QualityIssues
          issues={relatedIssues}
          onLostFocusWithZeroGuidedErrors={() => setFocusToEditor(editorRef)}
          handleTextChangeSuggestion={(value: string, item: GuidedCorrectionItem) =>
            handleRhfRichTextFieldChange(
              value,
              item,
              fieldValue.info,
              props.setValue,
              `${cC.languageVersions}.${props.language}.${cLv.charge}.info`
            )
          }
        />
      )}
    </>
  );
}
