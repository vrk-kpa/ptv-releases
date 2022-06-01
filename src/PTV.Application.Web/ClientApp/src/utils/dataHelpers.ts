import { ClassificationItem } from 'types/classificationItemsTypes';
import { ClassificationItemTypes } from 'types/enumTypes';
import { cService } from 'types/forms/serviceFormTypes';
import { LocalizedText } from 'types/miscellaneousTypes';

type ClassificationItemTree = {
  [key: string]: ClassificationItem;
};

export type TreeSettings = {
  mainCategoryNames: LocalizedText;
  subCategoryNames: LocalizedText;
};

export const createTree = (
  classification: ClassificationItemTypes,
  data: ClassificationItem[],
  settings: TreeSettings
): ClassificationItem[] => {
  switch (classification) {
    case cService.serviceClasses:
      return createServiceClassesTree(data, null, settings);
    case cService.industrialClasses:
      return createIndustrialClassesTree(data, null);
    default:
      return [];
  }
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const createIndustrialClassesTree = (data: ClassificationItem[], root: any): ClassificationItem[] => {
  const t: ClassificationItemTree = {};

  data.forEach((item: ClassificationItem) => {
    Object.assign((t[item.id] = t[item.id] || {}), item);
    const parentId = item.parentId ? item.parentId : root;
    t[parentId] = t[parentId] || {};
    t[parentId].children = t[parentId].children || [];
    t[parentId].children.push(t[item.id]);
  });

  return t[root].children;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const createServiceClassesTree = (data: ClassificationItem[], root: any, settings: TreeSettings): ClassificationItem[] => {
  const t: ClassificationItemTree = {};

  data.forEach((item: ClassificationItem) => {
    Object.assign((t[item.id] = t[item.id] || {}), item);
    const parentId = item.parentId ? item.parentId : root;
    t[parentId] = t[parentId] || {};
    if (parentId) {
      t[parentId].children = t[parentId].children || [];
      t[parentId].children[1] = t[parentId].children[1] || {
        id: `${parentId}_sub`,
        code: `${parentId}_sub`,
        name: 'sub',
        names: settings.subCategoryNames,
        children: null,
      };
      t[parentId].children[1].children = t[parentId].children[1].children || [];
      t[parentId].children[1].children.push(t[item.id]);
    }
    if (!parentId) {
      t[item.id].children = t[item.id].children || [];
      t[item.id].children[0] = t[item.id].children[0] || {
        id: `${item.id}_main`,
        code: `${item.id}_main`,
        name: 'main',
        names: settings.mainCategoryNames,
        children: null,
      };
      t[item.id].children[0].children = t[item.id].children[0].children || [];
      t[item.id].children[0].children.push(item);
      t[parentId].children = t[parentId].children || [];
      t[parentId].children.push(t[item.id]);
    }
  });

  return t[root].children;
};

export const getDiffByPropValue = <T extends Record<string, unknown>>(arrX: T[], arrY: T[], prop: string): T[] =>
  arrX.filter((x) => {
    return !arrY.some((y) => {
      return x[prop] === y[prop];
    });
  });
