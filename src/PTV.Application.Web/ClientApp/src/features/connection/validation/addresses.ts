import { FieldError, FieldErrors } from 'react-hook-form';
import { AddressType, Language } from 'types/enumTypes';
import { AddressLvModel, AddressModel, ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { containsErrors, createValidationError } from 'utils/rhf';
import * as fieldConfig from 'validation/fieldConfig';
import { maxLength, maxLengthNotEmpty, notEmptyWhenTrimmed, purgeEmptyErrors } from 'validation/rhf';
import { PoBoxMaxLength, StreetNumberMaxLength } from './fieldConfig';

export function validateAllAddresses(input: ConnectionFormModel, selectedLanguageCode: Language): FieldErrors<AddressModel>[] | undefined {
  // errors Array must have same lenght otherwise errors are displayed for wrong items in the UI.
  const errors: FieldErrors<AddressModel>[] = new Array(input.addresses.length);

  for (let index = 0; index < input.addresses.length; index++) {
    const address = input.addresses[index];
    const validationResult = validateAddress(address, selectedLanguageCode);
    if (validationResult) {
      errors[index] = validationResult;
    }
  }

  return containsErrors(errors) ? errors : undefined;
}

const validateAddress = (address: AddressModel, selectedLanguageCode: Language): FieldErrors<AddressModel> | undefined => {
  const errors: FieldErrors<AddressModel> = {};
  errors.postalCode = validatePostalCode(address);
  errors.countryCode = validateCountryCode(address);
  errors.street = validateStreet(address);
  errors.streetName = validateStreetName(address, selectedLanguageCode);
  errors.streetNumber = validateStreetNumber(address);
  errors.languageVersions = validateLanguageVersions(address, selectedLanguageCode);
  return purgeEmptyErrors(errors);
};

const validatePostalCode = (input: AddressModel): FieldError | undefined => {
  const addressTypesToTest: AddressType[] = ['Street', 'PostOfficeBox'];

  if (addressTypesToTest.includes(input.type)) {
    return notEmptyWhenTrimmed(input.postalCode, 'Ptv.ConnectionDetails.Addresses.Validation.Error.PostalCodeRequired');
  }
  return undefined;
};

const validateCountryCode = (input: AddressModel): FieldError | undefined => {
  const addressTypesToTest: AddressType[] = ['Foreign'];
  if (addressTypesToTest.includes(input.type)) {
    return notEmptyWhenTrimmed(input.countryCode, 'Ptv.ConnectionDetails.Addresses.Validation.Error.CountryCodeRequired');
  }
  return undefined;
};

const validateStreet = (input: AddressModel): FieldError | undefined => {
  const addressTypesToTest: AddressType[] = ['Street'];
  if (addressTypesToTest.includes(input.type) && input.street == null) {
    return createValidationError('Ptv.ConnectionDetails.Addresses.Validation.Error.StreetEmpty');
  }
  return undefined;
};

const validateStreetName = (input: AddressModel, selectedLanguageCode: Language): LanguageVersionType<FieldError> | undefined => {
  if (input.type !== 'Street') return undefined;

  const errors: LanguageVersionType<FieldError> = {};
  const streetName = input.streetName[selectedLanguageCode] ?? '';
  errors[selectedLanguageCode] = maxLengthNotEmpty(streetName, fieldConfig.SmallFieldLength);
  return purgeEmptyErrors(errors);
};

const validateStreetNumber = (input: AddressModel): FieldError | undefined => {
  if (input.type !== 'Street') return undefined;
  return maxLength(input.streetNumber, StreetNumberMaxLength);
};

const validateLanguageVersions = (
  input: AddressModel,
  selectedLanguageCode: Language
): LanguageVersionType<FieldErrors<AddressLvModel>> | undefined => {
  const errors: LanguageVersionType<FieldErrors<AddressLvModel>> = {};
  const languages: Language[] = [selectedLanguageCode];

  let hasErrors = false;
  for (const lang of languages) {
    const validationResult = validateLanguageVersion(input, selectedLanguageCode);
    if (validationResult) {
      hasErrors = true;
      errors[lang] = validationResult;
    }
  }

  return hasErrors ? errors : undefined;
};

const validateLanguageVersion = (input: AddressModel, selectedLanguageCode: Language): FieldErrors<AddressLvModel> | undefined => {
  const errors: FieldErrors<AddressLvModel> = {};
  errors.poBox = validatePoBox(input, selectedLanguageCode);
  errors.foreignAddress = validateForeignAddress(input, selectedLanguageCode);
  errors.additionalInformation = validateAdditionalInfo(input, selectedLanguageCode);
  return purgeEmptyErrors(errors);
};

const validatePoBox = (input: AddressModel, selectedLanguageCode: Language): FieldError | undefined => {
  if (input.type !== 'PostOfficeBox') return undefined;
  const lv = input.languageVersions[selectedLanguageCode];
  return maxLengthNotEmpty(lv.poBox, PoBoxMaxLength);
};

const validateForeignAddress = (input: AddressModel, selectedLanguageCode: Language): FieldError | undefined => {
  if (input.type !== 'Foreign') return undefined;
  const lv = input.languageVersions[selectedLanguageCode];
  return maxLengthNotEmpty(lv.foreignAddress, fieldConfig.LargeFieldLength);
};

const validateAdditionalInfo = (input: AddressModel, selectedLanguageCode: Language): FieldError | undefined => {
  const addressTypesToTest: AddressType[] = ['PostOfficeBox', 'Street'];
  if (!addressTypesToTest.includes(input.type)) return undefined;
  const lv = input.languageVersions[selectedLanguageCode];
  return maxLength(lv.additionalInformation, fieldConfig.MediumFieldLength);
};
