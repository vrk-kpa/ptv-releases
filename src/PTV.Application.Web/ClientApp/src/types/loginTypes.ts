import { TranslatedText } from './miscellaneousTypes';
import { OrganizationModel } from './organizationTypes';
import { UserInfo } from './userInfoTypes';

export type LoginModel = {
  name: string;
  password: string;
  userAccessRightsGroup: string | null;
  organization: string | null;
};

export type FakeLoginModel = {
  name: string;
  password: string;
  userAccessRightsGroup: string | null;
  organization: OrganizationModel | null;
};

export type LoginResponseModel = {
  data: {
    token: string;
    userInfo: UserInfo | null;
  };
};

export type UserAccessRightsGroupData = {
  UserAccessRightsGroups: UserAccessRightsGroup[];
};

export type UserAccessRightsGroup = {
  description: string | null;
  id: string;
  name: string;
  code: string;
  translation: TranslatedText;
};
