import { ClassificationItem } from 'types/classificationItemsTypes';
import { SortType } from 'types/miscellaneousTypes';
import { useGetUiLanguage } from './useGetUiLanguage';

export const useSortClassificationItems = (items: ClassificationItem[], sortType: SortType = 'default'): ClassificationItem[] => {
  const uiLanguage = useGetUiLanguage();

  if (!sortType) {
    return items;
  }

  return items.sort((a, b): number => {
    const nameA = a.names[uiLanguage] || '';
    const nameB = b.names[uiLanguage] || '';
    return nameA.localeCompare(nameB, uiLanguage);
  });
};
