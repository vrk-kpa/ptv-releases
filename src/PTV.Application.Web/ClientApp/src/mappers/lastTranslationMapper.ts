import { LastTranslationApiType } from 'types/api/translationApiTypes';
import { LastTranslationType } from 'types/forms/translationTypes';

export function toLastTranslationsUiModel(input: LastTranslationApiType): LastTranslationType {
  const { estimatedDelivery, orderedAt, ...sameFields } = input;

  return {
    estimatedDelivery: !!estimatedDelivery ? estimatedDelivery : null,
    orderedAt: orderedAt,
    ...sameFields,
  };
}

export function toLastTranslationsApiModel(input: LastTranslationType): LastTranslationApiType {
  const { estimatedDelivery, orderedAt, ...sameFields } = input;

  return {
    estimatedDelivery: estimatedDelivery,
    orderedAt: orderedAt,
    ...sameFields,
  };
}
