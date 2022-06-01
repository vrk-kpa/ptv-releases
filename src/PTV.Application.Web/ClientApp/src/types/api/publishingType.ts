import { PublishingStatus } from 'types/enumTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';

// Should be part of an object LanguageVersionType<PublishingModel>
export type PublishingType = {
  publishAt?: string | null | undefined; // empty for immediate publishing, Date for scheduled publishing
  archiveAt?: string | null | undefined; // empty for default archiving date, Date for scheduled archiving
  status: PublishingStatus; // publishing status of the language version
};

export type PublishingCommandModel = LanguageVersionType<PublishingType>;
