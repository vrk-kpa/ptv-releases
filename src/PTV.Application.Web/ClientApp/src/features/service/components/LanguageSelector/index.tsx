import React, { FunctionComponent, useCallback } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfMultiSelect } from 'fields';
import { MultiSelectData } from 'suomifi-ui-components';
import { Language, Mode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { LanguageItem } from 'types/settingTypes';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';
import { getFieldId } from 'utils/fieldIds';

type LanguageSelectorProps = {
  name: string;
  tabLanguage: Language;
  allLanguages: LanguageItem[];
  mode: Mode;
  control: Control<ServiceModel>;
};

export const LanguageSelector: FunctionComponent<LanguageSelectorProps> = (props: LanguageSelectorProps) => {
  const { t } = useTranslation();
  const translate = useTranslateLocalizedText();
  const id = getFieldId(props.name, props.tabLanguage, undefined);

  function getLanguageName(code: string) {
    const item = props.allLanguages.find((x) => x.code === code);
    if (!item) {
      return code;
    }

    return translate(item.names);
  }

  const toItem = (code: string): MultiSelectData => {
    const text = getLanguageName(code);
    return {
      uniqueItemId: code,
      labelText: text,
      chipText: text,
    };
  };

  const sortLangItems = useCallback((a: MultiSelectData, b: MultiSelectData): number => {
    const primaryLangOrder = ['fi', 'sv', 'en'];
    const indexA = primaryLangOrder.indexOf(a.uniqueItemId);
    const indexB = primaryLangOrder.indexOf(b.uniqueItemId);
    if (indexA !== -1 && indexB !== -1) {
      return indexA - indexB;
    } else if (indexA !== -1) {
      return -1;
    } else if (indexB !== -1) {
      return 1;
    } else {
      return a.labelText.localeCompare(b.labelText);
    }
  }, []);

  const languages = props.allLanguages.map((item) => toItem(item.code));

  return (
    <RhfMultiSelect
      control={props.control}
      name={props.name}
      mode={props.mode}
      id={id}
      toItem={toItem}
      items={languages}
      sortItems={sortLangItems}
      labelText={t('Ptv.Service.Form.Field.AvailableLanguages.Label')}
      noItemsText={''}
      chipListVisible={true}
      visualPlaceholder={t('Ptv.Service.Form.Field.AvailableLanguages.PlaceHolder')}
      ariaChipActionLabel={t('Ptv.Service.Form.Field.AvailableLanguages.ChipAction.Label')}
      ariaSelectedAmountText=''
      ariaOptionChipRemovedText=''
      ariaOptionsAvailableText=''
    />
  );
};
