import React, { useState } from 'react';
import { Control, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Select, { InputActionMeta } from 'react-select';
import { makeStyles } from '@mui/styles';
import { Text } from 'suomifi-ui-components';
import { FakeLoginModel } from 'types/loginTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { usePublicGetOrganizations } from 'hooks/queries/usePublicGetOrganizations';
import useTranslateTextNext from 'hooks/useTranslateLocalizedText';

type OrganizationsProps = {
  control: Control<FakeLoginModel>;
};

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

export default function Organizations(props: OrganizationsProps): React.ReactElement | null {
  const translate = useTranslateTextNext();
  const classes = useStyles();
  const { t } = useTranslation();
  const { field } = useController({ control: props.control, name: `organization` });
  const [searchValue, setSearchValue] = useState<string>('');

  const query = usePublicGetOrganizations(
    { searchAll: true, searchValue: searchValue },
    { enabled: !!searchValue, keepPreviousData: true }
  );

  function onInputChange(newValue: string, actionMeta: InputActionMeta) {
    if (newValue.length < 2) {
      return;
    }

    setSearchValue(newValue);
  }

  function onChange(value: SelectOption | null) {
    field.onChange(value?.organization);
  }

  function toOption(org: OrganizationModel) {
    const text = translate(org.texts, org.name);
    return {
      organization: org,
      label: text,
      value: org.id,
    };
  }

  const options = query.data ? query.data.map(toOption) : [];
  const value = field.value ? toOption(field.value) : null;

  return (
    <div>
      <div className={classes.title}>
        <Text smallScreen={true} variant='bold'>
          {t('Ptv.Login.Form.Field.Organization.Label')}
        </Text>
      </div>
      <Select<SelectOption>
        isLoading={query.isLoading}
        onInputChange={onInputChange}
        onChange={onChange}
        options={options}
        value={value}
        placeholder={t('Ptv.Service.Form.ServiceChannelSearch.Organization.Placeholder')}
        isClearable={true}
      />
    </div>
  );
}
