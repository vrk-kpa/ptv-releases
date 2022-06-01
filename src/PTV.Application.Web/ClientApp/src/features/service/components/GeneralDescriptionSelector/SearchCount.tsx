import React from 'react';
import { useTranslation } from 'react-i18next';
import { Text } from 'suomifi-ui-components';

type SearchCountProps = {
  count: number;
};

export default function SearchCount(props: SearchCountProps): React.ReactElement {
  const { t } = useTranslation();
  return (
    <Text variant='bold' smallScreen={true}>
      {props.count + ' ' + t('Ptv.Service.Form.GdSearch.SearchCount.Text')}
    </Text>
  );
}
