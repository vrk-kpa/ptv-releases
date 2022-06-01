import React, { useCallback, useContext, useMemo } from 'react';
import { Control, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Block, MultiSelect, MultiSelectData } from 'suomifi-ui-components';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { AppContext } from 'context/AppContextProvider';
import { useFormMetaContext } from 'context/formMeta';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getTextByLangPriority } from 'utils/translations';
import { GdLifeEvents } from './GdLifeEvents';
import { LifeEventsList } from './LifeEventsList';

interface ILifeEventsSelect {
  labelText: string;
  gdItems: string[];
  control: Control<ServiceModel>;
}

export function LifeEventsSelect(props: ILifeEventsSelect): React.ReactElement {
  const { t } = useTranslation();
  const appContext = useContext(AppContext);
  const lifeEvents = appContext.staticData.lifeEvents;
  const { mode } = useFormMetaContext();
  const uiLang = useGetUiLanguage();
  const { gdItems, control, labelText } = props;

  const { field } = useController({ control: control, name: `${cService.lifeEvents}` });

  const toItem = useCallback(
    (id: string): MultiSelectData => {
      const lifeEvent = lifeEvents.find((x) => x.id === id);
      const text = lifeEvent ? getTextByLangPriority(uiLang, lifeEvent.names, '') : '';
      return {
        uniqueItemId: id,
        labelText: text,
        chipText: text,
        disabled: gdItems.includes(id),
      };
    },
    [gdItems, lifeEvents, uiLang]
  );

  const items = useMemo(() => {
    return lifeEvents.map((x) => toItem(x.id)).sort(compareItems);
  }, [lifeEvents, toItem]);

  const gdLifeEvents = useMemo(() => {
    return gdItems.map((x) => toItem(x)).sort(compareItems);
  }, [gdItems, toItem]);

  const handleRemove = (id: string) => {
    const newValue = field.value.filter((selectedId) => selectedId !== id);
    field.onChange(newValue);
  };

  const onItemSelect = (uniqueItemId: string) => {
    if (field.value.includes(uniqueItemId)) {
      field.onChange(field.value.filter((x) => x !== uniqueItemId));
    } else {
      field.onChange([...field.value, uniqueItemId]);
    }
  };

  const selectedItems = field.value.map((x) => toItem(x)).sort(compareItems);
  const withoutGdItems = field.value
    .filter((x) => !gdItems.includes(x))
    .map((x) => toItem(x))
    .sort(compareItems);

  return (
    <Block>
      {mode === 'edit' && (
        <MultiSelect
          id={cService.lifeEvents}
          onItemSelect={onItemSelect}
          items={items}
          selectedItems={selectedItems}
          labelText={labelText}
          noItemsText={t('Ptv.Service.Form.Field.LifeEvents.Selection.Empty')}
          chipListVisible={false}
          visualPlaceholder={t('Ptv.Service.Form.Field.LifeEvents.Select.PlaceHolder')}
          ariaChipActionLabel={t('Ptv.Form.Chip.ActionLabel.Remove')}
          ariaSelectedAmountText=''
          ariaOptionChipRemovedText=''
          ariaOptionsAvailableText=''
        />
      )}
      <LifeEventsList items={withoutGdItems} remove={handleRemove} />
      {gdLifeEvents.length > 0 && <GdLifeEvents items={gdLifeEvents} />}
    </Block>
  );
}

function compareItems(left: MultiSelectData, right: MultiSelectData): number {
  return left.labelText.localeCompare(right.labelText);
}
