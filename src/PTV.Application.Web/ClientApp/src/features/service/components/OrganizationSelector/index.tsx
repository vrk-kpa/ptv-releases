import React, { useContext } from 'react';
import { Control, UseFormSetValue, useController, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Select from 'react-select';
import { makeStyles } from '@mui/styles';
import { RhfReadOnlyField } from 'fields';
import i18n from 'i18';
import { Text } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { AppContext } from 'context/AppContextProvider';
import { useFormMetaContext } from 'context/formMeta';
import { getUserMainOrganization } from 'context/selectors';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getTextByLangPriority } from 'utils/translations';

type OrganizationSelectorProps = {
  userOrganizations: OrganizationModel[];
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
  language: Language;
  namespace: string;
  enabledLanguages: Language[];
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

export default function OrganizationSelector(props: OrganizationSelectorProps): React.ReactElement {
  const appContext = useContext(AppContext);
  const { mode } = useFormMetaContext();
  const uiLang = useGetUiLanguage();
  const classes = useStyles();
  const { t } = useTranslation();

  const { field } = useController({
    control: props.control,
    name: `${cService.responsibleOrganization}`,
    rules: {
      deps: [`${cService.purchaseProducers}`, `${cService.otherProducers}`],
    },
  });

  const otherResponsibleOrganizations = useWatch({
    control: props.control,
    name: `${cService.otherResponsibleOrganizations}`,
  });

  const selfProducers = useWatch({ control: props.control, name: `${cService.selfProducers}` });
  const hasSelfProducers = useWatch({ control: props.control, name: `${cService.hasSelfProducers}` });

  function onChange(value: SelectOption | null) {
    const org = value?.organization;
    field.onChange(org);

    if (!org || !hasSelfProducers) return;

    // When user selects main responsible organisation, it is also
    // selected as organisation that produces the service
    const selfProducer = selfProducers.find((x) => x.id === org.id);
    if (!selfProducer) {
      props.setValue(`${cService.selfProducers}`, [...selfProducers, org]);
    }
  }

  function toOption(org: OrganizationModel): SelectOption {
    const text = getTextByLangPriority(uiLang, org.texts, org.name);
    return {
      organization: org,
      label: text,
      value: org.id,
    };
  }

  function buildOptionsList(userOrganizations: OrganizationModel[] | undefined, selected: SelectOption | null): SelectOption[] {
    const options = userOrganizations ? userOrganizations.map(toOption) : [];
    const existing = options.find((x) => x.organization.id === selected?.organization.id);

    // Old data might have organization selected that is not a user's organization
    // We need to add it to the list, otherwise react-select does not show it as selected
    if (!existing && selected) {
      options.push(selected);
    }

    // User cannot select organization if it has been selected as other responsible organization
    return options
      .filter((x) => !otherResponsibleOrganizations.find((o) => o.id === x.organization.id))
      .sort((left, right) => left.label.localeCompare(right.label, i18n.language));
  }

  function buildSelected(selectedOrganization: OrganizationModel | null | undefined): SelectOption | null {
    if (selectedOrganization) {
      return toOption(selectedOrganization);
    }

    const userMainOrganization = getUserMainOrganization(appContext);
    return userMainOrganization ? toOption(userMainOrganization) : null;
  }

  const selected = buildSelected(field.value);
  const options = buildOptionsList(props.userOrganizations, selected);
  const fieldId = `${props.namespace}.responsible-organization`;

  if (mode === 'view') {
    return (
      <RhfReadOnlyField id={fieldId} value={selected?.label || ''} labelText={t('Ptv.Service.Form.Field.ResponsibleOrganization.Label')} />
    );
  }

  return (
    <div>
      <div className={classes.title}>
        <Text smallScreen={true} variant='bold'>
          {t('Ptv.Service.Form.Field.ResponsibleOrganization.Label')}
        </Text>
      </div>
      <Select<SelectOption>
        id={fieldId}
        placeholder={t('Ptv.Service.Form.Field.ResponsibleOrganization.Placeholder')}
        options={options}
        value={selected}
        isClearable={false}
        onChange={onChange}
      />
    </div>
  );
}
