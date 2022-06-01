import { FieldErrors } from 'react-hook-form';
import { getKeys } from 'utils';
import { looksLikeValidUrl } from 'validation';
import { ConnectionFormModel, WebPageLvModel } from 'types/forms/connectionFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { containsErrors, createValidationError } from 'utils/rhf';
import * as fieldConfig from 'validation/fieldConfig';
import { maxLength, maxLengthNotEmpty, purgeEmptyErrors } from 'validation/rhf';

export function validateAllWebPages(input: ConnectionFormModel): LanguageVersionType<FieldErrors<WebPageLvModel>[]> | undefined {
  if (!shouldValidate(input)) return undefined;

  const languages = getKeys(input.languageVersions);

  const errors: LanguageVersionType<FieldErrors<WebPageLvModel>[]> = {};

  let hasErrors = false;
  for (const lang of languages) {
    const validationResult = validateWebPages(input.webPages[lang]);
    if (validationResult) {
      hasErrors = true;
      errors[lang] = validationResult;
    }
  }

  return hasErrors ? errors : undefined;
}

function validateWebPages(webPages: WebPageLvModel[]): FieldErrors<WebPageLvModel>[] | undefined {
  // errors Array must have same lenght otherwise errors are displayed for wrong items in the UI.
  const errors: FieldErrors<WebPageLvModel>[] = new Array(webPages.length);

  for (let index = 0; index < webPages.length; index++) {
    const validationResult = validateWebPage(webPages[index]);
    if (validationResult) {
      errors[index] = validationResult;
    }
  }

  return containsErrors(errors) ? errors : undefined;
}

function validateWebPage(webPage: WebPageLvModel): FieldErrors<WebPageLvModel> | undefined {
  const errors: FieldErrors<WebPageLvModel> = {};

  if (!looksLikeValidUrl(webPage.url)) {
    errors.url = createValidationError('Ptv.Validation.Error.Field.Url.Invalid');
  } else {
    errors.url = maxLength(webPage.url, fieldConfig.LargeFieldLength);
  }

  errors.additionalInformation = maxLength(webPage.additionalInformation, fieldConfig.MediumFieldLength);
  errors.name = maxLengthNotEmpty(webPage.name, fieldConfig.SmallFieldLength);

  return purgeEmptyErrors(errors);
}

function shouldValidate(input: ConnectionFormModel): boolean {
  return input.channelType === 'EChannel' || input.channelType === 'ServiceLocation' || input.channelType === 'Phone';
}
