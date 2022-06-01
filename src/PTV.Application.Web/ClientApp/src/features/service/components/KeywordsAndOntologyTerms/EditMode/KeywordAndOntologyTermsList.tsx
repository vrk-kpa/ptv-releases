import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid, styled } from '@mui/material';
import { Heading, Text } from 'suomifi-ui-components';
import { OntologyTerm } from 'types/classificationItemsTypes';
import { KeywordChipList } from 'features/service/components/KeywordsAndOntologyTerms/components/KeywordChipList';
import { OntologyTermChipList } from 'features/service/components/KeywordsAndOntologyTerms/components/OntologyTermChipList';

const Container = styled('div')({
  marginTop: '20px',
});

type KeywordAndOntologyTermsListProps = {
  ontologyTerms: OntologyTerm[];
  gdOntologyTerms: OntologyTerm[];
  keywords: string[];
  gdKeywords: string[];
  toggleOntologyTerm: (term: OntologyTerm) => void;
  toggleKeyword: (keyword: string) => void;
};

export function KeywordAndOntologyTermsList(props: KeywordAndOntologyTermsListProps): React.ReactElement {
  const { t } = useTranslation();
  return (
    <Grid container direction='column'>
      <Grid item>
        <Container>
          <Heading variant='h5' as='h2'>
            {t(`Ptv.Service.Form.Field.OntologyTerms.Display.Selection.Label`)}
          </Heading>
          {props.ontologyTerms.length > 0 ? (
            <OntologyTermChipList ontologyTerms={props.ontologyTerms} toggleOntologyTerm={props.toggleOntologyTerm} />
          ) : (
            <Text>{t(`Ptv.Service.Form.Field.OntologyTerms.Display.Selection.Placeholder`)}</Text>
          )}
        </Container>
      </Grid>
      <Grid item>
        <Container>
          <Heading variant='h5' as='h2'>
            {t(`Ptv.Service.Form.Field.FreeKeywords.Display.Selection.Label`)}
          </Heading>
          {props.keywords.length > 0 ? (
            <KeywordChipList keywords={props.keywords} toggleKeyword={props.toggleKeyword} />
          ) : (
            <Text>{t(`Ptv.Service.Form.Field.FreeKeywords.Display.Selection.Placeholder`)}</Text>
          )}
        </Container>
      </Grid>
      {props.gdOntologyTerms.length > 0 && (
        <Grid item>
          <Container>
            <Heading variant='h5' as='h2'>
              {t(`Ptv.Service.Form.Field.OntologyTerms.Display.SelectionGD.Label`)}
            </Heading>
            <OntologyTermChipList ontologyTerms={props.gdOntologyTerms} />
          </Container>
        </Grid>
      )}
      {props.gdKeywords.length > 0 && (
        <Grid item>
          <Container>
            <Heading variant='h5' as='h2'>
              {t(`Ptv.Service.Form.Field.FreeKeywords.Display.SelectionGD.Label`)}
            </Heading>
            <KeywordChipList keywords={props.gdKeywords} />
          </Container>
        </Grid>
      )}
    </Grid>
  );
}
