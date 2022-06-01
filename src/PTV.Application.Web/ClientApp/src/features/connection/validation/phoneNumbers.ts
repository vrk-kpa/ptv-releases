import { FieldErrors } from 'react-hook-form';
import { getKeys } from 'utils';
import { containsOnlyNumbers } from 'validation';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, PhoneNumberLvModel } from 'types/forms/connectionFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { hasNonEmptyKeys } from 'utils/objects';
import { containsErrors, createValidationError } from 'utils/rhf';
import * as fieldConfig from 'validation/fieldConfig';
import { maxLength } from 'validation/rhf';
import { PhoneNumberMaxLength } from './fieldConfig';

export function validateAllPhoneNumbers(input: ConnectionFormModel): LanguageVersionType<FieldErrors<PhoneNumberLvModel>[]> | undefined {
  if (!shouldValidate(input)) return undefined;

  const languages = getKeys(input.languageVersions) as Language[];
  const errors: LanguageVersionType<FieldErrors<PhoneNumberLvModel>[]> = {};

  let hasErrors = false;
  for (const lang of languages) {
    const phoneNumbers = input.phoneNumbers[lang];
    const validationResult = validatePhoneNumbers(phoneNumbers);
    if (validationResult) {
      hasErrors = true;
      errors[lang] = validationResult;
    }
  }

  return hasErrors ? errors : undefined;
}

function validatePhoneNumbers(phoneNumbers: PhoneNumberLvModel[]): FieldErrors<PhoneNumberLvModel>[] | undefined {
  // errors Array must have same lenght otherwise errors are displayed for wrong items in the UI.
  const errors: FieldErrors<PhoneNumberLvModel>[] = new Array(phoneNumbers.length);

  for (let index = 0; index < phoneNumbers.length; index++) {
    const phoneNumber = phoneNumbers[index];

    const result = validatePhoneNumber(phoneNumber);
    if (result) {
      errors[index] = result;
    }
  }

  return containsErrors(errors) ? errors : undefined;
}

function validatePhoneNumber(phoneNumber: PhoneNumberLvModel): FieldErrors<PhoneNumberLvModel> | undefined {
  const error: FieldErrors<PhoneNumberLvModel> = {};

  if (!phoneNumber.number) {
    error.number = createValidationError('Ptv.Validation.Error.Field.Empty');
  } else if (!containsOnlyNumbers(phoneNumber.number)) {
    error.number = createValidationError('Ptv.Validation.Error.Field.PhoneNumber.InvalidNumber');
  } else {
    error.number = maxLength(phoneNumber.number, PhoneNumberMaxLength);
  }

  if (phoneNumber.dialCodeType === 'Normal') {
    if (!phoneNumber.dialCodeId) {
      error.dialCodeId = createValidationError('Ptv.Validation.Error.Field.Empty');
    } else if (phoneNumber.number.startsWith('0')) {
      error.number = createValidationError('Ptv.Validation.Error.Field.PhoneNumber.CannotStartWithZero');
    }
  }

  error.additionalInformation = maxLength(phoneNumber.additionalInformation, fieldConfig.MediumFieldLength);
  error.chargeDescription = maxLength(phoneNumber.chargeDescription, fieldConfig.MediumFieldLength);

  return hasNonEmptyKeys(error) ? error : undefined;
}

function shouldValidate(input: ConnectionFormModel): boolean {
  return input.channelType === 'EChannel' || input.channelType === 'ServiceLocation' || input.channelType === 'Phone';
}
