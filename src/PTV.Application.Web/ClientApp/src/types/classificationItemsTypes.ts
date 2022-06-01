import { EnumItemType } from './enumItemType';
import { LocalizedText } from './miscellaneousTypes';

export interface ClassificationItemParsed extends ClassificationItem {
  count?: number;
}

export type ClassificationItem = EnumItemType & {
  children: ClassificationItem[];
  code: string;
  descriptions: LocalizedText;
  id: string;
  name?: string;
  names: LocalizedText;
  parentId?: string;
};

export type ClassificationItemMode = 'select' | 'display' | 'summary';

export type OntologyTermTreeType = ClassificationItem & {
  parents: ClassificationItem[];
  children: ClassificationItem[];
};

export type OntologyTerm = {
  id: string;
  code: string;
  names: LocalizedText;
};
