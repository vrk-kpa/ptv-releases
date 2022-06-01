import React, { FunctionComponent, useRef } from 'react';
import { Control, UseFormSetValue, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box, Grid } from '@mui/material';
import { styled } from '@mui/material/styles';
import { Button, Textarea } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cLv, cService, cServiceVoucherLink, cVoucher } from 'types/forms/serviceFormTypes';
import { GuidedCorrectionItem } from 'types/qualityAgentResponses';
import { VisualHeading } from 'components/VisualHeading';
import { useGetSkippedIssues } from 'context/qualityAgent';
import { useGetQualityIssues } from 'context/qualityAgent/useGetQualityIssues';
import { withNamespace } from 'utils/fieldIds';
import { toFieldStatus } from 'utils/rhf';
import { setFocusEndOfText } from 'utils/ui';
import { QualityIssues, filterRelatedIssues } from 'features/qualityAgent';
import { handleRhfTextFieldChange } from 'features/qualityAgent/utility';

const StyledBox = styled(Box)(({ theme }) => ({
  '& p.noTopMargin': {
    marginTop: 0,
  },
}));

interface IServiceVoucherLink {
  index: number;
  onRemove?: () => void;
  language: Language;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
}

export const ServiceVoucherLinkItem: FunctionComponent<IServiceVoucherLink> = (props: IServiceVoucherLink) => {
  const { t } = useTranslation();

  const namespace = `${cService.languageVersions}.${props.language}.${cLv.voucher}.${cVoucher.links}`;
  const nameFieldName = withNamespace(namespace, `${props.index}.${cServiceVoucherLink.name}`);
  const urlFieldName = withNamespace(namespace, `${props.index}.${cServiceVoucherLink.url}`);
  const infoFieldName = withNamespace(namespace, `${props.index}.${cServiceVoucherLink.additionalInformation}`);
  const nameRef = useRef<HTMLTextAreaElement>(null);
  const additionalInfoRef = useRef<HTMLTextAreaElement>(null);

  const { field: nameField, fieldState: nameFieldState } = useController({
    name: `${cService.languageVersions}.${props.language}.${cLv.voucher}.${cVoucher.links}.${props.index}.${cServiceVoucherLink.name}`,
    control: props.control,
  });

  const { field: urlField, fieldState: urlFieldState } = useController({
    name: `${cService.languageVersions}.${props.language}.${cLv.voucher}.${cVoucher.links}.${props.index}.${cServiceVoucherLink.url}`,
    control: props.control,
  });

  const { field: infoField, fieldState: infoFieldState } = useController({
    name: `${cService.languageVersions}.${props.language}.${cLv.voucher}.${cVoucher.links}.${props.index}.${cServiceVoucherLink.additionalInformation}`,
    control: props.control,
  });

  const qualityIssues = useGetQualityIssues();
  const skippedIssues = useGetSkippedIssues();

  const nameRelatedIssues = filterRelatedIssues(qualityIssues, `serviceVouchers.${props.language}.${props.index}`, skippedIssues).filter(
    (issue) => !issue.fieldId.includes('additionalInformation')
  );

  const infoRelatedIssues = filterRelatedIssues(qualityIssues, `serviceVouchers.${props.language}.${props.index}`, skippedIssues).filter(
    (issue) => issue.fieldId.includes('additionalInformation')
  );

  return (
    <StyledBox>
      <Grid container justifyContent='space-between' alignItems='center'>
        <VisualHeading className='noTopMargin' variant='h4'>
          {t('Ptv.Service.Form.Field.ServiceVoucherLink.Label')}
        </VisualHeading>
        <Button icon='remove' id={`${namespace}.${props.index}.remove`} variant='secondaryNoBorder' onClick={props.onRemove}>
          {t('Ptv.Service.Form.Field.ServiceVoucherLink.Remove.Label')}
        </Button>
      </Grid>
      <Box mt={2}>
        <Textarea
          forwardedRef={nameRef}
          id={nameFieldName}
          fullWidth={true}
          visualPlaceholder={t('Ptv.Service.Form.Field.ServiceVoucherLink.Name.Placeholder')}
          hintText={t('Ptv.Service.Form.Field.ServiceVoucherLink.Name.Hint')}
          labelText={t('Ptv.Service.Form.Field.ServiceVoucherLink.Name.Label')}
          {...nameField}
          {...toFieldStatus(nameFieldState)}
        />
        {nameRelatedIssues.length > 0 && (
          <QualityIssues
            issues={nameRelatedIssues}
            onLostFocusWithZeroGuidedErrors={() => setFocusEndOfText(nameRef)}
            handleTextChangeSuggestion={(value: string, item: GuidedCorrectionItem) =>
              handleRhfTextFieldChange(value, item, nameField.value, props.setValue, nameFieldName)
            }
          />
        )}
      </Box>
      <Box mt={2}>
        <Textarea
          id={urlFieldName}
          fullWidth={true}
          visualPlaceholder={t('Ptv.Service.Form.Field.ServiceVoucherLink.Url.Placeholder')}
          hintText={t('Ptv.Service.Form.Field.ServiceVoucherLink.Url.Hint')}
          labelText={t('Ptv.Service.Form.Field.ServiceVoucherLink.Url.Label')}
          {...urlField}
          {...toFieldStatus(urlFieldState)}
        />
      </Box>
      <Box mt={2}>
        <Textarea
          forwardedRef={additionalInfoRef}
          id={infoFieldName}
          fullWidth={true}
          visualPlaceholder={t('Ptv.Service.Form.Field.ServiceVoucherLink.AdditionalInformation.Placeholder')}
          labelText={t('Ptv.Service.Form.Field.ServiceVoucherLink.AdditionalInformation.Label')}
          optionalText={t('Ptv.Common.Optional')}
          {...infoField}
          {...toFieldStatus(infoFieldState)}
        />
        {infoRelatedIssues.length > 0 && (
          <QualityIssues
            issues={infoRelatedIssues}
            onLostFocusWithZeroGuidedErrors={() => setFocusEndOfText(additionalInfoRef)}
            handleTextChangeSuggestion={(value: string, item: GuidedCorrectionItem) =>
              handleRhfTextFieldChange(value, item, infoField.value, props.setValue, infoFieldName)
            }
          />
        )}
      </Box>
    </StyledBox>
  );
};
