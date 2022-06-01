import { createContext } from 'react';
import { Language, Mode } from 'types/enumTypes';
import { LanguageOptions, ModeWithLanguage, ProblemDetails } from 'types/miscellaneousTypes';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type Payload = any | null;

export type Action =
  | { type: 'SwitchFormMode'; payload: Mode }
  | { type: 'SwitchFormModeAndSelectLanguage'; payload: ModeWithLanguage }
  | { type: 'SelectFirstLanguage'; payload: Language[] }
  | { type: 'SelectLanguage'; payload: Language }
  | { type: 'SelectCompareLanguage'; payload: Language | undefined }
  | { type: 'SwitchCompareMode'; payload: boolean }
  | { type: 'SetServerError'; payload: ProblemDetails | undefined }
  | { type: 'CancelModification'; payload: LanguageOptions }
  | { type: 'SetLanguageVersionTabsErrorsNotificationVisibility'; payload: boolean }
  | { type: 'SetDetailedTabErrorsNotificationVisibility'; payload: boolean }
  | { type: 'SetMissingOrganizationErrorsNotificationVisibility'; payload: boolean }
  | { type: 'SetTranslationNotificationVisibility'; payload: boolean }
  | { type: 'ResetNotificationVisibilities' };

export type Dispatch = (action: Action) => void;

// eslint-disable-next-line @typescript-eslint/no-empty-function
export const DispatchContext = createContext<Dispatch>(() => {});
