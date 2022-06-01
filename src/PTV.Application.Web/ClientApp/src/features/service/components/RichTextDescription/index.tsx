import React, { FunctionComponent, ReactElement, useRef } from 'react';
import { Control, UseFormSetValue, useWatch } from 'react-hook-form';
import Box from '@mui/material/Box';
import { Editor } from 'draft-js';
import { Block } from 'suomifi-ui-components';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { GuidedCorrectionItem } from 'types/qualityAgentResponses';
import { ComparisonView } from 'components/ComparisonView';
import {
  useFormMetaContext,
  useGetCompareFieldId,
  useGetCompareFieldName,
  useGetFieldId,
  useGetFieldName,
  useGetSelectedLanguage,
} from 'context/formMeta';
import { useGetSkippedIssues } from 'context/qualityAgent';
import { useGetQualityIssues } from 'context/qualityAgent/useGetQualityIssues';
import { setFocusToEditor } from 'utils/draftjs';
import { QualityIssues, filterRelatedIssues } from 'features/qualityAgent';
import { handleRhfRichTextFieldChange } from 'features/qualityAgent/utility';
import { Description } from './Description';

interface IRichTextDescription {
  gd: GeneralDescriptionModel | null | undefined;
  name: 'description' | 'summary' | 'userInstructions' | 'deadline' | 'processingTime' | 'periodOfValidity' | 'conditions';
  labelText: string;
  hintText?: string;
  tooltipComponent?: ReactElement;
  placeHolder?: string;
  optionalText?: string;
  maxCharacters: number;
  compare?: boolean;
  qualitySelector: string;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
}

export const RichTextDescription: FunctionComponent<IRichTextDescription> = ({ name, qualitySelector, control, setValue, ...rest }) => {
  const meta = useFormMetaContext();
  const qualityIssues = useGetQualityIssues();
  const getName = useGetFieldName();
  const getCompareName = useGetCompareFieldName();
  const getId = useGetFieldId();
  const getCompareId = useGetCompareFieldId();
  const language = useGetSelectedLanguage();
  const skippedIssues = useGetSkippedIssues();
  const relatedIssues = meta.mode === 'edit' ? filterRelatedIssues(qualityIssues, qualitySelector, skippedIssues) : [];
  const displayWarning = relatedIssues.length > 0;
  const fieldValue = useWatch({ control: control, name: `${cService.languageVersions}.${meta.selectedLanguageCode}.${name}` });
  const editorRef = useRef<Editor>(null);

  function renderRight(): React.ReactElement | null {
    if (!meta.compareLanguageCode) {
      return null;
    }

    return (
      <Block>
        <Box mt={2}>
          <Description
            name={getCompareName(name, meta.compareLanguageCode)}
            gdFieldName={name}
            id={getCompareId(name, meta.compareLanguageCode)}
            language={meta.compareLanguageCode}
            control={control}
            mode={meta.mode}
            qualityResults={relatedIssues}
            {...rest}
          />
        </Box>
      </Block>
    );
  }

  return (
    <ComparisonView
      left={
        <>
          <Block>
            <Box mt={2}>
              <Description
                name={getName(name)}
                gdFieldName={name}
                id={getId(name)}
                language={language}
                mode={meta.mode}
                control={control}
                qualityResults={relatedIssues}
                forwardedRef={editorRef}
                {...rest}
              />
              {displayWarning && (
                <QualityIssues
                  issues={relatedIssues}
                  onLostFocusWithZeroGuidedErrors={() => setFocusToEditor(editorRef)}
                  handleTextChangeSuggestion={(value: string, item: GuidedCorrectionItem) =>
                    handleRhfRichTextFieldChange(
                      value,
                      item,
                      fieldValue,
                      setValue,
                      `${cService.languageVersions}.${meta.selectedLanguageCode}.${name}`
                    )
                  }
                />
              )}
            </Box>
          </Block>
        </>
      }
      right={renderRight()}
    />
  );
};
