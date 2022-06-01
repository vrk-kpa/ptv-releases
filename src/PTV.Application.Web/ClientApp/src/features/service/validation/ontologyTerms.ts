import { FieldError } from 'react-hook-form';
import { OntologyTerm } from 'types/classificationItemsTypes';
import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import { createValidationError } from 'utils/rhf';

export const MaxOntologyTerms = 10;

export function validateOntologyTerms(service: ServiceFormValues): FieldError | undefined {
  const gdOntologyTerms = service.generalDescription ? service.generalDescription.ontologyTerms : [];
  const count = getOntologyTermCount(gdOntologyTerms, service.ontologyTerms);
  if (count > MaxOntologyTerms) {
    return createValidationError('Ptv.Service.Form.Field.OntologyTerms.Message.TooManySelected');
  }

  return undefined;
}

export function getOntologyTermCount(gdOntologyTerms: OntologyTerm[], serviceOntologyTerms: OntologyTerm[]): number {
  return gdOntologyTerms.length + serviceOntologyTerms.length;
}
