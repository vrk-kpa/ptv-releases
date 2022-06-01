import React, { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { MultiSelect, MultiSelectData } from 'suomifi-ui-components';
import { DigitalAuthorizationModel } from 'types/digitalAuthTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';
import { getTopLevelGroups } from 'utils/digitalAuth';
import { localeCompareTexts } from 'utils/translations';
import { toFieldId } from 'features/connection/utils/fieldid';

interface MultiSelectItem extends MultiSelectData {
  isSelected: boolean;
  isGroup: boolean;
  authModel: DigitalAuthorizationModel;
}

type AuthorizationSelectorProps = {
  selected: string[];
  toggle: (authMode: DigitalAuthorizationModel, isGroup: boolean, isSelected: boolean) => void;
};

export function AuthorizationSelector(props: AuthorizationSelectorProps): React.ReactElement {
  const { t } = useTranslation();
  const groupPostfix = t('Ptv.ConnectionDetails.Authorization.Selector.GroupSuffix');
  const translate = useTranslateLocalizedText();
  const lang = useGetUiLanguage();
  const ctx = useAppContextOrThrow();
  const digitalAuthorizations = ctx.staticData.digitalAuthorizations;
  const selectedIds = props.selected;

  const groups = useMemo(() => {
    return getTopLevelGroups(digitalAuthorizations);
  }, [digitalAuthorizations]);

  const items = useMemo(() => {
    function createOption(authModel: DigitalAuthorizationModel, isGroup: boolean, isSelected: boolean): MultiSelectItem {
      const postFix = isGroup ? groupPostfix : '';
      return {
        isGroup: isGroup,
        isSelected: isSelected,
        authModel: authModel,
        uniqueItemId: authModel.id,
        labelText: translate(authModel.names, authModel.id) + postFix,
      };
    }

    function createOptionList(groups: DigitalAuthorizationModel[]): MultiSelectItem[] {
      const result: MultiSelectItem[] = [];
      for (const group of groups) {
        if (group.children.length === 0) continue;

        // If all the children have been selected it means the group has also been selected
        const groupSelected = group.children.every((x) => selectedIds.includes(x.id));
        result.push(createOption(group, true, groupSelected));

        for (const child of group.children) {
          result.push(createOption(child, false, selectedIds.includes(child.id)));
        }
      }

      return result.sort((left, right) => localeCompareTexts(left.authModel.names, right.authModel.names, lang));
    }

    return createOptionList(groups);
  }, [groups, lang, translate, selectedIds, groupPostfix]);

  function onItemToggle(uniqueItemId: string) {
    const item = items.find((x) => x.authModel.id === uniqueItemId);
    if (!item) return;
    props.toggle(item.authModel, item.isGroup, item.isSelected);
  }

  const selected = items.filter((x) => x.isSelected);

  return (
    <MultiSelect
      id={toFieldId('authorization-selector')}
      debounce={500}
      items={items}
      selectedItems={selected}
      onItemSelect={onItemToggle}
      labelText={t('Ptv.ConnectionDetails.Authorization.Selector.Label')}
      noItemsText=''
      chipListVisible={false}
      visualPlaceholder={t('Ptv.ConnectionDetails.Authorization.Selector.Placeholder')}
      ariaSelectedAmountText=''
      ariaOptionsAvailableText=''
      ariaOptionChipRemovedText=''
    ></MultiSelect>
  );
}
