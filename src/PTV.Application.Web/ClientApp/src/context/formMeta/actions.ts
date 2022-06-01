import { Language, Mode } from 'types/enumTypes';
import { LanguageOptions, ModeWithLanguage, ProblemDetails } from 'types/miscellaneousTypes';
import { Dispatch } from './DispatchFormMetaContext';

export function switchCompareMode(dispatch: Dispatch, isDisplayed: boolean): void {
  dispatch({
    type: 'SwitchCompareMode',
    payload: isDisplayed,
  });
}

export function changeCompareLanguage(dispatch: Dispatch, language: Language | undefined): void {
  dispatch({
    type: 'SelectCompareLanguage',
    payload: language,
  });
}

export function switchFormMode(dispatch: Dispatch, mode: Mode): void {
  dispatch({
    type: 'SwitchFormMode',
    payload: mode,
  });
}

export function switchFormModeAndSelectLanguage(dispatch: Dispatch, modeWithLanguage: ModeWithLanguage) {
  dispatch({
    type: 'SwitchFormModeAndSelectLanguage',
    payload: modeWithLanguage,
  });
}

export function setFirstTabAsSelected(dispatch: Dispatch, availableLanguages: Language[]): void {
  dispatch({
    type: 'SelectFirstLanguage',
    payload: availableLanguages,
  });
}

export function selectLanguage(dispatch: Dispatch, newLanguage: Language): void {
  dispatch({
    type: 'SelectLanguage',
    payload: newLanguage,
  });
}

export function setServerError(dispatch: Dispatch, details: ProblemDetails | undefined): void {
  dispatch({
    type: 'SetServerError',
    payload: details,
  });
}

export function cancelModification(dispatch: Dispatch, languageOptions: LanguageOptions): void {
  dispatch({
    type: 'CancelModification',
    payload: languageOptions,
  });
}

export function setOtherTabsErrorsVisibility(dispatch: Dispatch, isVisible: boolean): void {
  dispatch({
    type: 'SetLanguageVersionTabsErrorsNotificationVisibility',
    payload: isVisible,
  });
}

export function setMissingOrganizationErrorsVisibilty(dispatch: Dispatch, isVisible: boolean): void {
  dispatch({
    type: 'SetMissingOrganizationErrorsNotificationVisibility',
    payload: isVisible,
  });
}

export function setDetailedTabErrorsVisibility(dispatch: Dispatch, isVisible: boolean): void {
  dispatch({
    type: 'SetDetailedTabErrorsNotificationVisibility',
    payload: isVisible,
  });
}

export function SetTranslationNotificationVisibility(dispatch: Dispatch, isVisible: boolean): void {
  dispatch({
    type: 'SetTranslationNotificationVisibility',
    payload: isVisible,
  });
}

export function resetNotificationVisibilities(dispatch: Dispatch): void {
  dispatch({
    type: 'ResetNotificationVisibilities',
  });
}
