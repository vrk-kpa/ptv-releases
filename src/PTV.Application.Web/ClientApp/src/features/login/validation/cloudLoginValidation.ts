import { FieldError, FieldErrors, ResolverResult } from 'react-hook-form';
import _ from 'lodash';
import { LoginModel } from 'types/loginTypes';
import { getNonEmptyKeys } from 'utils/objects';
import { createValidationError } from 'utils/rhf';

export function validateLoginForm(input: LoginModel): ResolverResult<LoginModel> {
  const errors: FieldErrors<LoginModel> = {
    name: requiredStr(input.name),
    password: requiredStr(input.password),
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

  const result: ResolverResult<LoginModel> = {
    errors: _.omitBy(errors, _.isNil),
    values: {},
  };

  return result;
}

function requiredStr(value: string): FieldError | undefined {
  if (!value) return createValidationError('Ptv.Validation.Error.Field.Empty');
  return undefined;
}
