import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import Select from 'react-select';
import { makeStyles } from '@mui/styles';
import { Label } from 'suomifi-ui-components';
import { OrganizationModel } from 'types/organizationTypes';
import { usePublicGetOrganizations } from 'hooks/queries/usePublicGetOrganizations';
import { useDebounceValue } from 'hooks/useDebounceValue';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';

type SelectOption = {
  value: string;
  label: string;
  organization: OrganizationModel;
};

const useStyles = makeStyles(() => ({
  title: {
    marginBottom: '10px',
  },
}));

type OrganizationSelectorProps = {
  selected: OrganizationModel | null | undefined;
  onSelect: (org: OrganizationModel | null | undefined) => void;
};

const searchMinLength = 3;

export default function OrganizationSelector(props: OrganizationSelectorProps): React.ReactElement {
  const classes = useStyles();
  const translate = useTranslateLocalizedText();
  const { t } = useTranslation();
  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounceValue(searchText, 300);

  const query = usePublicGetOrganizations(
    {
      searchAll: true,
      searchValue: debouncedSearchText,
    },
    {
      enabled: debouncedSearchText.length >= searchMinLength,
    }
  );

  function toOption(org: OrganizationModel) {
    const text = translate(org.texts, org.name);
    return {
      organization: org,
      label: text,
      value: org.id,
    };
  }

  function onChange(value: SelectOption | null) {
    props.onSelect(value?.organization);
  }

  const options = query.data && debouncedSearchText.length >= searchMinLength ? query.data.map(toOption) : [];
  const value = props.selected ? toOption(props.selected) : null;

  return (
    <div>
      <div className={classes.title}>
        <Label>{t('Ptv.Service.Form.ServiceChannelSearch.Organization.Label')}</Label>
      </div>
      <Select<SelectOption>
        isLoading={query.isLoading}
        onInputChange={(value: string) => setSearchText(value)}
        onChange={onChange}
        options={options}
        value={value}
        placeholder={t('Ptv.Service.Form.ServiceChannelSearch.Organization.Placeholder')}
        isClearable={true}
      />
    </div>
  );
}
