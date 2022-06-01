import { PublishingStatus } from 'types/enumTypes';

export type SimpleLanguageVersionType = {
  name: string;
  status: PublishingStatus;
  isScheduled: boolean;
};
