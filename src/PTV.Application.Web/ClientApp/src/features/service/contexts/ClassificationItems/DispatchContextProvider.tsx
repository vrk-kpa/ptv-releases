import { createContext } from 'react';
import { ClassificationItemMode } from 'types/classificationItemsTypes';

export type Action =
  | { type: 'ADD_ITEM'; payload: string }
  | { type: 'REMOVE_ITEM'; payload: string }
  | { type: 'SET_ITEMS'; payload: string[] }
  | { type: 'ADD_OTHER_ITEM'; payload: string }
  | { type: 'REMOVE_OTHER_ITEM'; payload: string }
  | { type: 'SET_OTHER_ITEMS'; payload: string[] }
  | { type: 'UPDATE_QUERY'; payload: string }
  | { type: 'TOGGLE_MODE'; payload: ClassificationItemMode };

export type Dispatch = (action: Action) => void;

export const DispatchContext = createContext<Dispatch>(() => undefined);
