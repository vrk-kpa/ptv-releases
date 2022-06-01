import { createContext } from 'react';
import { UserOrganizationsAndRolesResponse } from 'types/organizationTypes';
import { AppSettings, StaticData } from 'types/settingTypes';
import { UserInfo } from 'types/userInfoTypes';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type Payload = any | null;

export type Action =
  | { type: 'Logout' }
  | { type: 'SettingsUpdated'; payload: AppSettings }
  | { type: 'StaticDataLoaded'; payload: StaticData }
  | { type: 'UserOrgAndRolesLoaded'; payload: UserOrganizationsAndRolesResponse }
  | { type: 'UserInfoLoaded'; payload: UserInfo };

export type Dispatch = (action: Action) => void;

// eslint-disable-next-line @typescript-eslint/no-empty-function
export const DispatchContext = createContext<Dispatch>(() => {});
