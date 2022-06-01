import React, { useRef } from 'react';
import { Control, UseFormSetValue, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { Editor } from 'draft-js';
import { Heading, Paragraph } from 'suomifi-ui-components';
import { ConnectionFormModel, cC, cLv } from 'types/forms/connectionFormTypes';
import { GuidedCorrectionItem } from 'types/qualityAgentResponses';
import { ComparisonView } from 'components/ComparisonView';
import { useFormMetaContext, useGetFieldName } from 'context/formMeta';
import { useGetSkippedIssues } from 'context/qualityAgent';
import { useGetQualityIssues } from 'context/qualityAgent/useGetQualityIssues';
import { setFocusToEditor } from 'utils/draftjs';
import { ConnectionCharge } from 'features/connection/components/ConnectionCharge';
import { Description } from 'features/connection/components/Description';
import { FormBlock } from 'features/connection/components/FormLayout';
import { toFieldId } from 'features/connection/utils/fieldid';
import { QualityIssues, filterRelatedIssues, getDescriptionSelector } from 'features/qualityAgent';
import { handleRhfRichTextFieldChange } from 'features/qualityAgent/utility';

type BasicInformationProps = {
  control: Control<ConnectionFormModel>;
  setValue: UseFormSetValue<ConnectionFormModel>;
};

export function BasicInformation(props: BasicInformationProps): React.ReactElement {
  const { t } = useTranslation();
  const meta = useFormMetaContext();
  const qualityIssues = useGetQualityIssues();
  const name = useGetFieldName();
  const skippedIssues = useGetSkippedIssues();
  const relatedIssues =
    meta.mode === 'edit'
      ? filterRelatedIssues(qualityIssues, getDescriptionSelector('Relations', 'Description', meta.selectedLanguageCode), skippedIssues)
      : [];
  const displayWarning = relatedIssues.length > 0;
  const descriptionFieldValue = useWatch({
    control: props.control,
    name: `${cC.languageVersions}.${meta.selectedLanguageCode}.${cLv.description}`,
  });
  const editorRef = useRef<Editor>(null);

  return (
    <FormBlock marginTop='0px' marginBottom='20px'>
      <Grid container>
        <Grid item>
          <FormBlock marginTop='20px'>
            <Heading variant='h3'>{t('Ptv.ConnectionDetails.BasicInformation.Description.Label')}</Heading>
          </FormBlock>
        </Grid>
        <Grid item>
          <FormBlock marginTop='20px'>
            <Paragraph>{t('Ptv.ConnectionDetails.BasicInformation.Description.Hint')}</Paragraph>
          </FormBlock>
        </Grid>
      </Grid>

      <FormBlock marginTop='20px'>
        <ComparisonView
          left={
            <>
              <Description
                forwardedRef={editorRef}
                control={props.control}
                name={name(cLv.description)}
                id={toFieldId(name(cLv.description))}
                qualityResults={relatedIssues}
              />
              {displayWarning && (
                <QualityIssues
                  issues={relatedIssues}
                  onLostFocusWithZeroGuidedErrors={() => setFocusToEditor(editorRef)}
                  handleTextChangeSuggestion={(value: string, item: GuidedCorrectionItem) =>
                    handleRhfRichTextFieldChange(
                      value,
                      item,
                      descriptionFieldValue,
                      props.setValue,
                      `${cC.languageVersions}.${meta.selectedLanguageCode}.${cLv.description}`
                    )
                  }
                />
              )}
            </>
          }
          right={
            <Description
              control={props.control}
              name={name(cLv.description, meta.compareLanguageCode)}
              id={toFieldId(name(cLv.description, meta.compareLanguageCode))}
              qualityResults={relatedIssues}
            />
          }
        />
      </FormBlock>

      <ConnectionCharge control={props.control} setValue={props.setValue} />
    </FormBlock>
  );
}
