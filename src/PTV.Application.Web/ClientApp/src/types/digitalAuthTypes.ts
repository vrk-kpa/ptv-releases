import { EnumItemType } from './enumItemType';

export type DigitalAuthorizationModel = EnumItemType & {
  children: DigitalAuthorizationModel[];
  isValid: boolean;
  parentId?: string;
};
