import { ControllerFieldState, FieldError, FieldErrors, SetValueConfig, UseFormSetValue } from 'react-hook-form';
import { StringMap, TOptions, t } from 'i18next';
import { TranslatedStatusResult } from 'types/miscellaneousTypes';

// Only reason for this function is that TS goes gracy (crashes or takes 30 seconds) when using
// the type safe setValue method
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function setFormValue(setValue: UseFormSetValue<any>, name: string, value: unknown, options?: SetValueConfig): void {
  setValue(name, value, options);
}

export function containsErrors<T>(input: FieldErrors<T>[]): boolean {
  // If we have e.g. 5 emails then we must report the errors by
  // providing an array which has lenght of 5. Otherwise RHF does not
  // know which error matches which email in the array. So the array
  // might have lenght of 5 but only has one item.
  const elements = input.filter((x) => x);
  return elements.length !== 0;
}

export function createValidationError(key: string, options?: string | TOptions<StringMap> | undefined): FieldError {
  return {
    type: 'validate',
    message: t(key, options),
  };
}

export function toFieldStatus(fieldState: ControllerFieldState): TranslatedStatusResult {
  const status = fieldState.error ? 'error' : 'default';
  const statusText = fieldState.error?.message;

  return {
    status,
    statusText,
  };
}
