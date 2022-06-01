import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Box, Divider, Grid } from '@mui/material';
import { styled } from '@mui/material/styles';
import { SingleSelect, SingleSelectData } from 'suomifi-ui-components';
import { OntologyTermsListRequest, OntologyTermsResponse } from 'types/annotationToolTypes';
import { OntologyTerm, OntologyTermTreeType } from 'types/classificationItemsTypes';
import LoadingIndicator from 'components/LoadingIndicator';
import { useFetchTermTreeQuery, useFetchTermsListQuery } from 'hooks/queries/useKeywordsQueries';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { translateToLang } from 'utils/translations';
import { getFirstTerm } from 'features/service/components/KeywordsAndOntologyTerms/utils';
import { Error } from './Error';
import { SearchResultInfo } from './SearchResultInfo';
import { SearchResults } from './SearchResults';

const searchTextMinLength = 3;

const LoadingContainer = styled('div')({
  marginLeft: '15px',
});

const ItemContainer = styled('div')({
  marginTop: '20px',
});

const Container = styled('div')({
  marginTop: '20px',
});

const ResultContainer = styled('div')({
  paddingTop: '20px',
});

type OntologyTabProps = {
  isVisible: boolean;
  toggleOntologyTerm: (ontologyTerm: OntologyTerm) => void;
  isOntologyTermChecked: (ontologyTerm: OntologyTerm) => boolean;
  isOntologyTermDisabled: (OntologyTerm: OntologyTerm) => boolean;
};

export function OntologyTab(props: OntologyTabProps): React.ReactElement {
  const { t } = useTranslation();
  const [searchText, setSearchText] = useState<string>('');
  const [searchResults, setSearchResults] = useState<SingleSelectData[]>([]);
  const [termId, setTermId] = useState<string | null>(null);
  const [term, setTerm] = useState<OntologyTermTreeType | null>(null);
  const uiLanguage = useGetUiLanguage();
  const termsListPayload: OntologyTermsListRequest = { language: uiLanguage, searchValue: searchText };

  const termListQuery = useFetchTermsListQuery(termsListPayload, {
    onSuccess: onFetchTermsListSuccess,
    onError: () => {
      setSearchResults([]);
    },
    enabled: searchText.length >= searchTextMinLength,
  });

  const termTreeQuery = useFetchTermTreeQuery([termId ?? ''], {
    enabled: !!termId,
    onSuccess: (data: OntologyTermTreeType[]) => setTerm(getFirstTerm(data)),
  });

  function toSingleSelectData(item: OntologyTermsResponse | OntologyTerm): SingleSelectData {
    return {
      uniqueItemId: item.id,
      labelText: translateToLang(uiLanguage, item.names, '') ?? '',
    };
  }

  function onFetchTermsListSuccess(data: OntologyTermsResponse[]): void {
    const searchResults = data.map((item) => toSingleSelectData(item));
    setSearchResults(searchResults);
  }

  function onItemSelect(uniqueItemId: string | null) {
    if (!uniqueItemId) {
      setSearchText('');
      setTerm(null);
    }

    setTermId(uniqueItemId);
  }

  const isError = termListQuery.isError || termTreeQuery.isError;
  const queryInProgress = termListQuery.isFetching || termTreeQuery.isFetching;
  const showSearchInfo = !isError && term;
  const noItemsText = queryInProgress ? '' : t('Ptv.Service.Form.Field.OntologyTerms.Select.Search.NotFound');
  const selectedItem = term ? toSingleSelectData(term) : undefined;

  // If you want to keep the state when user switches tabs the tab itself must
  // return something. You cannot use e.g. hidden:true (material-ui throws error)
  // and other ways I tried all lost the state that is inside the tab
  if (!props.isVisible) {
    return <></>;
  }

  return (
    <Container>
      <Grid container alignItems='flex-end' direction='row'>
        <Grid item>
          <SingleSelect
            id='ontologyterm-select-input'
            labelText={t('Ptv.Service.Form.Field.OntologyTerms.Select.Search.Label')}
            hintText={t('Ptv.Service.Form.Field.OntologyTerms.Select.Search.Hint')}
            clearButtonLabel={t('Ptv.Service.Form.Field.OntologyTerms.Select.Search.Clear')}
            visualPlaceholder={t('Ptv.Service.Form.Field.OntologyTerms.Select.Search.Placeholder')}
            ariaOptionsAvailableText={t('Ptv.Service.Form.Field.OntologyTerms.Select.Search.OptionsAvailable')}
            noItemsText={noItemsText}
            items={searchResults}
            defaultSelectedItem={selectedItem}
            debounce={200}
            onChange={(value: string) => setSearchText(value)}
            onItemSelect={onItemSelect}
          />
        </Grid>
        {queryInProgress && (
          <Grid item>
            <LoadingContainer>
              <LoadingIndicator size='30px' />
            </LoadingContainer>
          </Grid>
        )}
      </Grid>

      <Grid container direction='column'>
        <Grid item>
          <ItemContainer>
            <Box visibility={showSearchInfo ? 'visible' : 'hidden'}>
              <SearchResultInfo term={term} />
            </Box>
          </ItemContainer>
        </Grid>
        <Grid item>
          <ItemContainer>
            <Divider />
          </ItemContainer>
        </Grid>
      </Grid>

      <ResultContainer>
        {!isError && (
          <Grid container direction='column'>
            <Grid item>
              <SearchResults
                term={term}
                toggleOntologyTerm={props.toggleOntologyTerm}
                isOntologyTermChecked={props.isOntologyTermChecked}
                isOntologyTermDisabled={props.isOntologyTermDisabled}
              />
            </Grid>
          </Grid>
        )}

        {isError && <Error termListQueryFailed={termListQuery.isError} termTreeQueryFailed={termTreeQuery.isError} />}
      </ResultContainer>
    </Container>
  );
}
