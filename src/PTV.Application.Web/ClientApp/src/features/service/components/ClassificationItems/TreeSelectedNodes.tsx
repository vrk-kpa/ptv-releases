import React, { FunctionComponent, useContext, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { capitalize } from '@mui/material';
import { Block } from 'suomifi-ui-components';
import { componentMode } from 'types/enumTypes';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';
import { DispatchContext } from 'features/service/contexts/ClassificationItems/DispatchContextProvider';
import { TreeSelectedNodesCategory } from './TreeSelectedNodesCategory';

export const useGetItems = (fieldValue: string[]): string[] => {
  const { selectedItems, gdItems, elementMode } = useContext(ClassificationItemsContext);
  const dispatch = useContext(DispatchContext);
  const [result, setResult] = useState<string[]>([]);

  useEffect(() => {
    const selectedFiltered = selectedItems.filter((x: string) => !gdItems.includes(x));
    const fieldFiltered = fieldValue.filter((x: string) => !gdItems.includes(x));
    elementMode === componentMode.DISPLAY ? setResult(fieldFiltered) : setResult(selectedFiltered);
  }, [dispatch, selectedItems, fieldValue, elementMode, gdItems]);

  return result;
};

type TreeSelectedNodesProps = {
  fieldValue: string[];
  setFieldValue: (values: string[]) => void;
};

export const TreeSelectedNodes: FunctionComponent<TreeSelectedNodesProps> = (props: TreeSelectedNodesProps) => {
  const { t } = useTranslation();
  const { classification, gdItems } = useContext(ClassificationItemsContext);

  const items = useGetItems(props.fieldValue);

  return (
    <Block>
      <TreeSelectedNodesCategory
        categoryLabel={t(`Ptv.Service.Form.Field.${capitalize(classification)}.Display.Selection.Label`)}
        emptyMessage={t(`Ptv.Service.Form.Field.${capitalize(classification)}.Display.Selection.Placeholder`)}
        items={items}
        fieldValue={props.fieldValue}
        setFieldValue={props.setFieldValue}
      />
      <TreeSelectedNodesCategory
        categoryLabel={t(`Ptv.Service.Form.Field.${capitalize(classification)}.Display.SelectionGD.Label`)}
        hideIfEmpty
        items={gdItems}
        fieldValue={props.fieldValue}
        setFieldValue={props.setFieldValue}
      />
    </Block>
  );
};
