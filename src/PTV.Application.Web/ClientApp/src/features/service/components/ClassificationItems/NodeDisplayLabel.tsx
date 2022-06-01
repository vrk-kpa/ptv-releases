import React, { FunctionComponent, useContext } from 'react';
import { componentMode } from 'types/enumTypes';
import { CustomChip } from 'components/CustomChip';
import { GDChip } from 'components/GDChip';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';
import { DispatchContext } from 'features/service/contexts/ClassificationItems/DispatchContextProvider';
import { removeItem } from 'features/service/contexts/ClassificationItems/actions';
import { NodeLabel } from './NodeLabel';

interface ChipLabelInterface {
  id: string;
  onRemove?: (id: string) => void;
  setFieldValue: (values: string[]) => void;
  fieldValue: string[];
  label: string;
}

export const NodeDisplayLabel: FunctionComponent<ChipLabelInterface> = ({ id, onRemove, fieldValue, setFieldValue, label }) => {
  const { selectedItems, gdItems, gdOtherItems, elementMode } = useContext(ClassificationItemsContext);
  const dispatch = useContext(DispatchContext);

  const handleOnRemove = (id: string) => {
    onRemove && onRemove(id);
    if (elementMode === componentMode.DISPLAY) {
      if (fieldValue.includes(id)) setFieldValue(fieldValue.filter((x: string) => x !== id));
    } else {
      if (selectedItems.includes(id)) removeItem(dispatch, id);
    }
  };

  const isFromGD = (gdItems && gdItems.includes(id)) || (gdOtherItems && gdOtherItems.includes(id));

  return (
    (isFromGD && (
      <GDChip>
        <NodeLabel label={label} />
      </GDChip>
    )) || (
      <CustomChip onClick={() => handleOnRemove(id)}>
        <NodeLabel label={label} />
      </CustomChip>
    )
  );
};
