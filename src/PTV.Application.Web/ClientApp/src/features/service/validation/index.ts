import { FieldErrors, ResolverResult } from 'react-hook-form';
import _ from 'lodash';
import { ServiceFormContext, ServiceFormValues, ServiceModel } from 'types/forms/serviceFormTypes';
import { getNonEmptyKeys } from 'utils/objects';
import { validateLanguageVersions } from './languageVersions';
import { validateOntologyTerms } from './ontologyTerms';
import { validateOtherProducers, validatePurchaseProducers } from './producers';
import { validateServiceClasses } from './serviceClasses';

export function validateServiceForm(input: ServiceFormValues, contextInput: unknown | undefined): ResolverResult<ServiceModel> {
  const context: ServiceFormContext | undefined = contextInput as ServiceFormContext;
  if (!context) {
    throw new Error('Service form validation context not initialized');
  }

  const errors: FieldErrors<ServiceModel> = {
    languageVersions: validateLanguageVersions(input),
    purchaseProducers: validatePurchaseProducers(input, context.uiLanguage),
    otherProducers: validateOtherProducers(input, context.uiLanguage),
    serviceClasses: validateServiceClasses(input, context),
    ontologyTerms: validateOntologyTerms(input),
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

  const result: ResolverResult<ServiceModel> = {
    errors: _.omitBy(errors, _.isNil),
    values: {},
  };

  return result;
}
