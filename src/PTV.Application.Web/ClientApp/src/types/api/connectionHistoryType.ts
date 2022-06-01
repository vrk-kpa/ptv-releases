import { MainEntityType, SubEntityType } from 'types/enumTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { SimpleLanguageVersionType } from './simpleLanguageVersionType';

export type ConnectionOperationType = 'Detached' | 'Unchanged' | 'Deleted' | 'Modified' | 'Added';

export type ConnectionHistoryType = {
  editor: string;
  editedAt: string;
  languageVersions: LanguageVersionType<SimpleLanguageVersionType>;
  operationType: ConnectionOperationType;
  operationId: string;
  entityType: MainEntityType;
  subEntityType: SubEntityType;
  id: string;
};
