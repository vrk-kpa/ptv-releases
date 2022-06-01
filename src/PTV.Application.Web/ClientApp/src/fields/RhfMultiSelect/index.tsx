import React, { useMemo } from 'react';
import { UseControllerProps, useController } from 'react-hook-form';
import { ViewValueList } from 'fields';
import { MultiSelect, MultiSelectData, MultiSelectProps } from 'suomifi-ui-components';
import { Mode } from 'types/enumTypes';
import { warn } from 'utils/debug';
import { toFieldStatus } from 'utils/rhf';

interface RhfMultiSelectProps extends MultiSelectProps<MultiSelectData> {
  name: string;
  id: string;
  toItem: (key: string) => MultiSelectData;
  mode: Mode;
  sortItems?: (a: MultiSelectData, b: MultiSelectData) => number;
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function RhfMultiSelect(props: RhfMultiSelectProps & UseControllerProps<any>): React.ReactElement {
  const { name, toItem, id, mode, sortItems, items, ...rest } = props;
  const { field, fieldState } = useController(props);
  const { status, statusText } = toFieldStatus(fieldState);

  const fieldValue = field.value as unknown[];
  const selectedItems = fieldValue.map((x) => props.toItem(x as string));
  const selectedSortedItems = useMemo(() => {
    return sortItems ? selectedItems.sort(sortItems) : selectedItems;
  }, [selectedItems, sortItems]);

  const sortedItems = useMemo(() => {
    return sortItems ? items.sort(sortItems) : items;
  }, [items, sortItems]);

  function onItemSelect(uniqueItemId: string) {
    const item = sortedItems.find((x) => x.uniqueItemId === uniqueItemId);
    if (!item) {
      warn(`The item ${uniqueItemId} is not part of RhfMultiSelect ${name} items. Cannot select/unselect it.`);
      return;
    }

    const selectedIds = selectedSortedItems.map((x) => x.uniqueItemId);

    let newItems: string[] = [];
    const alreadySelected = selectedIds.find((x) => x === uniqueItemId);
    if (alreadySelected) {
      newItems = selectedIds.filter((x) => x !== uniqueItemId);
    } else {
      newItems = [item.uniqueItemId, ...selectedIds];
    }

    field.onChange(newItems);
  }

  if (mode === 'view') {
    const values = selectedSortedItems.map((value) => value.labelText);
    return <ViewValueList id={props.id} values={values} {...rest} />;
  }

  return (
    <MultiSelect
      id={props.id}
      selectedItems={selectedSortedItems}
      onItemSelect={onItemSelect}
      status={status}
      statusText={statusText}
      items={sortedItems}
      {...rest}
    />
  );
}
