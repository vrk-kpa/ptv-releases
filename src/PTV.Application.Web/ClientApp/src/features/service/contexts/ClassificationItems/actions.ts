import { ClassificationItemMode } from 'types/classificationItemsTypes';
import { Dispatch } from './DispatchContextProvider';

export function addItem(dispatch: Dispatch, item: string): void {
  dispatch({
    type: 'ADD_ITEM',
    payload: item,
  });
}

export function removeItem(dispatch: Dispatch, item: string): void {
  dispatch({
    type: 'REMOVE_ITEM',
    payload: item,
  });
}

export function setItems(dispatch: Dispatch, items: string[]): void {
  dispatch({
    type: 'SET_ITEMS',
    payload: items,
  });
}

export function addOtherItem(dispatch: Dispatch, item: string): void {
  dispatch({
    type: 'ADD_OTHER_ITEM',
    payload: item,
  });
}

export function removeOtherItem(dispatch: Dispatch, item: string): void {
  dispatch({
    type: 'REMOVE_OTHER_ITEM',
    payload: item,
  });
}

export function setOtherItems(dispatch: Dispatch, items: string[]): void {
  dispatch({
    type: 'SET_OTHER_ITEMS',
    payload: items,
  });
}

export function updateQuery(dispatch: Dispatch, query: string): void {
  dispatch({
    type: 'UPDATE_QUERY',
    payload: query,
  });
}

export function toggleMode(dispatch: Dispatch, mode: ClassificationItemMode): void {
  dispatch({
    type: 'TOGGLE_MODE',
    payload: mode,
  });
}
