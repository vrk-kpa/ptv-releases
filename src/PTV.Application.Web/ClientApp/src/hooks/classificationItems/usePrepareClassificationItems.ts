import { useContext } from 'react';
import { ClassificationItem, ClassificationItemParsed } from 'types/classificationItemsTypes';
import { Language } from 'types/enumTypes';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';

export const usePrepareClassificationItems = (items: ClassificationItem[]): ClassificationItem[] => {
  const { searchValue } = useContext(ClassificationItemsContext);
  const uiLanguage = useGetUiLanguage();
  const filteredItems = (searchValue && filterQueryResult(items, searchValue, uiLanguage)) || items;
  return getItemsWithLeafCount(filteredItems);
};

const filterQueryResult = (items: ClassificationItem[], searchValue: string, language: Language) => {
  const getChildren = (result: ClassificationItem[], object: ClassificationItem) => {
    if (!object.children && object.names[language]?.toLowerCase().indexOf(searchValue.toLowerCase()) !== -1) {
      result.push(object);
      return result;
    }
    if (Array.isArray(object.children)) {
      const children = object.children.reduce(getChildren, []);
      if (children.length) result.push({ ...object, children });
    }
    return result;
  };

  return items.reduce(getChildren, []);
};

const getItemsWithLeafCount = (items: ClassificationItem[]): ClassificationItemParsed[] => {
  const getChildren = (result: ClassificationItemParsed[], object: ClassificationItem) => {
    if (!object.children) {
      result.push({ ...object, count: 1 });
      return result;
    }
    if (Array.isArray(object.children)) {
      const children = object.children.reduce(getChildren, []);
      const count = children.reduce((acc, curr: ClassificationItemParsed) => {
        return acc + (curr.count || 0);
      }, 0);
      if (children.length) result.push({ ...object, children, count });
    }
    return result;
  };

  return items.reduce(getChildren, []);
};
