import React, { useState } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid, styled } from '@mui/material';
import { Checkbox, Expander, ExpanderContent, ExpanderTitleButton, InlineAlert, Paragraph } from 'suomifi-ui-components';
import { OntologyTermsResponse } from 'types/annotationToolTypes';
import { OntologyTerm } from 'types/classificationItemsTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import LoadingIndicator from 'components/LoadingIndicator';
import { Fieldset } from 'fields/Fieldset/Fieldset';
import { useAnnotationToolDependency } from 'hooks/keywords/useAnnotationToolDependency';
import { useAnnotationToolQuery } from 'hooks/queries/useAnnotationToolQuery';
import { useDebounceObject } from 'hooks/useDebounceObject';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getDiffByPropValue } from 'utils/dataHelpers';
import { getTextByLangPriority } from 'utils/translations';
import { toSortedOntologyTerms } from 'features/service/components/KeywordsAndOntologyTerms/utils';

const ItemContainer = styled('div')({
  marginTop: '20px',
});

const CheckboxContainer = styled('div')({
  marginBottom: '10px',
});

type AnnotationDiffInfo = {
  annotationData: OntologyTermsResponse[];
  diffCount: number;
};

type SuggestionsProps = {
  control: Control<ServiceModel>;
  termLimitReached: boolean;
  toggleOntologyTerm: (term: OntologyTerm) => void;
  isOntologyTermDisabled: (term: OntologyTerm) => boolean;
  isOntologyTermChecked: (term: OntologyTerm) => boolean;
};

const DebounceTime = 5 * 1000;

export function Suggestions(props: SuggestionsProps): React.ReactElement {
  const { t } = useTranslation();
  const uiLang = useGetUiLanguage();

  const [annotationDiffInfo, setAnnotationDiffInfo] = useState<AnnotationDiffInfo | null>(null);
  const annotationDependencyData = useAnnotationToolDependency({
    control: props.control,
    language: uiLang,
  });

  const annotationData = useDebounceObject(annotationDependencyData, DebounceTime);
  const query = useAnnotationToolQuery(annotationData.requestData, { enabled: annotationData.isEnabled, onSuccess: onSuccess });

  function onSuccess(response: OntologyTermsResponse[]) {
    setAnnotationDiffInfo((prevState) => {
      if (prevState) {
        const diffData = getDiffByPropValue(response, prevState.annotationData, 'id');
        return { annotationData: response, diffCount: diffData.length };
      }
      return { annotationData: response, diffCount: 0 };
    });
  }

  const result = query.data || [];
  const ontologyTerms = toSortedOntologyTerms(uiLang, result);
  const diffCount = annotationDiffInfo?.diffCount || 0;
  const showSuggestionsNote = annotationData.isEnabled && !query.isLoading && !query.isError && diffCount > 1;
  const showInsufficientDataNote = !query.isLoading && !query.isError && ontologyTerms.length === 0;

  return (
    <Expander defaultOpen={false} id='automaticKeywordsExpander'>
      <ExpanderTitleButton asHeading='h4'>
        {t('Ptv.Service.Form.Field.OntologyTerms.AutomaticSearch.Label', { count: result.length })}
      </ExpanderTitleButton>
      <ExpanderContent>
        <Paragraph>{t('Ptv.Service.Form.Field.OntologyTerms.AutomaticSearch.Description')}</Paragraph>

        {query.isLoading && (
          <ItemContainer>
            <Grid container justifyContent='center' alignItems='center' direction='column'>
              <Grid item>
                <LoadingIndicator />
              </Grid>
            </Grid>
          </ItemContainer>
        )}

        {showSuggestionsNote && (
          <ItemContainer>
            <InlineAlert status='neutral'>
              {t('Ptv.Service.Form.Field.OntologyTerms.AutomaticSearch.Message.NewItems', { count: diffCount })}
            </InlineAlert>
          </ItemContainer>
        )}

        {showInsufficientDataNote && (
          <ItemContainer>
            <InlineAlert status='neutral'>{t('Ptv.Service.Form.Field.OntologyTerms.AutomaticSearch.Message.InsufficientInfo')}</InlineAlert>
          </ItemContainer>
        )}

        {query.isError && (
          <ItemContainer>
            <InlineAlert status='warning'>{t('Ptv.Service.Form.Field.OntologyTerms.AutomaticSearch.SearchError')}</InlineAlert>
          </ItemContainer>
        )}

        {ontologyTerms.length > 0 && (
          <ItemContainer>
            <Fieldset>
              {ontologyTerms.map((ontologyTerm) => {
                return (
                  <CheckboxContainer key={ontologyTerm.id}>
                    <Checkbox
                      onClick={() => props.toggleOntologyTerm(ontologyTerm)}
                      checked={props.isOntologyTermChecked(ontologyTerm)}
                      disabled={props.isOntologyTermDisabled(ontologyTerm)}
                    >
                      {getTextByLangPriority(uiLang, ontologyTerm.names)}
                    </Checkbox>
                  </CheckboxContainer>
                );
              })}
            </Fieldset>
          </ItemContainer>
        )}

        {props.termLimitReached && (
          <ItemContainer>
            <InlineAlert status='neutral'>{t(`Ptv.Service.Form.Field.OntologyTerms.Message.LimitReached`)}</InlineAlert>
          </ItemContainer>
        )}
      </ExpanderContent>
    </Expander>
  );
}
