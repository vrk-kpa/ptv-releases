import React, { FunctionComponent, useRef } from 'react';
import { Control, UseFormSetValue, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { makeStyles } from '@mui/styles';
import { Editor } from 'draft-js';
import { RhfReadOnlyField, RhfTextEditor } from 'fields';
import { Block, Text } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ChargeModel, cCharge } from 'types/forms/chargeType';
import { cLv } from 'types/forms/connectionFormTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { GuidedCorrectionItem } from 'types/qualityAgentResponses';
import { Message } from 'components/Message';
import { Tooltip } from 'components/Tooltip';
import { useFormMetaContext } from 'context/formMeta';
import { useGetSkippedIssues } from 'context/qualityAgent';
import { useGetQualityIssues } from 'context/qualityAgent/useGetQualityIssues';
import { setFocusToEditor } from 'utils/draftjs';
import { withNamespace } from 'utils/fieldIds';
import { getGdValueOrDefault } from 'utils/gd';
import { getKeyForServiceChargeType } from 'utils/translations';
import { QualityIssues, filterRelatedIssues, getDescriptionSelector } from 'features/qualityAgent';
import { handleRhfRichTextFieldChange } from 'features/qualityAgent/utility';
import { ServiceChargeInfoMaxLength } from 'features/service/validation/serviceCharge';
import { ServiceChargeDescriptionView } from './ServiceChargeDescriptionView';

const useStyles = makeStyles(() => ({
  message: {
    marginTop: '20px',
  },
}));

interface ServiceChargeDescriptionEditInterface {
  id: string;
  name: string;
  language: Language;
  gd: GeneralDescriptionModel | null | undefined;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
}

export const ServiceChargeDescriptionEdit: FunctionComponent<ServiceChargeDescriptionEditInterface> = (props) => {
  const { t } = useTranslation();
  const classes = useStyles();
  const chargeInfoFieldName = withNamespace(props.name, cCharge.info);
  const { mode } = useFormMetaContext();
  const qualityIssues = useGetQualityIssues();
  const skippedIssues = useGetSkippedIssues();
  const editorRef = useRef<Editor>(null);
  const relatedIssues =
    mode === 'edit'
      ? filterRelatedIssues(
          qualityIssues,
          getDescriptionSelector('Services', 'ChargeTypeAdditionalInfo', props.language ?? 'fi'),
          skippedIssues
        )
      : [];
  const displayWarning = relatedIssues.length > 0;
  const fieldValue = useWatch({ control: props.control, name: `${cService.languageVersions}.${props.language}.${cLv.chargeInfo}` });

  let charge: ChargeModel | undefined = undefined;
  if (props.gd) {
    charge = getGdValueOrDefault(props.gd.languageVersions, props.language, (x) => x.charge, undefined);
  }

  return (
    <Block>
      {props.gd && (
        <Message type='generalDescription' className={classes.message}>
          <Text smallScreen variant='bold'>
            {t('Ptv.Service.Form.FromGD.Preview.Label')}
          </Text>
          <Box mt={2}>
            <RhfReadOnlyField
              value={t(getKeyForServiceChargeType(props.gd?.chargeType))}
              id={`languageVersions[${props.language}].${cService.chargeType}`}
              labelText={t('Ptv.Service.Form.Field.FeeSelect.Label')}
            />
          </Box>
          <ServiceChargeDescriptionView fromGd value={charge} {...props} />
        </Message>
      )}
      <Box mt={2}>
        <RhfTextEditor
          forwardedRef={editorRef}
          control={props.control}
          id={props.id}
          maxCharacters={ServiceChargeInfoMaxLength}
          labelText={t('Ptv.Service.Form.Field.FeeExtraInfo.Label')}
          placeHolder={t('Ptv.Service.Form.Field.FeeExtraInfo.Placeholder')}
          optionalText={t('Ptv.Common.Optional')}
          name={chargeInfoFieldName}
          value={fieldValue}
          qualityResults={relatedIssues}
          mode='edit'
          tooltipComponent={
            <Tooltip
              ariaInfoButtonLabelText={t('Ptv.Common.Tooltip.Button.Label', {
                label: t('Ptv.Service.Form.Field.FeeExtraInfo.Label'),
              })}
              ariaCloseButtonLabelText={t('Ptv.Common.Tooltip.CloseButton.Label', {
                label: t('Ptv.Service.Form.Field.FeeExtraInfo.Label'),
              })}
            >
              {t('Ptv.Service.Form.Field.FeeExtraInfo.Tooltip')}
            </Tooltip>
          }
        />
        {displayWarning && (
          <QualityIssues
            issues={relatedIssues}
            handleTextChangeSuggestion={(value: string, item: GuidedCorrectionItem) => {
              handleRhfRichTextFieldChange(
                value,
                item,
                fieldValue,
                props.setValue,
                `${cService.languageVersions}.${props.language}.${cLv.chargeInfo}`
              );
            }}
            onLostFocusWithZeroGuidedErrors={() => setFocusToEditor(editorRef)}
          />
        )}
      </Box>
    </Block>
  );
};
