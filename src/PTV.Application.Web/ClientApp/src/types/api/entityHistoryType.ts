import { Language } from 'types/enumTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { LocalizedText } from 'types/miscellaneousTypes';
import { SimpleLanguageVersionType } from './simpleLanguageVersionType';

export type HistoryActionType =
  | 'Save'
  | 'Delete'
  | 'Publish'
  | 'Restore'
  | 'Withdraw'
  | 'TranslationOrdered'
  | 'TranslationReceived'
  | 'Copy'
  | 'MassPublish'
  | 'ScheduledPublish'
  | 'ScheduledArchive'
  | 'MassRestore'
  | 'TranslationReordered'
  | 'OldPublished'
  | 'Expired'
  | 'MassArchive'
  | 'ArchivedViaOrganization'
  | 'ArchivedViaScheduling';

type CopyDetailsType = {
  templateId: string;
  templateOrganizationId: string;
  templateOrganizationNames: LocalizedText;
};

export type EntityHistoryType = {
  editor: string;
  editedAt: string;
  id: string;
  entityType: string;
  subEntityType: string;
  historyAction: HistoryActionType;
  operationId: string;
  languageVersions: LanguageVersionType<SimpleLanguageVersionType>;
  version: string;
  copyInfo?: CopyDetailsType | null | undefined;
  nextVersion: string;
  sourceLanguage?: Language | null | undefined;
  targetLanguages: Language[];
  showLink: boolean;
};
