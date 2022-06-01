import { FieldError, FieldErrors } from 'react-hook-form';
import { getKeys } from 'utils';
import { looksLikeValidEmail } from 'validation';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, EmailLvModel } from 'types/forms/connectionFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { containsErrors, createValidationError } from 'utils/rhf';
import * as fieldConfig from 'validation/fieldConfig';
import { maxLength } from 'validation/rhf';

export function validateAllEmails(input: ConnectionFormModel): LanguageVersionType<FieldErrors<EmailLvModel>[]> | undefined {
  if (!shouldValidate(input)) return undefined;

  const languages = getKeys(input.languageVersions) as Language[];
  const errors: LanguageVersionType<FieldErrors<EmailLvModel>[]> = {};

  let hasErrors = false;
  for (const lang of languages) {
    const emails = input.emails[lang];
    const validationResult = validateEmails(emails);
    if (validationResult) {
      hasErrors = true;
      errors[lang] = validationResult;
    }
  }

  return hasErrors ? errors : undefined;
}

function validateEmails(emails: EmailLvModel[]): FieldErrors<EmailLvModel>[] | undefined {
  // errors Array must have same lenght otherwise errors are displayed for wrong items in the UI.
  const errors: FieldErrors<EmailLvModel>[] = new Array(emails.length);

  for (let index = 0; index < emails.length; index++) {
    const email = emails[index];
    const validationResult = validateEmail(email.value);
    if (validationResult) {
      errors[index] = {
        value: validationResult,
      };
    }
  }

  return containsErrors(errors) ? errors : undefined;
}

function validateEmail(email: string): FieldError | undefined {
  if (!looksLikeValidEmail(email)) return createValidationError('Ptv.ConnectionDetails.EmailAddresses.Validation.Error.InvalidEmail');
  return maxLength(email, fieldConfig.SmallFieldLength);
}

function shouldValidate(input: ConnectionFormModel): boolean {
  return input.channelType === 'EChannel' || input.channelType === 'ServiceLocation' || input.channelType === 'Phone';
}
