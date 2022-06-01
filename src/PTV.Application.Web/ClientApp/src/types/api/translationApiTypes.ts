import { Language, TranslationStateType } from 'types/enumTypes';

export type LastTranslationApiType = {
  sourceLanguage: Language;
  targetLanguage: Language;
  state: TranslationStateType;
  estimatedDelivery: string | null | undefined;
  orderedAt: string;
  checked: boolean;
  translationId: string;
};

export type TranslationDetailApiType = {
  sourceLanguage: Language;
  targetLanguage: Language;
  subscriberEmail: string;
  orderedAt: string;
  additionalInformation: string | null | undefined;
  orderNumber: number;
  orderId: string;
};

export type TranslationHistoryApiType = {
  sourceLanguage: Language;
  targetLanguage: Language;
  orderId: string;
  state: TranslationStateType;
  orderedAt: string;
  subscriberEmail: string;
  orderNumber: number;
  estimatedDelivery: string | null | undefined;
};
