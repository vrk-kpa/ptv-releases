import React, { useState } from 'react';
import { Control, UseFormSetValue, useController, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ViewValueList } from 'fields';
import i18nPtv from 'i18';
import { Language } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { PtvMultiSelect } from 'components/PtvMultiSelect';
import { useFormMetaContext } from 'context/formMeta';
import { usePublicGetOrganizations } from 'hooks/queries/usePublicGetOrganizations';
import { useDebounceValue } from 'hooks/useDebounceValue';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { withNamespace } from 'utils/fieldIds';
import { MultiSelectItem, organizationModelToMultiSelectItem } from 'utils/multiselect';

type OtherOrganizationSelectorProps = {
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
  namespace: string;
  enabledLanguages: Language[];
};

const searchTextMinLength = 3;

export default function OtherOrganizationSelector(props: OtherOrganizationSelectorProps): React.ReactElement {
  const { mode } = useFormMetaContext();
  const uiLang = useGetUiLanguage();
  const { t } = useTranslation();

  const { field } = useController({
    control: props.control,
    name: `${cService.otherResponsibleOrganizations}`,
    rules: {
      deps: [`${cService.purchaseProducers}`, `${cService.otherProducers}`],
    },
  });

  const responsibleOrganization = useWatch({ control: props.control, name: `${cService.responsibleOrganization}` });
  const selfProducers = useWatch({ control: props.control, name: `${cService.selfProducers}` });
  const hasSelfProducers = useWatch({ control: props.control, name: `${cService.hasSelfProducers}` });

  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounceValue(searchText, 300);
  const [optionItems, setOptionItems] = useState<MultiSelectItem[]>([]);
  const [selectedItems, setSelectedItems] = useState<MultiSelectItem[]>(
    field.value.map((organization) => organizationModelToMultiSelectItem(organization, uiLang))
  );

  const { data: queryData, isLoading: queryIsLoading } = usePublicGetOrganizations(
    {
      searchAll: true,
      searchValue: debouncedSearchText,
    },
    {
      enabled: debouncedSearchText.length >= searchTextMinLength && mode === 'edit',
      onSettled: (data, _error) => {
        const items = data
          ? data
              .filter(({ publishingStatus }) => publishingStatus === 'Published')
              .map((organization) => {
                return organizationModelToMultiSelectItem(organization, uiLang);
              })
              .filter((x) => x.organization.id !== responsibleOrganization?.id)
              .sort((left, right) => left.labelText.localeCompare(right.labelText, i18nPtv.language))
          : [];

        setOptionItems(items);
      },
    }
  );

  function removeSelfProducer(organization: OrganizationModel) {
    if (!hasSelfProducers) return;

    const selfProducer = selfProducers.find((x) => x.id === organization.id);
    if (!selfProducer) return;

    props.setValue(
      `${cService.selfProducers}`,
      selfProducers.filter((x) => x.id !== organization.id)
    );
  }

  const multiSelectItemToOrganizationModel = (multiselectItem: MultiSelectItem): OrganizationModel | undefined => {
    return (
      queryData?.find((queryItem) => queryItem.id === multiselectItem.uniqueItemId) ||
      field.value.find((fieldItem) => fieldItem.id === multiselectItem.uniqueItemId)
    );
  };

  const onItemSelect = (uniqueItemId: string) => {
    if (selectedItems.some((x) => x.uniqueItemId === uniqueItemId)) {
      // Remove from selectedItems
      const items = selectedItems.filter((x) => x.uniqueItemId !== uniqueItemId);
      const itemToRemove = selectedItems.find((x) => x.uniqueItemId === uniqueItemId);
      setSelectedItems(items);
      onFieldChange(items);

      if (itemToRemove) {
        const org = multiSelectItemToOrganizationModel(itemToRemove);
        if (org) {
          removeSelfProducer(org);
        }
      }
    } else {
      // Add item to selectedItems
      const item = optionItems.find((x) => x.uniqueItemId === uniqueItemId);
      if (item) {
        const items = [...selectedItems, item];
        setSelectedItems(items);
        onFieldChange(items);
      }
    }
  };

  const onFieldChange = (items: MultiSelectItem[]) => {
    // Map to OrganizationModel
    const selectedOrgs = items.map((x) => multiSelectItemToOrganizationModel(x)).filter((org) => org !== undefined) as OrganizationModel[];
    field.onChange(selectedOrgs);
  };

  const onValueChange = (value: string) => {
    if (value.length < searchTextMinLength) {
      setOptionItems([]);
    }
    setSearchText(value);
  };

  const fieldId = withNamespace(props.namespace, `${cService.otherResponsibleOrganizations}`);
  const labelText = t('Ptv.Service.Form.Field.OtherOrganizations.Label');

  if (mode === 'view') {
    const values = selectedItems.map((value) => value.labelText);
    return <ViewValueList id={fieldId} values={values} labelText={labelText} />;
  }

  return (
    <PtvMultiSelect
      className={queryIsLoading ? 'loading' : ''}
      id={fieldId}
      items={optionItems}
      selectedItems={selectedItems}
      onChange={onValueChange}
      // Blur removes the inputed text in suomifi component. We need to clear the searchText in state also
      onBlur={() => onValueChange('')}
      onItemSelect={onItemSelect}
      labelText={labelText}
      noItemsText=''
      chipListVisible={true}
      visualPlaceholder={t('Ptv.Service.Form.Field.OtherOrganizations.Placeholder')}
      ariaChipActionLabel={t('Ptv.Form.Chip.ActionLabel.Remove')}
      ariaSelectedAmountText=''
      ariaOptionChipRemovedText=''
      ariaOptionsAvailableText=''
      hintText={t('Ptv.Service.Form.Field.OtherOrganizations.HintText')}
      optionalText={t('Ptv.Common.Optional')}
    ></PtvMultiSelect>
  );
}
