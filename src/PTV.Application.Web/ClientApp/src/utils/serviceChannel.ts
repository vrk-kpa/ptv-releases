import {
  ConnectableChannelLanguageVersion,
  ServiceChannelConnectionLanguageVersion,
  ServiceChannelLanguageVersion,
} from 'types/api/serviceChannelModel';
import { Language, PublishingStatus } from 'types/enumTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { LanguagePriorities } from './languages';

type ChannelLanguageVersions = LanguageVersionType<ServiceChannelLanguageVersion>;
type ChannelSelector<T> = (lv: ServiceChannelLanguageVersion) => T | null | undefined;

export function getChannelValue<T>(
  lvs: ChannelLanguageVersions,
  wantedLanguage: Language,
  selector: ChannelSelector<T>,
  fallBackValue: T
): T {
  const languages = [wantedLanguage].concat(LanguagePriorities);
  for (const lang of languages) {
    const lv = lvs[lang];
    if (lv) {
      const value = selector(lv);
      if (value) {
        return value;
      }
    }
  }

  return fallBackValue;
}

type ConnectableChannelLanguageVersions = LanguageVersionType<ConnectableChannelLanguageVersion>;
type ConnectableChannelSelector<T> = (lv: ConnectableChannelLanguageVersion) => T | null | undefined;

export function getConnectableChannelValue<T>(
  lvs: ConnectableChannelLanguageVersions,
  wantedLanguage: Language,
  selector: ConnectableChannelSelector<T>,
  fallBackValue: T
): T {
  const languages = [wantedLanguage].concat(LanguagePriorities);
  for (const lang of languages) {
    const lv = lvs[lang];
    if (lv) {
      const value = selector(lv);
      if (value) {
        return value;
      }
    }
  }

  return fallBackValue;
}

type ServiceChannelConnectionLanguageVersions = LanguageVersionType<ServiceChannelConnectionLanguageVersion>;
type ServiceChannelConnectionLanguageVersionSelector<T> = (lv: ServiceChannelConnectionLanguageVersion) => T | null | undefined;

export function getServiceChannelConnectionValue<T>(
  lvs: ServiceChannelConnectionLanguageVersions,
  wantedLanguage: Language,
  selector: ServiceChannelConnectionLanguageVersionSelector<T>,
  fallBackValue: T
): T {
  const languages = [wantedLanguage].concat(LanguagePriorities);
  for (const lang of languages) {
    const lv = lvs[lang];
    if (lv) {
      const value = selector(lv);
      if (value) {
        return value;
      }
    }
  }

  return fallBackValue;
}

export function canModifyConnections(serviceId: string | null | undefined, serviceStatus: PublishingStatus): boolean {
  // Service must have been saved at least once
  if (!serviceId) {
    return false;
  }

  if (serviceStatus === 'Deleted' || serviceStatus === 'OldPublished' || serviceStatus === 'Removed') {
    return false;
  }

  return true;
}
