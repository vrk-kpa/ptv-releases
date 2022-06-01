import React from 'react';
import { useTranslation } from 'react-i18next';
import { OntologyTermTreeType } from 'types/classificationItemsTypes';
import { VisualHeading } from 'components/VisualHeading';
import { translateToLang } from 'utils/translations';
import { getSearchResultCount } from 'features/service/components/KeywordsAndOntologyTerms/utils';

type SearchResultInfoProps = {
  term: OntologyTermTreeType | null;
};

export function SearchResultInfo(props: SearchResultInfoProps): React.ReactElement {
  const { t } = useTranslation();
  const count = getSearchResultCount(props.term);
  const searchText = props.term ? translateToLang('fi', props.term?.names, '') ?? '' : '';
  const text = t('Ptv.Service.Form.Field.OntologyTerms.Select.Search.Found', { searchValue: searchText, count: count });
  return <VisualHeading variant='h5'>{text}</VisualHeading>;
}
