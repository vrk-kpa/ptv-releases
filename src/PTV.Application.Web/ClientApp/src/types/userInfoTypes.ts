import { PermissionEnum, PermissionGroup, UserRole } from './enumTypes';

export type UserInfo = {
  email: string;
  name: string;
  surname: string;
  role: UserRole;
  userOrganization: string;
  hasAccess: boolean;
  permisions: { [group in PermissionGroup]?: Permission };
};

export type Permission = {
  code: PermissionGroup;
  rulesAll: PermissionEnum;
  rulesOwn: PermissionEnum;
};
