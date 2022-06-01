import { Draft } from 'immer';
import { Language, Mode } from 'types/enumTypes';
import { LanguageOptions, ModeWithLanguage, ProblemDetails } from 'types/miscellaneousTypes';
import { initialNotificationStatuses } from 'types/notificationStatus';
import { assertUnreachable } from 'utils/reducer';
import { Action } from './DispatchFormMetaContext';
import { IFormMetaContext } from './FormMetaContext';

export function formMetaContextReducer(draft: Draft<IFormMetaContext>, action: Action): void {
  switch (action.type) {
    case 'SwitchFormMode':
      switchFormMode(draft, action.payload);
      break;
    case 'SwitchFormModeAndSelectLanguage':
      switchFormModeAndSelectLanguage(draft, action.payload);
      break;
    case 'SelectFirstLanguage':
      selectFirstLanguage(draft, action.payload);
      break;
    case 'SelectLanguage':
      selectLanguage(draft, action.payload);
      break;
    case 'SelectCompareLanguage':
      draft.compareLanguageCode = action.payload;
      break;
    case 'SwitchCompareMode':
      switchCompareMode(draft, action.payload);
      break;
    case 'SetServerError':
      setServerError(draft, action.payload);
      break;
    case 'CancelModification':
      cancelModification(draft, action.payload);
      break;
    case 'SetLanguageVersionTabsErrorsNotificationVisibility':
      setLanguageVersionErrorsVisibility(draft, action.payload);
      break;
    case 'SetDetailedTabErrorsNotificationVisibility':
      setDetailedTabErrorsVisibility(draft, action.payload);
      break;
    case 'SetMissingOrganizationErrorsNotificationVisibility':
      setMissingOrganizationErrorsVisibilty(draft, action.payload);
      break;
    case 'SetTranslationNotificationVisibility':
      setTranslationNotificationVisibility(draft, action.payload);
      break;
    case 'ResetNotificationVisibilities':
      resetNotificationVisibilities(draft);
      break;
    default:
      assertUnreachable(action);
  }
}

function switchFormMode(draft: Draft<IFormMetaContext>, mode: Mode) {
  if (mode === 'view') {
    draft.displayComparison = false;
    draft.compareLanguageCode = undefined;
  }
  draft.mode = mode;
}

function switchFormModeAndSelectLanguage(draft: Draft<IFormMetaContext>, modeWithLanguage: ModeWithLanguage) {
  switchFormMode(draft, modeWithLanguage.mode);
  selectLanguage(draft, modeWithLanguage.language);
  draft.mode = modeWithLanguage.mode;
}

function selectFirstLanguage(draft: Draft<IFormMetaContext>, availableLanguages: Language[]) {
  if (!availableLanguages || availableLanguages.length === 0) {
    return;
  }
  if (!draft.selectedLanguageCode || !availableLanguages.some((lang) => lang === draft.selectedLanguageCode)) {
    draft.selectedLanguageCode = availableLanguages[0];
  }
}

function selectLanguage(draft: Draft<IFormMetaContext>, availableLanguage: Language) {
  if (!availableLanguage) {
    return;
  }
  draft.selectedLanguageCode = availableLanguage;
}

function switchCompareMode(draft: Draft<IFormMetaContext>, isDisplayed: boolean) {
  draft.displayComparison = isDisplayed;

  if (!isDisplayed) {
    draft.compareLanguageCode = undefined;
  }
}

function cancelModification(draft: Draft<IFormMetaContext>, languageOptions: LanguageOptions) {
  draft.compareLanguageCode = undefined;
  draft.displayComparison = false;
  if (languageOptions.enabledLanguages.some((lang) => lang === languageOptions.selectedLanguage)) {
    draft.selectedLanguageCode = languageOptions.selectedLanguage;
  } else {
    draft.selectedLanguageCode = languageOptions.enabledLanguages[0];
  }
  draft.mode = 'view';
}

function setLanguageVersionErrorsVisibility(draft: Draft<IFormMetaContext>, isVisible: boolean) {
  draft.notificationStatuses.languageVersionTabsErrorsVisible = isVisible;
}

function setDetailedTabErrorsVisibility(draft: Draft<IFormMetaContext>, isVisible: boolean) {
  draft.notificationStatuses.detailedTabErrorsVisible = isVisible;
}

function setMissingOrganizationErrorsVisibilty(draft: Draft<IFormMetaContext>, isVisible: boolean) {
  draft.notificationStatuses.missingOrganizationErrorsVisible = isVisible;
}

function setTranslationNotificationVisibility(draft: Draft<IFormMetaContext>, isVisible: boolean) {
  draft.notificationStatuses.translationVisible = isVisible;
}

function setServerError(draft: Draft<IFormMetaContext>, payload: ProblemDetails | undefined) {
  if (payload) {
    resetNotificationVisibilities(draft);
  }
  draft.serverError = payload;
}

function resetNotificationVisibilities(draft: Draft<IFormMetaContext>) {
  draft.notificationStatuses = initialNotificationStatuses;
}
