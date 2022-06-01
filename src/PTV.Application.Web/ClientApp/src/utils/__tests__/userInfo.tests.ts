import { OrganizationModel } from 'types/organizationTypes';
import { IAppContext } from 'context/AppContextProvider';
import { getUserOrganization } from 'utils/userInfo';

function createOrganization(id: string): OrganizationModel {
  return {
    id: id,
    name: 'anything',
    alternateName: null,
    code: null,
    publishingStatus: 'Published',
    type: null,
    texts: {
      en: 'anything',
    },
  };
}

function createContext(userOrgId: string): IAppContext {
  return {
    userInfo: {
      userOrganization: userOrgId,
    },
    userOrganizations: [createOrganization('1'), createOrganization('2')],
  };
}

describe('getUserOrganization', () => {
  it('returns organization user has currently logged in as', () => {
    const org = getUserOrganization(createContext('1'));
    expect(org.id).toBe('1');
  });

  it('returns undefined if organization is not found', () => {
    const org = getUserOrganization(createContext('notfound'));
    expect(org).toBeUndefined();
  });
});
