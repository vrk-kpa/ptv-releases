import { FieldErrors, ResolverResult } from 'react-hook-form';
import _ from 'lodash';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { IFormMetaContext } from 'context/formMeta/FormMetaContext';
import { getNonEmptyKeys } from 'utils/objects';
import { validateAllAddresses } from './addresses';
import { validateAllEmails } from './emails';
import { validateAllFaxNumbers } from './faxNumbers';
import { validateAllLanguageVersions } from './languageVersions';
import { validateOpeningHours } from './openingHours';
import { validateAllPhoneNumbers } from './phoneNumbers';
import { validateAllWebPages } from './webPages';

export function validateConnectionFormModel(
  input: ConnectionFormModel,
  contextInput: IFormMetaContext | unknown | undefined
): ResolverResult<ConnectionFormModel> {
  const context: IFormMetaContext | undefined = contextInput as IFormMetaContext;

  const errors: FieldErrors<ConnectionFormModel> = {
    languageVersions: validateAllLanguageVersions(input),
    emails: validateAllEmails(input),
    webPages: validateAllWebPages(input),
    openingHours: validateOpeningHours(input),
    phoneNumbers: validateAllPhoneNumbers(input),
    faxNumbers: validateAllFaxNumbers(input),
    addresses: validateAllAddresses(input, context.selectedLanguageCode),
  };

  // If there are no errors the validation must return empty errors object, otherwise
  // form thinks that the validation has failed
  const keys = getNonEmptyKeys(errors);
  if (keys.length === 0) {
    return {
      errors: {},
      values: input,
    };
  }

  const result: ResolverResult<ConnectionFormModel> = {
    errors: _.omitBy(errors, _.isNil),
    values: {},
  };

  return result;
}
