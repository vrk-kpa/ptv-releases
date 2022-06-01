import React, { FunctionComponent, createContext } from 'react';
import { castDraft } from 'immer';
import { useImmerReducer } from 'use-immer';
import { ClassificationItemMode } from 'types/classificationItemsTypes';
import { ClassificationItemTypes } from 'types/enumTypes';
import { cService } from 'types/forms/serviceFormTypes';
import { useMainCategoryFilter } from 'hooks/classificationItems/useMainCategoryFilter';
import { DispatchContext } from './DispatchContextProvider';
import { classificationItemsContextReducer } from './reducers';

/**
 * Industrial classes context
 */
export interface ClassificationItemsContextInterface {
  selectedItems: string[];
  otherItems: string[];
  gdItems: string[];
  gdOtherItems?: string[];
  elementMode: ClassificationItemMode;
  searchValue: string;
  classification: ClassificationItemTypes;
  mainCategoryIds?: string[];
  namespace: string;
}

/**
 * This is the state before we have actually received anything
 * i.e. compare to redux store that is defined in code
 */
export const initialState: ClassificationItemsContextInterface = {
  selectedItems: [],
  otherItems: [],
  gdItems: [],
  gdOtherItems: [],
  elementMode: 'display',
  searchValue: '',
  classification: cService.serviceClasses,
  namespace: '',
};

export const ClassificationItemsContext = createContext<ClassificationItemsContextInterface>(initialState);

// eslint-disable-next-line @typescript-eslint/explicit-module-boundary-types
export const useClassificationItemsContextReducer = (initialState: ClassificationItemsContextInterface) => {
  return useImmerReducer(classificationItemsContextReducer, initialState);
};

type ClassificationItemsContextProviderProps = {
  classification: ClassificationItemTypes;
  namespace: string;
  gdItems: string[];
  gdOtherItems?: string[];
  selectedItems: string[];
};

export const ClassificationItemsContextProvider: FunctionComponent<ClassificationItemsContextProviderProps> = ({
  classification,
  namespace,
  gdItems,
  gdOtherItems,
  children,
  selectedItems,
}) => {
  const mainCategoryIds = useMainCategoryFilter(classification);

  const [state, dispatch] = useClassificationItemsContextReducer({
    ...initialState,
    classification,
    namespace,
    selectedItems,
  });

  const value = castDraft({ ...state, mainCategoryIds, gdItems, gdOtherItems });

  return (
    <ClassificationItemsContext.Provider value={value}>
      <DispatchContext.Provider value={dispatch}>{children}</DispatchContext.Provider>
    </ClassificationItemsContext.Provider>
  );
};
