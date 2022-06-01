import { union } from 'lodash';
import { cService } from 'types/forms/serviceFormTypes';
import { MaxServiceClasses } from 'features/service/validation/serviceClasses';
import { ClassificationItemsContextInterface } from './ClassificationItemsContextProvider';

export function getAreOnlyMainClassesSelected(context: ClassificationItemsContextInterface): boolean {
  const mainCategoryIds = context.mainCategoryIds;
  if (!mainCategoryIds) {
    return false;
  }

  const allItems = getSelectedItemsUnion(context);
  return allItems.length > 0 && allItems.every((id) => mainCategoryIds.includes(id));
}

export function getAreMainClassesDisabled(context: ClassificationItemsContextInterface): boolean {
  const mainCategoryIds = context.mainCategoryIds;
  if (!mainCategoryIds) {
    return false;
  }

  const allItems = getSelectedItemsUnion(context);
  return allItems.length === 3 && allItems.every((id) => mainCategoryIds.includes(id));
}

export function getIsLimitReached(context: ClassificationItemsContextInterface): boolean {
  const classification = context.classification;
  if (classification !== cService.serviceClasses && classification !== cService.ontologyTerms) {
    return false;
  }

  const limit = {
    [cService.serviceClasses]: MaxServiceClasses,
    [cService.ontologyTerms]: 10,
  }[classification];

  const allItems = getSelectedItemsUnion(context);
  return allItems.length >= limit;
}

export function getSelectedItemsUnion(context: ClassificationItemsContextInterface): string[] {
  return union(context.selectedItems, context.gdItems).concat(union(context.otherItems, context.gdOtherItems));
}
