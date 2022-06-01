import React from 'react';
import { useTranslation } from 'react-i18next';
import { InlineAlert } from 'suomifi-ui-components';

type ErrorProps = {
  termListQueryFailed: boolean;
  termTreeQueryFailed: boolean;
};

export function Error(props: ErrorProps): React.ReactElement | null {
  const { t } = useTranslation();
  if (!props.termListQueryFailed && !props.termTreeQueryFailed) return null;
  const msg = props.termListQueryFailed
    ? t('Ptv.Service.Form.Field.OntologyTerms.Search.SearchError')
    : t('Ptv.Service.Form.Field.OntologyTerms.Term.FetchError');
  return <InlineAlert status='error'>{msg}</InlineAlert>;
}
