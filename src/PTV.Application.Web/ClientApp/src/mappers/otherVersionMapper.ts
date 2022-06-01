import { OtherVersionApiType } from 'types/api/otherVersionApiType';
import { OtherVersionType } from 'types/forms/otherVersionType';

export function toOtherLanguageVersionUiModel(input: OtherVersionApiType | null | undefined): OtherVersionType | null {
  if (!input) {
    return null;
  }

  return {
    id: input.id,
    modified: input.modified,
  };
}

export function toOtherLanguageVersionApiModel(input: OtherVersionType | null | undefined): OtherVersionApiType | null {
  if (!input) {
    return null;
  }

  return {
    id: input.id,
    modified: input.modified,
  };
}
