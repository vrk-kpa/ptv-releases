import { DigitalAuthorizationModel } from 'types/digitalAuthTypes';

export function getTopLevelGroupsThatContain(root: DigitalAuthorizationModel[], authorizationIds: string[]): DigitalAuthorizationModel[] {
  return getTopLevelGroups(root).filter((x) => x.children.some((c) => authorizationIds.some((o) => o === c.id)));
}

export function getTopLevelGroups(root: DigitalAuthorizationModel[]): DigitalAuthorizationModel[] {
  // Server side returns groups as list but there is only single node
  // at the root level and we want to get groups that are below that.
  if (root.length !== 1) {
    throw new Error(`Cannot get authorization groups.`);
  }
  return root[0].children;
}
