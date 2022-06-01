import { Draft } from 'immer';
import { IAppContext } from 'context/AppContextProvider';
import { Action } from 'context/DispatchContextProvider';
import { assertUnreachable } from 'utils/reducer';

export function appContextReducer(draft: Draft<IAppContext>, action: Action): void {
  switch (action.type) {
    case 'Logout':
      draft.userInfo = null;
      draft.organizationRoles = [];
      draft.userOrganizations = [];
      break;
    case 'SettingsUpdated':
      draft.settings = action.payload;
      break;
    case 'StaticDataLoaded':
      draft.staticData = action.payload;
      break;
    case 'UserOrgAndRolesLoaded':
      draft.userOrganizations = action.payload.userOrganizations;
      draft.organizationRoles = action.payload.organizationRoles;
      break;
    case 'UserInfoLoaded':
      draft.userInfo = action.payload;
      break;
    default:
      assertUnreachable(action as never);
  }
}
