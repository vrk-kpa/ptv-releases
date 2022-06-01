import React, { createContext, useContext } from 'react';
import { castDraft } from 'immer';
import { useImmerReducer } from 'use-immer';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { ValidationModel } from 'types/api/validationModel';
import { DispatchContext } from './DispatchContextProvider';
import { serviceContextReducer } from './reducers';

/**
 * Service level context
 */
export interface IServiceContext {
  validationResult: ValidationModel<ServiceApiModel> | null;
}

/**
 * This is the state before we have actually received anything
 * i.e. compare to redux store that is defined in code
 */
export const initialState: IServiceContext = {
  validationResult: null,
};

export const ServiceContext = createContext<IServiceContext>(initialState);

// eslint-disable-next-line @typescript-eslint/explicit-module-boundary-types
export const useServiceContextReducer = (initialState: IServiceContext) => useImmerReducer(serviceContextReducer, initialState);

export function useServiceContext(): IServiceContext {
  const context = useContext(ServiceContext);
  if (context === undefined) {
    throw new Error('Service context must be used with service context provider');
  }
  return context;
}

type ServiceContextProviderProps = {
  children: React.ReactNode;
};

export function ServiceContextProvider(props: ServiceContextProviderProps): React.ReactElement {
  const [state, dispatch] = useServiceContextReducer(initialState);
  const value = castDraft(state);
  return (
    <ServiceContext.Provider value={value}>
      <DispatchContext.Provider value={dispatch}>{props.children}</DispatchContext.Provider>
    </ServiceContext.Provider>
  );
}
