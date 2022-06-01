import { EnumItemType } from './enumItemType';
import { LocalizedText } from './miscellaneousTypes';

export type TargetGroup = EnumItemType & {
  id: string;
  parentId: string | null;
  code: string;
  name: string;
  names: LocalizedText;
  descriptions: LocalizedText;
};
