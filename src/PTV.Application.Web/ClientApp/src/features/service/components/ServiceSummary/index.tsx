import React, { FunctionComponent, RefObject, useRef } from 'react';
import { Control, UseFormSetValue, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfTextarea } from 'fields';
import { Language, Mode } from 'types/enumTypes';
import { ServiceModel, cLv, cService } from 'types/forms/serviceFormTypes';
import { GuidedCorrectionItem } from 'types/qualityAgentResponses';
import { ComparisonView } from 'components/ComparisonView';
import { useFormMetaContext, useGetCompareFieldId, useGetCompareFieldName, useGetFieldId, useGetFieldName } from 'context/formMeta';
import { useGetSkippedIssues } from 'context/qualityAgent';
import { useGetQualityIssues } from 'context/qualityAgent/useGetQualityIssues';
import { setFocusEndOfText } from 'utils/ui';
import { QualityIssues, filterRelatedIssues, getDescriptionSelector } from 'features/qualityAgent';
import { handleRhfTextFieldChange } from 'features/qualityAgent/utility';

type ServiceSummaryProps = {
  control: Control<ServiceModel>;
  language: Language;
  setValue: UseFormSetValue<ServiceModel>;
};

export const ServiceSummary: FunctionComponent<ServiceSummaryProps> = (props: ServiceSummaryProps) => {
  const meta = useFormMetaContext();
  const qualityIssues = useGetQualityIssues();
  const name = useGetFieldName();
  const compareName = useGetCompareFieldName();
  const id = useGetFieldId();
  const compareId = useGetCompareFieldId();
  const skippedIssues = useGetSkippedIssues();
  const relatedIssues =
    meta.mode === 'edit'
      ? filterRelatedIssues(qualityIssues, getDescriptionSelector('Services', 'Summary', meta.selectedLanguageCode ?? 'fi'), skippedIssues)
      : [];
  const displayWarning = relatedIssues.length > 0;
  const fieldValue = useWatch({ control: props.control, name: `${cService.languageVersions}.${props.language}.${cLv.summary}` });
  const inputRef = useRef<HTMLTextAreaElement>(null);

  return (
    <ComparisonView
      left={
        <>
          <ServiceSummaryItem
            forwardedRef={inputRef}
            id={id(cLv.summary)}
            name={name(cLv.summary)}
            mode={meta.mode}
            control={props.control}
          />
          {displayWarning && (
            <QualityIssues
              issues={relatedIssues}
              handleTextChangeSuggestion={(value: string, item: GuidedCorrectionItem) =>
                handleRhfTextFieldChange(
                  value,
                  item,
                  fieldValue,
                  props.setValue,
                  `${cService.languageVersions}.${props.language}.${cLv.summary}`
                )
              }
              onLostFocusWithZeroGuidedErrors={() => setFocusEndOfText(inputRef)}
            />
          )}
        </>
      }
      right={
        <ServiceSummaryItem
          id={compareId(cLv.summary, meta.compareLanguageCode)}
          name={compareName(cLv.summary, meta.compareLanguageCode)}
          mode={meta.mode}
          control={props.control}
        />
      }
    />
  );
};

type ServiceSummaryItemProps = {
  name: string;
  id: string;
  mode: Mode;
  control: Control<ServiceModel>;
  forwardedRef?: RefObject<HTMLTextAreaElement>;
};

export const ServiceSummaryItem: FunctionComponent<ServiceSummaryItemProps> = (props: ServiceSummaryItemProps) => {
  const { t } = useTranslation();
  return (
    <RhfTextarea
      forwardedRef={props.forwardedRef}
      id={props.id}
      name={props.name}
      mode={props.mode}
      visualPlaceholder={t('Ptv.Service.Form.Field.Summary.Placeholder')}
      hintText={t('Ptv.Service.Form.Field.Summary.Hint')}
      labelText={t('Ptv.Service.Form.Field.Summary.Label')}
      control={props.control}
    />
  );
};
