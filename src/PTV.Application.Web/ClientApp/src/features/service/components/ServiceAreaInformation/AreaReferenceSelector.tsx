import React, { FunctionComponent } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { RhfMultiSelect } from 'fields';
import { MultiSelectData } from 'suomifi-ui-components';
import { Region } from 'types/areaTypes';
import { Language, Mode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getFieldId } from 'utils/fieldIds';
import { translateToLang } from 'utils/translations';
import AreaOverview from './AreaOverview';

interface AreaReferenceTypeInterface {
  name: string;
  tabLanguage: Language;
  allItems: Region[];
  label: string;
  placeholder: string;
  areaOverviewLabel: string;
  areaOverviewButtonOpenText: string;
  mode: Mode;
  control: Control<ServiceModel>;
}

export const AreaReferenceSelector: FunctionComponent<AreaReferenceTypeInterface> = (props) => {
  const uiLang = useGetUiLanguage();
  const { t } = useTranslation();
  const id = getFieldId(props.name, props.tabLanguage, undefined);

  function getEnumTypeName(id: string) {
    const item = props.allItems.find((x) => x.id === id);
    if (!item) {
      return id;
    }
    return translateToLang(uiLang, item.names) ?? '';
  }

  const toItem = (id: string): MultiSelectData => {
    const text = getEnumTypeName(id);
    return {
      uniqueItemId: id,
      labelText: text,
      chipText: text,
    };
  };

  const allMultiSelectItems = props.allItems
    .map((value) => toItem(value.id))
    .sort((a, b) => a.labelText.localeCompare(b.labelText, uiLang));

  return (
    <Box mb={1}>
      <RhfMultiSelect
        control={props.control}
        id={id}
        name={props.name}
        mode={props.mode}
        toItem={toItem}
        items={allMultiSelectItems}
        labelText={props.label}
        noItemsText=''
        chipListVisible={true}
        visualPlaceholder={props.placeholder}
        ariaChipActionLabel={t('Ptv.Form.Chip.ActionLabel.Remove')}
        ariaSelectedAmountText=''
        ariaOptionChipRemovedText=''
        ariaOptionsAvailableText=''
      />
      <AreaOverview items={props.allItems} overviewLabel={props.areaOverviewLabel} buttonOpenText={props.areaOverviewButtonOpenText} />
    </Box>
  );
};
