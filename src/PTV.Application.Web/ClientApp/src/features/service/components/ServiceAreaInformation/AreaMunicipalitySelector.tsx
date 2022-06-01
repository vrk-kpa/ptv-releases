import React, { FunctionComponent } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { RhfMultiSelect } from 'fields';
import { MultiSelectData } from 'suomifi-ui-components';
import { Municipality } from 'types/areaTypes';
import { Language, Mode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';
import { getFieldId } from 'utils/fieldIds';

interface AreaMunicipalityTypeInterface {
  name: string;
  tabLanguage: Language;
  allMunicipalities: Municipality[];
  mode: Mode;
  control: Control<ServiceModel>;
}

export const AreaMunicipalitySelector: FunctionComponent<AreaMunicipalityTypeInterface> = (
  props: AreaMunicipalityTypeInterface
): React.ReactElement => {
  const { t } = useTranslation();
  const translate = useTranslateLocalizedText();
  const id = getFieldId(props.name, props.tabLanguage, undefined);

  function getEnumTypeName(id: string) {
    const item = props.allMunicipalities.find((x) => x.id === id);
    if (!item) {
      return id;
    }

    return translate(item.names);
  }

  const toItem = (id: string): MultiSelectData => {
    const text = getEnumTypeName(id);
    return {
      uniqueItemId: id,
      labelText: text,
      chipText: text,
    };
  };

  const allValues = props.allMunicipalities
    .map((x) => toItem(x.id))
    .sort((a, b) => a.labelText.localeCompare(b.labelText, props.tabLanguage));

  return (
    <Box>
      <Box mb={1}>
        <RhfMultiSelect
          control={props.control}
          id={id}
          mode={props.mode}
          name={props.name}
          toItem={toItem}
          items={allValues}
          labelText={t('Ptv.Service.Form.Field.AreaMunicipalities.Label')}
          noItemsText=''
          chipListVisible={true}
          visualPlaceholder={t('Ptv.Service.Form.Field.AreaMunicipalities.PlaceHolder')}
          ariaChipActionLabel={t('Ptv.Form.Chip.ActionLabel.Remove')}
          ariaSelectedAmountText=''
          ariaOptionChipRemovedText=''
          ariaOptionsAvailableText=''
        />
      </Box>
    </Box>
  );
};
