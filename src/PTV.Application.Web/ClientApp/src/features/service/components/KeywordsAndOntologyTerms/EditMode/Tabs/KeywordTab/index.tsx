import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Divider, Grid, styled } from '@mui/material';
import { Button, InlineAlert, TextInput } from 'suomifi-ui-components';
import { OntologyTermsResponse } from 'types/annotationToolTypes';
import { useGetOntologyTermsByName } from 'hooks/queries/useGetOntologyTermsByName';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { KeywordStatus } from 'features/service/components/KeywordsAndOntologyTerms/types';
import { KeywordCreatedNotification } from './KeywordCreatedNotification';
import { LoadingStatus } from './LoadingStatus';

const searchTextMinLength = 1;

const Container = styled('div')({
  marginTop: '20px',
});

const ItemContainer = styled('div')({
  marginTop: '20px',
});

const InputContainer = styled('div')({
  width: '60%',
});

const ResultContainer = styled('div')({
  marginTop: '40px',
});

type KeywordTabProps = {
  isVisible: boolean;
  toggleKeyword: (keyword: string) => void;
  isKeywordSelected: (keyword: string) => boolean;
};

export function KeywordTab(props: KeywordTabProps): React.ReactElement {
  const { t } = useTranslation();
  const uiLang = useGetUiLanguage();
  const [searchText, setSearchText] = useState<string>('');
  const [keywordStatus, setKeywordStatus] = useState<KeywordStatus | undefined>(undefined);

  const query = useGetOntologyTermsByName({ name: searchText, language: uiLang }, { enabled: false, onSuccess: onSuccess });

  function onSuccess(data: OntologyTermsResponse[]) {
    const foundOntologyTerms = data.length > 0;

    if (foundOntologyTerms) {
      setKeywordStatus('OntologyTermsFound');
    } else {
      props.toggleKeyword(searchText);
      setKeywordStatus('KeywordCreated');
    }
  }

  function createFreeKeyword() {
    if (props.isKeywordSelected(searchText)) {
      setKeywordStatus('KeywordAlreadyAdded');
      return;
    }

    setKeywordStatus(undefined);
    query.refetch();
  }

  function onChange(value: string | number | undefined) {
    setSearchText(value ? value.toString() : '');
    setKeywordStatus(undefined);
  }

  // If you want to keep the state when user switches tabs the tab itself must
  // return something. You cannot use e.g. hidden:true (material-ui throws error)
  // and other ways I tried all lost the state that is inside the tab
  if (!props.isVisible) {
    return <></>;
  }

  const createDisabled = query.isLoading || query.isError || searchText.length < searchTextMinLength;

  return (
    <Container>
      <Grid container direction='column'>
        <Grid item>
          <InputContainer>
            <TextInput
              disabled={query.isLoading}
              onChange={onChange}
              fullWidth={true}
              value={searchText}
              labelText={t('Ptv.Service.Form.Field.FreeKeywords.Input.Label')}
              hintText={t('Ptv.Service.Form.Field.FreeKeywords.Input.Hint')}
              visualPlaceholder={t('Ptv.Service.Form.Field.FreeKeywords.Input.Placeholder')}
            />
          </InputContainer>
        </Grid>
        <Grid item>
          <ItemContainer>
            <Button disabled={createDisabled} onClick={createFreeKeyword}>
              {t('Ptv.Service.Form.Field.FreeKeywords.Add.Label')}
            </Button>
          </ItemContainer>
        </Grid>
        <Grid item>
          <ItemContainer>
            <Divider />
          </ItemContainer>
        </Grid>
      </Grid>

      <ResultContainer>
        {query.isLoading && (
          <Grid container justifyContent='center' alignItems='center'>
            <Grid item>
              <ItemContainer>
                <LoadingStatus />
              </ItemContainer>
            </Grid>
          </Grid>
        )}

        {keywordStatus === 'OntologyTermsFound' && (
          <ItemContainer>
            <InlineAlert status='error'>{t('Ptv.Service.Form.Field.FreeKeywords.Error.FoundAsOntologyTerm')}</InlineAlert>
          </ItemContainer>
        )}

        {keywordStatus === 'KeywordAlreadyAdded' && (
          <ItemContainer>
            <InlineAlert status='neutral'>{t('Ptv.Service.Form.Field.FreeKeywords.Error.KeywordAlreadyAdded')}</InlineAlert>
          </ItemContainer>
        )}

        {keywordStatus === 'KeywordCreated' && (
          <ItemContainer>
            <KeywordCreatedNotification keyword={searchText} />
          </ItemContainer>
        )}

        {query.isError && (
          <ItemContainer>
            <InlineAlert status='error'>{t('Ptv.Service.Form.Field.FreeKeywords.Error.DuplicityCheckFailed')}</InlineAlert>
          </ItemContainer>
        )}
      </ResultContainer>
    </Container>
  );
}
