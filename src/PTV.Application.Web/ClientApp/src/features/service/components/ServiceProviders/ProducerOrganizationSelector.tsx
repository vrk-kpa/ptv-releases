import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { ViewValueList } from 'fields';
import i18nPtv from 'i18';
import { MultiSelectData } from 'suomifi-ui-components';
import { OrganizationModel } from 'types/organizationTypes';
import { CustomChip } from 'components/CustomChip';
import { InvalidChip } from 'components/InvalidChip';
import { PtvMultiSelect } from 'components/PtvMultiSelect';
import { useFormMetaContext } from 'context/formMeta';
import { usePublicGetOrganizations } from 'hooks/queries/usePublicGetOrganizations';
import { useDebounceValue } from 'hooks/useDebounceValue';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { MultiSelectItem, organizationModelToMultiSelectItem } from 'utils/multiselect';

interface IProducerOrganizationSelector {
  id: string;
  label: string;
  placeholder: string;
  selectedOrganizations: OrganizationModel[];
  responsibleOrganizationIds: string[];
  selectOrganizations: (organizations: OrganizationModel[]) => void;
}

const searchTextMinLength = 3;

export default function ProducerOrganizationSelector(props: IProducerOrganizationSelector): React.ReactElement {
  const { t } = useTranslation();
  const { mode } = useFormMetaContext();
  const uiLang = useGetUiLanguage();

  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounceValue(searchText, 300);
  const [optionItems, setOptionItems] = useState<MultiSelectItem[]>([]);
  const [selectedItems, setSelectedItems] = useState<MultiSelectItem[]>(
    props.selectedOrganizations.map((organization) => organizationModelToMultiSelectItem(organization, uiLang))
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
              .map((organization) => organizationModelToMultiSelectItem(organization, uiLang))
              .sort((left, right) => left.labelText.localeCompare(right.labelText, i18nPtv.language))
          : [];
        setOptionItems(items);
      },
    }
  );

  const multiSelectItemToOrganizationModel = (multiselectItem: MultiSelectItem): OrganizationModel | undefined => {
    return (
      queryData?.find((queryItem) => queryItem.id === multiselectItem.uniqueItemId) ||
      props.selectedOrganizations.find((fieldItem) => fieldItem.id === multiselectItem.uniqueItemId)
    );
  };

  const onItemSelect = (uniqueItemId: string) => {
    if (selectedItems.some((x) => x.uniqueItemId === uniqueItemId)) {
      // Remove from selectedItems
      const items = selectedItems.filter((x) => x.uniqueItemId !== uniqueItemId);
      setSelectedItems(items);
      onFieldChange(items);
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
    props.selectOrganizations(selectedOrgs);
  };

  const handleRemove = (uniqueItemId: string) => {
    onItemSelect(uniqueItemId);
  };

  const onValueChange = (value: string) => {
    if (value.length < searchTextMinLength) {
      setOptionItems([]);
    }
    setSearchText(value);
  };

  if (mode === 'view') {
    const values = selectedItems.map((value) => value.labelText);
    return <ViewValueList id={props.id} values={values} labelText={props.label} />;
  }

  return (
    <Box>
      <PtvMultiSelect
        className={queryIsLoading ? 'loading' : ''}
        id={props.id}
        items={optionItems}
        selectedItems={selectedItems}
        onChange={onValueChange}
        // Blur removes the inputed text in suomifi component. We need to clear the searchText in state also
        onBlur={() => onValueChange('')}
        onItemSelect={onItemSelect}
        labelText={props.label}
        noItemsText=''
        chipListVisible={false}
        visualPlaceholder={props.placeholder}
        ariaChipActionLabel={t('Ptv.Form.Chip.ActionLabel.Remove')}
        ariaSelectedAmountText=''
        ariaOptionChipRemovedText=''
        ariaOptionsAvailableText=''
        hintText={t('Ptv.Service.Form.Field.ProducerOrganization.HintText')}
      ></PtvMultiSelect>

      <Box>
        {selectedItems.map((item: MultiSelectData) => {
          if (props.responsibleOrganizationIds.includes(item.uniqueItemId)) {
            return (
              <InvalidChip key={item.uniqueItemId} onClick={() => handleRemove(item.uniqueItemId)}>
                {item.labelText}
              </InvalidChip>
            );
          } else {
            return (
              <CustomChip key={item.uniqueItemId} onClick={() => handleRemove(item.uniqueItemId)}>
                {item.labelText}
              </CustomChip>
            );
          }
        })}
      </Box>
    </Box>
  );
}
