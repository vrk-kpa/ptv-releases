import React, { FunctionComponent, useRef } from 'react';
import { Control, UseFormSetValue, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { ViewValueList } from 'fields';
import { Button, Paragraph, Text } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cLv, cService, cSp } from 'types/forms/serviceFormTypes';
import { GuidedCorrectionItem } from 'types/qualityAgentResponses';
import { useFormMetaContext } from 'context/formMeta';
import { useGetSkippedIssues } from 'context/qualityAgent';
import { useGetQualityIssues } from 'context/qualityAgent/useGetQualityIssues';
import { setFocusEndOfText } from 'utils/ui';
import { QualityIssues, filterRelatedIssues } from 'features/qualityAgent';
import { handleRhfTextFieldChange } from 'features/qualityAgent/utility';
import { AdditionalInfo } from './AdditionalInfo';

interface IProducerAdditionalInformation {
  name: cLv.purchaseProducers | cLv.otherProducers;
  description: string;
  control: Control<ServiceModel>;
  language: Language;
  setValue: UseFormSetValue<ServiceModel>;
}

export const ProducerAdditionalInformation: FunctionComponent<IProducerAdditionalInformation> = (props: IProducerAdditionalInformation) => {
  const { t } = useTranslation();
  const { mode } = useFormMetaContext();
  const inputRef = useRef<HTMLTextAreaElement>(null);
  const { fields, append, remove } = useFieldArray({
    control: props.control,
    name: `${cService.languageVersions}.${props.language}.${props.name}`,
  });

  function onRemove(index: number) {
    remove(index);
  }

  function onAddNew() {
    append({ name: '' });
  }

  const qualityIssues = useGetQualityIssues();
  const skippedIssues = useGetSkippedIssues();

  const additionalInfoRelatedIssues = filterRelatedIssues(qualityIssues, `organizations.additionalInformation.fi`, skippedIssues).filter(
    (issue) => issue.fieldId.includes(props.name)
  );

  if (mode === 'view') {
    const values = fields.map((x) => x.name);
    return (
      <ViewValueList
        id={props.name}
        values={values}
        labelText={t('Ptv.Service.Form.Field.ServiceProviders.ProducerAdditionalInformation.Title.Text')}
      />
    );
  }

  return (
    <Box mb={2}>
      <Box mb={1} mt={1}>
        <Text smallScreen={true} variant='bold'>
          {t('Ptv.Service.Form.Field.ServiceProviders.AdditionalInformation.OtherProducers.Title')}
        </Text>
      </Box>

      <Box mb={1} mt={1}>
        <Paragraph>{props.description}</Paragraph>
      </Box>

      {fields.map((item, index) => {
        const fieldName = `${cService.languageVersions}.${props.language}.${props.name}.${index}.${cSp.name}`;
        const fieldIssues = additionalInfoRelatedIssues.filter((issue) =>
          issue.fieldId.includes(`organization.${props.language}.${props.name}.${index}`)
        );
        const fieldValue = item.name;

        return (
          <div key={item.id}>
            <div>
              <AdditionalInfo
                forwardedRef={inputRef}
                control={props.control}
                mode={mode}
                name={fieldName}
                onRemove={() => onRemove(index)}
              />
            </div>
            {fieldIssues.length > 0 && (
              <QualityIssues
                issues={fieldIssues}
                onLostFocusWithZeroGuidedErrors={() => setFocusEndOfText(inputRef)}
                handleTextChangeSuggestion={(value: string, item: GuidedCorrectionItem) =>
                  handleRhfTextFieldChange(value, item, fieldValue, props.setValue, fieldName)
                }
              />
            )}
          </div>
        );
      })}

      <Button variant='secondary' type='button' onClick={onAddNew}>
        {t('Ptv.Service.Form.Field.ServiceProviders.AdditionalInformation.Button.AddProducer')}
      </Button>
    </Box>
  );
};
