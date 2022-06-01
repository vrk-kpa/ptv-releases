import { FieldError } from 'react-hook-form';
import { ServiceFormContext, ServiceFormValues } from 'types/forms/serviceFormTypes';
import { createValidationError } from 'utils/rhf';

export const MaxServiceClasses = 4;

export function validateServiceClasses(service: ServiceFormValues, context: ServiceFormContext | undefined): FieldError | undefined {
  const all = service.generalDescription
    ? service.generalDescription.serviceClasses.concat(service.serviceClasses)
    : service.serviceClasses;

  if (all.length > MaxServiceClasses) {
    return createValidationError('Ptv.Service.Form.Field.ServiceClasses.Message.TooManySelected');
  }

  if (context) {
    const mainClasses = context.serviceClasses.filter((x) => all.includes(x.id) && !x.parentId);
    if (mainClasses.length === all.length && all.length !== 0) {
      return createValidationError('Ptv.Service.Form.Field.ServiceClasses.Message.MainClassesOnly');
    }
  }

  return undefined;
}
