import React from 'react';
import { useTranslation } from 'react-i18next';
import { SearchInput } from 'suomifi-ui-components';

type SearchBoxProps = {
  value: string;
  onChange: (value: string) => void;
};

export default function SearchBox(props: SearchBoxProps): React.ReactElement {
  const { t } = useTranslation();
  function onChange(newValue: string | number | undefined) {
    props.onChange(newValue ? newValue.toString() : '');
  }

  return (
    <SearchInput
      visualPlaceholder={t('Ptv.Service.Form.GdSearch.SearcBox.Placeholder')}
      labelText={t('Ptv.Service.Form.GdSearch.SearchBox.Label')}
      searchButtonLabel={t('Ptv.Service.Form.GdSearch.SearchBox.SearchButton.Label')}
      clearButtonLabel={t('Ptv.Service.Form.GdSearch.SearchBox.ClearButton.Label')}
      labelMode='visible'
      onChange={onChange}
      value={props.value}
    />
  );
}
