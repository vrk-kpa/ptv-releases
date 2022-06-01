import React, { Fragment, FunctionComponent } from 'react';
import Box from '@mui/material/Box';
import { ClassificationItem } from 'types/classificationItemsTypes';
import { SortType } from 'types/miscellaneousTypes';
import { useSortClassificationItems } from 'hooks/useSortClassificationItems';
import TreeNode from './TreeNode';

interface TreeNodeListInterface {
  items: ClassificationItem[];
  sortType?: SortType;
}

export const TreeNodeList: FunctionComponent<TreeNodeListInterface> = ({ items, sortType }) => {
  const sortedItems = useSortClassificationItems(items, sortType) || items;

  return (
    <Fragment>
      {sortedItems.map((child) => (
        <Box mb={1} key={child.id}>
          <TreeNode item={child} />
        </Box>
      ))}
    </Fragment>
  );
};
