import { Draft } from 'immer';
import { assertUnreachable } from 'utils/reducer';
import { ClassificationItemsContextInterface } from './ClassificationItemsContextProvider';
import { Action } from './DispatchContextProvider';

export function classificationItemsContextReducer(draft: Draft<ClassificationItemsContextInterface>, action: Action): void {
  switch (action.type) {
    case 'ADD_ITEM':
      addItem(draft, action.payload);
      break;
    case 'REMOVE_ITEM':
      removeItem(draft, action.payload);
      break;
    case 'SET_ITEMS':
      setItems(draft, action.payload);
      break;
    case 'ADD_OTHER_ITEM':
      addOtherItem(draft, action.payload);
      break;
    case 'REMOVE_OTHER_ITEM':
      removeOtherItem(draft, action.payload);
      break;
    case 'SET_OTHER_ITEMS':
      setOtherItems(draft, action.payload);
      break;
    case 'UPDATE_QUERY':
      draft.searchValue = action.payload;
      break;
    case 'TOGGLE_MODE':
      draft.elementMode = action.payload;
      break;
    default:
      assertUnreachable(action);
  }
}

function addItem(draft: Draft<ClassificationItemsContextInterface>, item: string) {
  draft.selectedItems.push(item);
}

function removeItem(draft: Draft<ClassificationItemsContextInterface>, item: string) {
  draft.selectedItems = draft.selectedItems.filter((x) => x !== item);
}

function setItems(draft: Draft<ClassificationItemsContextInterface>, items: string[]) {
  draft.selectedItems = items;
}

function addOtherItem(draft: Draft<ClassificationItemsContextInterface>, item: string) {
  draft.otherItems.push(item);
}

function removeOtherItem(draft: Draft<ClassificationItemsContextInterface>, item: string) {
  draft.otherItems = draft.otherItems.filter((x) => x !== item);
}

function setOtherItems(draft: Draft<ClassificationItemsContextInterface>, items: string[]) {
  draft.otherItems = items;
}
