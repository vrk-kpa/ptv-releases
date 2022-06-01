import React, { createContext } from 'react';
import { useImmerReducer } from 'use-immer';
import { Language, Mode } from 'types/enumTypes';
import { ProblemDetails } from 'types/miscellaneousTypes';
import { NotificationStatuses, initialNotificationStatuses } from 'types/notificationStatus';
import { Action, DispatchContext } from './DispatchFormMetaContext';
import { formMetaContextReducer } from './reducers';

export interface IFormMetaContext {
  selectedLanguageCode: Language;
  compareLanguageCode: Language | undefined;
  mode: Mode;
  displayComparison: boolean;
  availableLanguages: Language[];
  serverError: ProblemDetails | undefined;
  notificationStatuses: NotificationStatuses;
}

export function createInitialState(mode: Mode = 'view', language: Language = 'fi'): IFormMetaContext {
  return {
    selectedLanguageCode: language,
    compareLanguageCode: undefined,
    mode: mode,
    displayComparison: false,
    availableLanguages: [],
    serverError: undefined,
    notificationStatuses: initialNotificationStatuses,
  };
}

export const FormMetaContext = createContext<IFormMetaContext | null>(null);

export const useFormMetaContextReducer = (initialState: IFormMetaContext): [IFormMetaContext, React.Dispatch<Action>] =>
  useImmerReducer(formMetaContextReducer, initialState);

type FormMetaContextProviderProps = {
  children: React.ReactNode;
  initialState: IFormMetaContext;
};

export function FormMetaContextProvider(props: FormMetaContextProviderProps): React.ReactElement {
  const [state, dispatch] = useFormMetaContextReducer(props.initialState);
  return (
    <FormMetaContext.Provider value={state}>
      <DispatchContext.Provider value={dispatch}>{props.children}</DispatchContext.Provider>
    </FormMetaContext.Provider>
  );
}
