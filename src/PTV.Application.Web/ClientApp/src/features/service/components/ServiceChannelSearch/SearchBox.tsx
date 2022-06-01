import React from 'react';
import { useTranslation } from 'react-i18next';
import { TextInput } from 'suomifi-ui-components';

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
    <TextInput
      visualPlaceholder={t('Ptv.Service.Form.ServiceChannelSearch.SearcBox.Placeholder')}
      labelText={t('Ptv.Service.Form.ServiceChannelSearch.SearchBox.Label')}
      icon='search'
      labelMode='visible'
      onChange={onChange}
      value={props.value}
      fullWidth={true}
    />
  );
}
