import { FieldError, FieldErrors } from 'react-hook-form';
import _ from 'lodash';
import { looksLikeValidUrl } from 'validation';
import { getPlainText, parseRawDescription } from 'utils/draftjs';
import { getNonEmptyKeys } from 'utils/objects';
import { createValidationError } from 'utils/rhf';
import { FieldEmptyErrorKey, InvalidUrlErrorKey, TooManyCharactersErrorKey } from './keys';

export function notEmptyWhenTrimmed(value: string | null | undefined, key?: string): FieldError | undefined {
  const errorKey = key ?? FieldEmptyErrorKey;
  const trimmed = value ? value.trim() : '';
  if (!trimmed) return createValidationError(errorKey);
  return undefined;
}

export function maxLengthNotEmpty(value: string, max: number): FieldError | undefined {
  if (!value) return createValidationError(FieldEmptyErrorKey);

  return maxLength(value, max);
}

export function maxLength(value: string, max: number): FieldError | undefined {
  if (value.length <= max) return undefined;
  return createValidationError(TooManyCharactersErrorKey, { current: value.length, limit: max });
}

export function maxLengthRichText(value: string | null | undefined, max: number): FieldError | undefined {
  if (!value) return undefined;

  const raw = parseRawDescription(value);
  const text = getPlainText(raw);

  if (text.length <= max) return undefined;
  return createValidationError(TooManyCharactersErrorKey, { current: text.length, limit: max });
}

export function requiredUrl(url: string, max: number): FieldError | undefined {
  if (!looksLikeValidUrl(url)) return createValidationError(InvalidUrlErrorKey);
  return maxLength(url, max);
}

export function purgeEmptyErrors<T>(errors: FieldErrors<T>): FieldErrors<T> | undefined {
  // In order for validation to display correctly we need to remove
  // keys which do not have errors. Even if there is null/undefined
  // the validation thinks there is an error.
  const keys = getNonEmptyKeys(errors);
  if (keys.length === 0) return undefined;
  return _.omitBy(errors, _.isNil) as FieldErrors<T>;
}
