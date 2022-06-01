import { Language, PublishingStatus } from 'types/enumTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { TranslationAvailabilityType } from 'types/miscellaneousTypes';

export type LanguageVersionBaseModel = {
  language: Language;
  modified: string;
  modifiedBy: string;
  name: string;
  status: PublishingStatus;
  translationAvailability: TranslationAvailabilityType | null | undefined;
};

export type EntityBaseModel = {
  languageVersions: LanguageVersionType<LanguageVersionBaseModel>;
  id: string | null | undefined;
};
