import React, { FunctionComponent, useContext, useEffect, useState } from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { Expander, ExpanderContent, ExpanderTitleButton } from 'suomifi-ui-components';
import { ClassificationItemParsed } from 'types/classificationItemsTypes';
import { cService } from 'types/forms/serviceFormTypes';
import { SortType } from 'types/miscellaneousTypes';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { useSortClassificationItems } from 'hooks/useSortClassificationItems';
import { translateToLang } from 'utils/translations';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';
import { NodeLabel } from './NodeLabel';
import { TreeNodeSub } from './TreeNodeSub';

const useStyles = makeStyles(() => ({
  mainCategory: {
    '&.isOpen': {
      marginBottom: '20px',
      marginTop: '20px',
    },
    '&:first-child': {
      marginTop: 0,
    },
  },
}));

interface TreeNodeMainInterface {
  item: ClassificationItemParsed;
}

export const TreeNodeMain: FunctionComponent<TreeNodeMainInterface> = ({ item }) => {
  const { id, names, count } = item;
  const { searchValue, classification } = useContext(ClassificationItemsContext);
  const uiLang = useGetUiLanguage();
  const classes = useStyles();
  const sortType: SortType = classification === cService.serviceClasses ? null : 'default';
  const sortedItems = useSortClassificationItems(item.children, sortType);

  const defaultOpen = !!searchValue;
  const [open, setOpen] = useState(defaultOpen);

  useEffect(() => {
    setOpen(!!searchValue);
  }, [searchValue]);

  const handleOnOpenChange = (open: boolean) => {
    setOpen(!open);
  };

  const label = translateToLang(uiLang, names) ?? '';
  const mainCategoryClass = clsx(classes.mainCategory, { isOpen: open });
  return (
    <Expander key={id} open={open} onOpenChange={handleOnOpenChange} className={mainCategoryClass}>
      <ExpanderTitleButton asHeading='h3'>
        <NodeLabel label={label} count={count} />
      </ExpanderTitleButton>
      {open && (
        <ExpanderContent>
          {sortedItems.map((item) => (
            <TreeNodeSub key={item.id} item={item} />
          ))}
        </ExpanderContent>
      )}
    </Expander>
  );
};
