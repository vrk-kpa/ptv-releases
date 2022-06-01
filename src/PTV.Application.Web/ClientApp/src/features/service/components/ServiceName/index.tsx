import React, { FunctionComponent, RefObject, useRef } from 'react';
import { Control, UseFormSetValue, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfTextInput } from 'fields';
import { Language, Mode } from 'types/enumTypes';
import { ServiceModel, cLv, cService } from 'types/forms/serviceFormTypes';
import { GuidedCorrectionItem } from 'types/qualityAgentResponses';
import { ComparisonView } from 'components/ComparisonView';
import { Tooltip } from 'components/Tooltip';
import { useFormMetaContext, useGetCompareFieldId, useGetCompareFieldName, useGetFieldId, useGetFieldName } from 'context/formMeta';
import { useGetSkippedIssues } from 'context/qualityAgent';
import { useGetQualityIssues } from 'context/qualityAgent/useGetQualityIssues';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import { setFocusEndOfText } from 'utils/ui';
import { QualityIssues, filterRelatedIssues } from 'features/qualityAgent';
import { handleRhfTextFieldChange } from 'features/qualityAgent/utility';

type ServiceNameProps = {
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
};

export const ServiceName: FunctionComponent<ServiceNameProps> = (props: ServiceNameProps) => {
  const meta = useFormMetaContext();
  const name = useGetFieldName();
  const compareName = useGetCompareFieldName();
  const id = useGetFieldId();
  const compareId = useGetCompareFieldId();
  const inputRef = useRef<HTMLInputElement>(null);

  return (
    <ComparisonView
      left={
        <NameItem
          forwardedRef={inputRef}
          name={name(cLv.name)}
          id={id(cLv.name)}
          mode={meta.mode}
          language={meta.selectedLanguageCode}
          control={props.control}
          setValue={props.setValue}
        />
      }
      right={
        <NameItem
          name={compareName(cLv.name, meta.compareLanguageCode)}
          id={compareId(cLv.name, meta.compareLanguageCode)}
          mode={meta.mode}
          language={meta.compareLanguageCode ?? meta.selectedLanguageCode}
          control={props.control}
          setValue={props.setValue}
        />
      }
    />
  );
};

type NameItemProps = {
  name: string;
  id: string;
  mode: Mode;
  language: Language;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
  forwardedRef?: RefObject<HTMLInputElement>;
};

const NameItem: FunctionComponent<NameItemProps> = (props: NameItemProps) => {
  const { t } = useTranslation();
  const { mode } = useFormMetaContext();
  const qualityIssues = useGetQualityIssues();
  const skippedIssues = useGetSkippedIssues();
  const relatedIssues = mode === 'edit' ? filterRelatedIssues(qualityIssues, `serviceNames.Name.${props.language}`, skippedIssues) : [];
  const displayWarning = relatedIssues.length > 0;
  const fieldValue = useWatch({ control: props.control, name: `${cService.languageVersions}.${props.language}.${cLv.name}` });
  const hintKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Field.Name.Hint',
    'Ptv.Service.Form.Field.Name.GdSelected.Hint'
  );
  const tooltipKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Field.Name.Tooltip',
    'Ptv.Service.Form.Field.Name.GdSelected.Tooltip'
  );

  function setFocusToInputElement(forwardedRef?: RefObject<HTMLInputElement>) {
    if (!forwardedRef) return;
    setFocusEndOfText(forwardedRef);
  }

  return (
    <>
      <RhfTextInput
        forwardedRef={props.forwardedRef}
        name={props.name}
        id={props.id}
        control={props.control}
        mode={props.mode}
        visualPlaceholder={t('Ptv.Service.Form.Field.Name.Placeholder')}
        labelText={t('Ptv.Service.Form.Field.Name.Label')}
        hintText={t(hintKey)}
        tooltipComponent={
          <Tooltip
            ariaInfoButtonLabelText={t('Ptv.Common.Tooltip.Button.Label', { label: t('Ptv.Service.Form.Field.Name.Label') })}
            ariaCloseButtonLabelText={t('Ptv.Common.Tooltip.CloseButton.Label', { label: t('Ptv.Service.Form.Field.Name.Label') })}
          >
            {t(tooltipKey)}
          </Tooltip>
        }
      />
      {displayWarning && (
        <QualityIssues
          issues={relatedIssues}
          handleTextChangeSuggestion={(value: string, item: GuidedCorrectionItem) =>
            handleRhfTextFieldChange(value, item, fieldValue, props.setValue, `${cService.languageVersions}.${props.language}.${cLv.name}`)
          }
          onLostFocusWithZeroGuidedErrors={() => setFocusToInputElement(props.forwardedRef)}
        />
      )}
    </>
  );
};
