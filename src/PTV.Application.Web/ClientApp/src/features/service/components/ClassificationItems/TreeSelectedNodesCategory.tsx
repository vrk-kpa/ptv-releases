import React, { FunctionComponent, useContext } from 'react';
import Box from '@mui/material/Box';
import { makeStyles } from '@mui/styles';
import { Text } from 'suomifi-ui-components';
import { componentMode } from 'types/enumTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { translateToLang } from 'utils/translations';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';
import { NodeDisplayLabel } from './NodeDisplayLabel';
import { getClassificationItem } from './utils';

const useStyles = makeStyles(() => ({
  count: {
    marginLeft: '5px',
  },
}));

interface TreeSelectedNodesCategoryInterface {
  items: string[] | null | undefined;
  categoryLabel: string;
  emptyMessage?: string;
  hideIfEmpty?: boolean;
  onRemove?: (id: string) => void;
  setFieldValue: (values: string[]) => void;
  fieldValue: string[];
}

export const TreeSelectedNodesCategory: FunctionComponent<TreeSelectedNodesCategoryInterface> = ({
  items,
  categoryLabel,
  emptyMessage,
  hideIfEmpty,
  onRemove,
  fieldValue,
  setFieldValue,
}) => {
  const classes = useStyles();
  const { elementMode, classification } = useContext(ClassificationItemsContext);
  const staticData = useAppContextOrThrow().staticData;
  const uilang = useGetUiLanguage();

  if (!items || (hideIfEmpty && items.length === 0)) return null;

  const hasItems = items.length !== 0;

  return (
    <Box my={2}>
      <Text variant='bold' smallScreen>
        <span>{categoryLabel}</span>
        {elementMode !== componentMode.DISPLAY && <span className={classes.count}>({items.length})</span>}
      </Text>
      <Box>
        {!hasItems && <Text smallScreen>{emptyMessage}</Text>}
        {hasItems && (
          <div>
            {items.map((item) => {
              const classificationItem = getClassificationItem(item, staticData, classification);
              const label = classificationItem ? translateToLang(uilang, classificationItem.names) ?? '' : '';
              return (
                <NodeDisplayLabel
                  key={item}
                  label={label}
                  id={item}
                  onRemove={onRemove}
                  setFieldValue={setFieldValue}
                  fieldValue={fieldValue}
                />
              );
            })}
          </div>
        )}
      </Box>
    </Box>
  );
};
