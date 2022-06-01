import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid, styled } from '@mui/material';
import { Checkbox } from 'suomifi-ui-components';
import { OntologyTerm, OntologyTermTreeType } from 'types/classificationItemsTypes';
import { Fieldset } from 'fields/Fieldset/Fieldset';
import { Legend } from 'fields/Legend/Legend';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getTextByLangPriority } from 'utils/translations';
import { toOntologyTerm, toSortedOntologyTerms } from 'features/service/components/KeywordsAndOntologyTerms/utils';

type SearchResultsProps = {
  term: OntologyTermTreeType | null;
  toggleOntologyTerm: (ontologyTerm: OntologyTerm) => void;
  isOntologyTermChecked: (ontologyTerm: OntologyTerm) => boolean;
  isOntologyTermDisabled: (ontologyTerm: OntologyTerm) => boolean;
};

const ItemContainer = styled('div')({
  marginTop: '10px',
});

const CheckboxContainer = styled('div')({
  marginBottom: '10px',
});

export function SearchResults(props: SearchResultsProps): React.ReactElement | null {
  const { t } = useTranslation();
  const uiLang = useGetUiLanguage();

  if (!props.term) return null;

  const ontologyTerm = toOntologyTerm(props.term);
  const parents = toSortedOntologyTerms(uiLang, props.term.parents);
  const children = toSortedOntologyTerms(uiLang, props.term.children);
  const termTitle = getTextByLangPriority(uiLang, ontologyTerm.names);

  return (
    <Grid container direction='column'>
      <Grid item>
        <Fieldset>
          <Legend>{t('Ptv.Service.Form.Field.OntologyTerms.Select.Concept.Label')}</Legend>
          <CheckboxContainer>
            <Checkbox
              onClick={() => props.toggleOntologyTerm(ontologyTerm)}
              checked={props.isOntologyTermChecked(ontologyTerm)}
              disabled={props.isOntologyTermDisabled(ontologyTerm)}
            >
              {getTextByLangPriority(uiLang, ontologyTerm.names)}
            </Checkbox>
          </CheckboxContainer>
        </Fieldset>
      </Grid>
      {parents.length > 0 && (
        <Grid item>
          <ItemContainer>
            <Fieldset>
              <Legend>{t('Ptv.Service.Form.Field.OntologyTerms.Select.TopConcept.Label', { term: termTitle })}</Legend>
              {parents.map((item) => {
                return (
                  <CheckboxContainer key={item.id}>
                    <Checkbox
                      onClick={() => props.toggleOntologyTerm(item)}
                      checked={props.isOntologyTermChecked(item)}
                      disabled={props.isOntologyTermDisabled(item)}
                    >
                      {getTextByLangPriority(uiLang, item.names)}
                    </Checkbox>
                  </CheckboxContainer>
                );
              })}
            </Fieldset>
          </ItemContainer>
        </Grid>
      )}
      {children.length > 0 && (
        <Grid item>
          <ItemContainer>
            <Fieldset>
              <Legend>{t('Ptv.Service.Form.Field.OntologyTerms.Select.SubConcept.Label', { term: termTitle })}</Legend>
              {children.map((item) => {
                return (
                  <CheckboxContainer key={item.id}>
                    <Checkbox
                      onClick={() => props.toggleOntologyTerm(item)}
                      checked={props.isOntologyTermChecked(item)}
                      disabled={props.isOntologyTermDisabled(item)}
                    >
                      {getTextByLangPriority(uiLang, item.names)}
                    </Checkbox>
                  </CheckboxContainer>
                );
              })}
            </Fieldset>
          </ItemContainer>
        </Grid>
      )}
    </Grid>
  );
}
