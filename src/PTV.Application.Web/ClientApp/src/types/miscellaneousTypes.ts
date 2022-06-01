import { Language, Mode } from './enumTypes';
import { LanguageVersionType, RequiredLanguageVersionType } from './languageVersionTypes';

export type RequiredLocalizedText = RequiredLanguageVersionType<string>;

export type LocalizedText = LanguageVersionType<string>;
// For backwards compatiblity with old apis
export type TranslatedText = {
  DefaultText: string;
  Texts: LocalizedText;
  Id: string;
};

export type WebPageModel = {
  id: string;
  name: string | null;
  urlAddress: string;
};

export type TranslationAvailabilityType = {
  isTranslationDelivered: boolean;
  isInTranslation: boolean;
  canBeTranslated: boolean;
};

export type OpeningHours = {
  exceptionalOpeningHours: Record<string, unknown>[]; // TODO: implement real type
  normalOpeningHours: Record<string, unknown>[]; // TODO: implement real type
  specialOpeningHours: Record<string, unknown>[]; // TODO: implement real type
  holidayHours: Record<string, unknown>[]; // TODO: implement real type
};

export interface ApiMessage {
  code: string | null;
  subCode: string | null;
  params: string[] | null;
}

export interface ApiError extends ApiMessage {
  stackTrace: string | null;
}

export type ApiResponseWrapper<T> = {
  data: T;
  doNotCamelize: boolean;
  messages: {
    errors: ApiError[];
    warnings: ApiMessage[];
    infos: ApiMessage[];
  };
};

export class HttpError extends Error {
  readonly response?: Response;
  readonly details?: ProblemDetails;
  constructor(response?: Response, message?: string, details?: ProblemDetails) {
    super(message);
    this.response = response;
    this.details = details;

    // Below so that instanceof returns HttpError instead of Error
    Object.setPrototypeOf(this, new.target.prototype);
    this.name = HttpError.name;
  }

  isUnathorizedError(): boolean {
    return this.response?.status === 401;
  }

  isNotFoundError(): boolean {
    return this.response?.status === 404;
  }

  isRetryableError(): boolean {
    if (this.isUnathorizedError() || this.isNotFoundError()) {
      return false;
    }

    return true;
  }
}

export type SortType = 'default' | null | undefined;

export type KeyAndText<TKey> = {
  key: TKey;
  text: string;
};

export type ProblemDetails = {
  detail: string;
  instance: string;
  status: number;
  title: string;
  type: string;
};

export type TranslatedStatusResult = {
  status: 'default' | 'error';
  statusText: string | undefined;
};

export type LanguageOptions = {
  selectedLanguage: Language;
  enabledLanguages: Language[];
};

export type ModeWithLanguage = {
  mode: Mode;
  language: Language;
};
