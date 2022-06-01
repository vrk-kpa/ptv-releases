import { FieldError } from 'react-hook-form';
import { Language } from 'types/enumTypes';
import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { createValidationError } from 'utils/rhf';
import { translateToLang } from 'utils/translations';

export function validatePurchaseProducers(service: ServiceFormValues, uiLanguage: Language): FieldError | undefined {
  return validateProducers(
    service,
    service.purchaseProducers,
    'Ptv.Service.Form.Field.ServiceProducers.PurchaseProducers.Message.CannotAddDuplicateOrganization',
    uiLanguage
  );
}

export function validateOtherProducers(service: ServiceFormValues, uiLanguage: Language): FieldError | undefined {
  return validateProducers(
    service,
    service.otherProducers,
    'Ptv.Service.Form.Field.ServiceProducers.OtherProducers.Message.CannotAddDuplicateOrganization',
    uiLanguage
  );
}

function validateProducers(
  service: ServiceFormValues,
  producers: OrganizationModel[],
  errorKey: string,
  uiLanguage: Language
): FieldError | undefined {
  const orgs = service.responsibleOrganization
    ? service.otherResponsibleOrganizations.concat([service.responsibleOrganization])
    : service.otherResponsibleOrganizations;

  const invalid = producers.filter((x) => orgs.some((o) => o.id === x.id));
  if (invalid.length !== 0) {
    const names = invalid.map((x) => translateToLang(uiLanguage, x.texts)).join(', ');
    return createValidationError(errorKey, { organizations: names });
  }
}
