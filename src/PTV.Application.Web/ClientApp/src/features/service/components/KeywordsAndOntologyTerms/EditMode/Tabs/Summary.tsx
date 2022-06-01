import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid, styled } from '@mui/material';
import { Heading } from 'suomifi-ui-components';
import { OntologyTerm } from 'types/classificationItemsTypes';
import { KeywordChipList } from 'features/service/components/KeywordsAndOntologyTerms/components/KeywordChipList';
import { OntologyTermChipList } from 'features/service/components/KeywordsAndOntologyTerms/components/OntologyTermChipList';

const Container = styled('div')({
  marginTop: '20px',
});

type SummaryProps = {
  keywords: string[];
  gdKeywords: string[];
  ontologyTerms: OntologyTerm[];
  gdOntologyTerms: OntologyTerm[];
  toggleOntologyTerm: (ontologyTerm: OntologyTerm) => void;
  toggleKeyword: (keyword: string) => void;
};

export function Summary(props: SummaryProps): React.ReactElement {
  const { t } = useTranslation();

  return (
    <Grid container direction='column'>
      {props.ontologyTerms.length > 0 && (
        <Grid item>
          <Heading variant='h5' as='h2'>
            {t('Ptv.Service.Form.Field.KeywordsAndOntologyTerms.Summary.OntologyTerms.Title', { count: props.ontologyTerms.length })}
          </Heading>
          <OntologyTermChipList ontologyTerms={props.ontologyTerms} toggleOntologyTerm={props.toggleOntologyTerm} />
        </Grid>
      )}
      {props.keywords.length > 0 && (
        <Grid item>
          <Container>
            <Heading variant='h5' as='h2'>
              {t('Ptv.Service.Form.Field.KeywordsAndOntologyTerms.Summary.Keywords.Title', { count: props.keywords.length })}
            </Heading>
            <KeywordChipList keywords={props.keywords} toggleKeyword={props.toggleKeyword} />
          </Container>
        </Grid>
      )}
      {props.gdOntologyTerms.length > 0 && (
        <Grid item>
          <Container>
            <Heading variant='h5' as='h2'>
              {t('Ptv.Service.Form.Field.KeywordsAndOntologyTerms.Summary.GdOntologyTerms.Title', { count: props.gdOntologyTerms.length })}
            </Heading>
            <OntologyTermChipList ontologyTerms={props.gdOntologyTerms} />
          </Container>
        </Grid>
      )}
      {props.gdKeywords.length > 0 && (
        <Grid item>
          <Container>
            <Heading variant='h5' as='h2'>
              {t('Ptv.Service.Form.Field.KeywordsAndOntologyTerms.Summary.GdKeywords.Title', { count: props.gdKeywords.length })}
            </Heading>
            <KeywordChipList keywords={props.gdKeywords} />
          </Container>
        </Grid>
      )}
    </Grid>
  );
}
