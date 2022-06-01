import { FieldErrors } from 'react-hook-form';
import { getKeys } from 'utils';
import { containsOnlyNumbers } from 'validation';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, FaxNumberLvModel } from 'types/forms/connectionFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { hasNonEmptyKeys } from 'utils/objects';
import { containsErrors, createValidationError } from 'utils/rhf';
import { maxLength } from 'validation/rhf';
import { PhoneNumberMaxLength } from './fieldConfig';

export function validateAllFaxNumbers(input: ConnectionFormModel): LanguageVersionType<FieldErrors<FaxNumberLvModel>[]> | undefined {
  if (!shouldValidate(input)) return undefined;

  const languages = getKeys(input.languageVersions) as Language[];
  const errors: LanguageVersionType<FieldErrors<FaxNumberLvModel>[]> = {};

  let hasErrors = false;
  for (const lang of languages) {
    const faxNumbers = input.faxNumbers[lang];
    const validationResult = validateFaxNumbers(faxNumbers);
    if (validationResult) {
      hasErrors = true;
      errors[lang] = validationResult;
    }
  }

  return hasErrors ? errors : undefined;
}

function validateFaxNumbers(faxNumbers: FaxNumberLvModel[]): FieldErrors<FaxNumberLvModel>[] | undefined {
  // errors Array must have same lenght otherwise errors are displayed for wrong items in the UI.
  const errors: FieldErrors<FaxNumberLvModel>[] = new Array(faxNumbers.length);

  for (let index = 0; index < faxNumbers.length; index++) {
    const faxNumber = faxNumbers[index];

    const result = validateFaxNumber(faxNumber);
    if (result) {
      errors[index] = result;
    }
  }

  return containsErrors(errors) ? errors : undefined;
}

function validateFaxNumber(faxNumber: FaxNumberLvModel): FieldErrors<FaxNumberLvModel> | undefined {
  const error: FieldErrors<FaxNumberLvModel> = {};

  if (!faxNumber.dialCodeId) {
    error.dialCodeId = createValidationError('Ptv.Validation.Error.Field.Empty');
  }

  if (!faxNumber.number) {
    error.number = createValidationError('Ptv.Validation.Error.Field.Empty');
  } else if (faxNumber.number.startsWith('0')) {
    error.number = createValidationError('Ptv.Validation.Error.Field.PhoneNumber.CannotStartWithZero');
  } else if (!containsOnlyNumbers(faxNumber.number)) {
    error.number = createValidationError('Ptv.Validation.Error.Field.PhoneNumber.InvalidNumber');
  } else {
    error.number = maxLength(faxNumber.number, PhoneNumberMaxLength);
  }

  return hasNonEmptyKeys(error) ? error : undefined;
}

function shouldValidate(input: ConnectionFormModel): boolean {
  return input.channelType === 'EChannel' || input.channelType === 'ServiceLocation' || input.channelType === 'Phone';
}
