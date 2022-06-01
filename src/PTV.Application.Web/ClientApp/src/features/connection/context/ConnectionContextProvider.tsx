import React, { createContext } from 'react';
import { useImmerReducer } from 'use-immer';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { connectionContextReducer } from 'features/connection/reducers';
import { ConnectionDispatchContext } from './ConnectionDispatchContextProvider';

export interface IConnectionContext {
  connection: ConnectionFormModel;
}

export const ConnectionContext = createContext<IConnectionContext | null>(null);

export const useConnectionReducer = (initialState: IConnectionContext): [IConnectionContext, React.Dispatch<unknown>] =>
  useImmerReducer(connectionContextReducer, initialState);

type ConnectionContextProviderProps = {
  children: React.ReactNode;
  connection: ConnectionFormModel;
};

export function ConnectionContextProvider(props: ConnectionContextProviderProps): React.ReactElement {
  const [state, dispatch] = useConnectionReducer({
    connection: props.connection,
  });

  return (
    <ConnectionContext.Provider value={state}>
      <ConnectionDispatchContext.Provider value={dispatch}>{props.children}</ConnectionDispatchContext.Provider>
    </ConnectionContext.Provider>
  );
}
