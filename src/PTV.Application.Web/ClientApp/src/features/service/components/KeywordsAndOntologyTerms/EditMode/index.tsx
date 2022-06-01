import React, { useState } from 'react';
import { useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { Block, Button, InlineAlert, Paragraph } from 'suomifi-ui-components';
import { OntologyTerm } from 'types/classificationItemsTypes';
import { cLv, cService } from 'types/forms/serviceFormTypes';
import { HeadingWithTooltip } from 'components/HeadingWithTooltip';
import { useFormMetaContext } from 'context/formMeta';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { toFieldStatus } from 'utils/rhf';
import { KeywordsAndOntologyTermsProps } from 'features/service/components/KeywordsAndOntologyTerms/types';
import {
  addOrRemoveKeyword,
  addOrRemoveOntologyTerm,
  hasOntologyTermLimitBeenReached,
  isOntologyTermDisabled,
  sortKeywords,
  sortOntologyTerms,
} from 'features/service/components/KeywordsAndOntologyTerms/utils';
import { Editor } from './Editor';
import { KeywordAndOntologyTermsList } from './KeywordAndOntologyTermsList';
import { Suggestions } from './Suggestions';

export function KeywordsAndOntologyTermsForm(props: KeywordsAndOntologyTermsProps): React.ReactElement {
  const uiLang = useGetUiLanguage();
  const { selectedLanguageCode } = useFormMetaContext();
  const { t } = useTranslation();
  const [isOpen, setIsOpen] = useState<boolean>(false);

  const hintKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Field.OntologyTerms.Display.Description',
    'Ptv.Service.Form.Field.OntologyTerms.Display.GdSelected.Description'
  );

  const tooltipKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Field.OntologyTerms.Display.Tooltip',
    'Ptv.Service.Form.Field.OntologyTerms.Display.GdSelected.Tooltip'
  );

  const { field: ontologyTermsField, fieldState: ontologyTermsFieldState } = useController({
    control: props.control,
    name: `${cService.ontologyTerms}`,
  });
  const ontologyTermsFieldStatus = toFieldStatus(ontologyTermsFieldState);

  const { field: keywordsField } = useController({
    control: props.control,
    name: `${cService.languageVersions}.${selectedLanguageCode}.${cLv.keywords}`,
  });

  function toggleOntologyTerm(ontologyTerm: OntologyTerm) {
    ontologyTermsField.onChange(sortOntologyTerms(uiLang, addOrRemoveOntologyTerm(ontologyTerm, ontologyTermsField.value)));
  }

  function isOntologyTermChecked(ontologyTerm: OntologyTerm): boolean {
    if (ontologyTermsField.value.some((x) => x.id === ontologyTerm.id)) return true;
    if (props.gdOntologyTerms.some((x) => x.id === ontologyTerm.id)) return true;
    return false;
  }

  function isTermDisabled(ontologyTerm: OntologyTerm): boolean {
    return isOntologyTermDisabled(ontologyTerm, props.gdOntologyTerms, ontologyTermsField.value);
  }

  function toggleKeyword(keyword: string) {
    keywordsField.onChange(sortKeywords(uiLang, addOrRemoveKeyword(keyword, keywordsField.value)));
  }

  function closeDiscardChanges(): void {
    setIsOpen(false);
  }

  function closeSaveChanges(chosenOntologyTerms: OntologyTerm[], chosenKeywords: string[]): void {
    ontologyTermsField.onChange(chosenOntologyTerms);
    keywordsField.onChange(chosenKeywords);
    setIsOpen(false);
  }

  const termLimitReached = hasOntologyTermLimitBeenReached(props.gdOntologyTerms, ontologyTermsField.value);

  return (
    <Block>
      <HeadingWithTooltip
        variant='h4'
        id={`${props.namespace}.${cService.ontologyTerms}`}
        tooltipContent={t(tooltipKey)}
        tooltipAriaLabel={t('Ptv.Service.Form.Field.OntologyTerms.Display.Label')}
        tabIndex={0}
      >
        {t('Ptv.Service.Form.Field.OntologyTerms.Display.Label')}
      </HeadingWithTooltip>
      <Box mt={2}>
        <Paragraph>{t(hintKey)}</Paragraph>
      </Box>
      <Box mt={2}>
        <Suggestions
          control={props.control}
          termLimitReached={termLimitReached}
          toggleOntologyTerm={toggleOntologyTerm}
          isOntologyTermChecked={isOntologyTermChecked}
          isOntologyTermDisabled={isTermDisabled}
        />
      </Box>

      {isOpen && (
        <Editor
          control={props.control}
          isOpen={isOpen}
          serviceOntologyTerms={ontologyTermsField.value}
          gdOntologyTerms={props.gdOntologyTerms}
          serviceKeywords={keywordsField.value}
          gdKeywords={props.gdKeywords}
          closeDiscardChanges={closeDiscardChanges}
          closeSaveChanges={closeSaveChanges}
        />
      )}
      <Box mt={2}>
        <Button id={`${props.namespace}.ontologyterms.open`} onClick={() => setIsOpen(true)} variant='secondary'>
          {t('Ptv.Service.Form.Field.OntologyTerms.Display.ToggleSelectMode.Label')}
        </Button>
      </Box>

      {ontologyTermsFieldStatus.status === 'error' && (
        <Box mt={2}>
          <InlineAlert status='error'>{ontologyTermsFieldStatus.statusText}</InlineAlert>
        </Box>
      )}

      <Box mt={2}>
        <KeywordAndOntologyTermsList
          ontologyTerms={ontologyTermsField.value}
          gdOntologyTerms={props.gdOntologyTerms}
          keywords={keywordsField.value}
          gdKeywords={props.gdKeywords}
          toggleOntologyTerm={toggleOntologyTerm}
          toggleKeyword={toggleKeyword}
        />
      </Box>
    </Block>
  );
}
