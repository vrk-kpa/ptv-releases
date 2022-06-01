import React, { useRef } from 'react';
import { Control, UseFormSetValue, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Textarea } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cLv, cService, cVoucher } from 'types/forms/serviceFormTypes';
import { GuidedCorrectionItem } from 'types/qualityAgentResponses';
import { useGetSkippedIssues } from 'context/qualityAgent';
import { useGetQualityIssues } from 'context/qualityAgent/useGetQualityIssues';
import { toFieldStatus } from 'utils/rhf';
import { setFocusEndOfText } from 'utils/ui';
import { QualityIssues, filterRelatedIssues } from 'features/qualityAgent';
import { handleRhfTextFieldChange } from 'features/qualityAgent/utility';

type ServiceVoucherAdditionalInfoProps = {
  tabLanguage: Language;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
};

export function ServiceVoucherAdditionalInfo(props: ServiceVoucherAdditionalInfoProps): React.ReactElement | null {
  const { t } = useTranslation();
  const name = `${cService.languageVersions}.${props.tabLanguage}.${cLv.voucher}.${cVoucher.info}`;

  const { field, fieldState } = useController({
    name: `${cService.languageVersions}.${props.tabLanguage}.${cLv.voucher}.${cVoucher.info}`,
    control: props.control,
  });
  const { status, statusText } = toFieldStatus(fieldState);
  const inputRef = useRef<HTMLTextAreaElement>(null);

  const qualityIssues = useGetQualityIssues();
  const skippedIssues = useGetSkippedIssues();
  const relatedIssues = filterRelatedIssues(qualityIssues, `serviceVouchers.${props.tabLanguage}.additionalInformation`, skippedIssues);

  const displayWarning = relatedIssues.length > 0;

  return (
    <>
      <Textarea
        forwardedRef={inputRef}
        id={name}
        fullWidth={true}
        visualPlaceholder={t('Ptv.Service.Form.Field.ServiceVouchers.AdditionalInformation.Placeholder')}
        hintText={t('Ptv.Service.Form.Field.ServiceVouchers.AdditionalInformation.Hint')}
        labelText={t('Ptv.Service.Form.Field.ServiceVouchers.AdditionalInformation.Label')}
        optionalText={t('Ptv.Common.Optional')}
        status={status}
        statusText={statusText}
        {...field}
      />
      {displayWarning && (
        <QualityIssues
          issues={relatedIssues}
          onLostFocusWithZeroGuidedErrors={() => setFocusEndOfText(inputRef)}
          handleTextChangeSuggestion={(value: string, item: GuidedCorrectionItem) =>
            handleRhfTextFieldChange(
              value,
              item,
              field.value,
              props.setValue,
              `${cService.languageVersions}.${props.tabLanguage}.${cLv.voucher}.${cVoucher.info}`
            )
          }
        />
      )}
    </>
  );
}
