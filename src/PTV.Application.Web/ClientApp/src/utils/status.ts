import { Language, PublishingStatus } from 'types/enumTypes';
import { ServiceModelLangaugeVersionsValuesType } from 'types/forms/serviceFormTypes';
import { getEnabledLanguages } from './service';

export function isArchived(status: PublishingStatus): boolean {
  return status === 'Deleted' || status === 'OldPublished';
}

export function isArchivedOrRemoved(status: PublishingStatus): boolean {
  return status === 'Deleted' || status === 'OldPublished' || status === 'Removed';
}

export function getLVStatus(lVStatus: PublishingStatus, entityStatus: PublishingStatus): PublishingStatus {
  return isArchived(entityStatus) ? entityStatus : lVStatus;
}

export function areOtherVersionsArchived(language: Language, lvs: ServiceModelLangaugeVersionsValuesType): boolean {
  const otherLanguages = getEnabledLanguages(lvs).filter((l) => l !== language);
  for (const lang of otherLanguages) {
    if (!isArchived(lvs[lang].status)) {
      return false;
    }
  }
  return true;
}

export function areAllVersionsArchived(lvs: ServiceModelLangaugeVersionsValuesType): boolean {
  const languages = getEnabledLanguages(lvs);
  for (const lang of languages) {
    if (!isArchived(lvs[lang].status)) return false;
  }

  return true;
}
