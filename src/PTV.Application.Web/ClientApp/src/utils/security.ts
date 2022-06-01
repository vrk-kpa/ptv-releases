import { PermissionEnum, PermissionGroup } from 'types/enumTypes';
import { OrganizationRoleModel } from 'types/organizationTypes';
import { UserInfo } from 'types/userInfoTypes';

export function isPtvAdmin(userInfo: UserInfo | null): boolean {
  return userInfo?.role === 'Eeva';
}

export function canAccess(userInfo: UserInfo | null): boolean {
  return userInfo ? userInfo.hasAccess : false;
}

export function canDelete(
  userInfo: UserInfo,
  permissionGroup: PermissionGroup,
  targetOrgId: string | null | undefined,
  organizationRoles: OrganizationRoleModel[]
): boolean {
  if (hasAccessToTargetOrganization(userInfo, organizationRoles, targetOrgId)) {
    return canDeleteOwn(userInfo, permissionGroup);
  }

  return canDeleteAll(userInfo, permissionGroup);
}

export function canDeleteOwn(userInfo: UserInfo, permissionGroup: PermissionGroup): boolean {
  return hasOwnPermission(userInfo, permissionGroup, PermissionEnum.Delete);
}

export function canDeleteAll(userInfo: UserInfo, permissionGroup: PermissionGroup): boolean {
  return hasAllPermission(userInfo, permissionGroup, PermissionEnum.Delete);
}

export function canCreate(
  userInfo: UserInfo,
  permissionGroup: PermissionGroup,
  targetOrgId: string | null | undefined,
  organizationRoles: OrganizationRoleModel[]
): boolean {
  if (hasAccessToTargetOrganization(userInfo, organizationRoles, targetOrgId)) {
    return canCreateOwn(userInfo, permissionGroup);
  }

  return canCreateAll(userInfo, permissionGroup);
}

export function canCreateOwn(userInfo: UserInfo, permissionGroup: PermissionGroup): boolean {
  return hasOwnPermission(userInfo, permissionGroup, PermissionEnum.Create);
}

export function canCreateAll(userInfo: UserInfo, permissionGroup: PermissionGroup): boolean {
  return hasOwnPermission(userInfo, permissionGroup, PermissionEnum.Create);
}

export function canPublish(
  userInfo: UserInfo,
  permissionGroup: PermissionGroup,
  targetOrgId: string | null | undefined,
  organizationRoles: OrganizationRoleModel[]
): boolean {
  if (hasAccessToTargetOrganization(userInfo, organizationRoles, targetOrgId)) {
    return canPublishOwn(userInfo, permissionGroup);
  }

  return canPublishAll(userInfo, permissionGroup);
}

function canPublishOwn(userInfo: UserInfo, permissionGroup: PermissionGroup): boolean {
  return hasOwnPermission(userInfo, permissionGroup, PermissionEnum.Publish);
}

function canPublishAll(userInfo: UserInfo, permissionGroup: PermissionGroup): boolean {
  return hasAllPermission(userInfo, permissionGroup, PermissionEnum.Publish);
}

export function canUpdate(
  userInfo: UserInfo,
  permissionGroup: PermissionGroup,
  targetOrgId: string | null | undefined,
  organizationRoles: OrganizationRoleModel[]
): boolean {
  if (hasAccessToTargetOrganization(userInfo, organizationRoles, targetOrgId)) {
    return canUpdateOwn(userInfo, permissionGroup);
  }

  return canUpdateAll(userInfo, permissionGroup);
}

function canUpdateOwn(userInfo: UserInfo, permissionGroup: PermissionGroup): boolean {
  return hasOwnPermission(userInfo, permissionGroup, PermissionEnum.Update);
}

function canUpdateAll(userInfo: UserInfo, permissionGroup: PermissionGroup): boolean {
  return hasAllPermission(userInfo, permissionGroup, PermissionEnum.Update);
}

function hasOwnPermission(userInfo: UserInfo, permissionGroup: PermissionGroup, wanted: PermissionEnum): boolean {
  const permission = userInfo.permisions[permissionGroup];
  if (!permission) {
    return false;
  }

  return has(permission.rulesOwn, wanted);
}

function hasAllPermission(userInfo: UserInfo, permissionGroup: PermissionGroup, wanted: PermissionEnum): boolean {
  const permission = userInfo.permisions[permissionGroup];
  if (!permission) {
    return false;
  }

  return has(permission.rulesAll, wanted);
}

function has(flags: PermissionEnum, wanted: PermissionEnum) {
  return !!(flags & wanted);
}

function hasAccessToTargetOrganization(
  userInfo: UserInfo,
  organizationRoles: OrganizationRoleModel[],
  targetOrgId: string | null | undefined
): boolean {
  if (!targetOrgId) return false;
  if (userInfo.userOrganization === targetOrgId) return true;
  return organizationRoles.some((x) => x.organizationId === targetOrgId);
}
