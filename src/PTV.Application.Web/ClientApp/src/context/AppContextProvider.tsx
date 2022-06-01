import React, { createContext } from 'react';
import { appContextReducer } from 'reducers';
import { useImmerReducer } from 'use-immer';
import { OrganizationModel, OrganizationRoleModel } from 'types/organizationTypes';
import { AppSettings, StaticData } from 'types/settingTypes';
import { UserInfo } from 'types/userInfoTypes';
import { DispatchContext } from './DispatchContextProvider';

/**
 * Application level context
 */
export interface IAppContext {
  settings: AppSettings | null;
  userInfo: UserInfo | null;
  staticData: StaticData;
  userOrganizations: OrganizationModel[];
  organizationRoles: OrganizationRoleModel[];
}

export const initialState: IAppContext = {
  settings: null,
  userInfo: null,
  staticData: {
    targetGroups: [],
    serviceClasses: [],
    ontologyTerms: [],
    lifeEvents: [],
    industrialClasses: [],
    municipalities: [],
    businessRegions: [],
    hospitalRegions: [],
    provinces: [],
    languages: [],
    dialCodes: [],
    countries: [],
    digitalAuthorizations: [],
  },
  userOrganizations: [],
  organizationRoles: [],
};

export const AppContext = createContext<IAppContext>(initialState);

// eslint-disable-next-line @typescript-eslint/explicit-module-boundary-types
export const useAppContextReducer = (initialState: IAppContext) => useImmerReducer(appContextReducer, initialState);

type AppContextProviderProps = {
  children: React.ReactNode;
};

export function AppContextProvider(props: AppContextProviderProps): React.ReactElement {
  const [state, dispatch] = useAppContextReducer(initialState);

  return (
    <AppContext.Provider value={state}>
      <DispatchContext.Provider value={dispatch}>{props.children}</DispatchContext.Provider>
    </AppContext.Provider>
  );
}
