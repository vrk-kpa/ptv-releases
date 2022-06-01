import _ from 'lodash';

export function getNonEmptyKeys(object: Record<string, unknown>): string[] {
  return Object.keys(_.omitBy(object, _.isNil));
}

export function hasNonEmptyKeys(object: Record<string, unknown>): boolean {
  return getNonEmptyKeys(object).length !== 0;
}
