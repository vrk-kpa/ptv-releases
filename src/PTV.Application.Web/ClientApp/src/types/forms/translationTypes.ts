import { Language, TranslationStateType } from 'types/enumTypes';

export type LastTranslationType = {
  sourceLanguage: Language;
  targetLanguage: Language;
  state: TranslationStateType;
  estimatedDelivery: string | null | undefined;
  orderedAt: string;
  checked: boolean;
  translationId: string;
};
