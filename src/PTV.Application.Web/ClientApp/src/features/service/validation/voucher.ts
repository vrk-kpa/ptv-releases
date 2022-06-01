import { FieldErrors } from 'react-hook-form';
import { getKeys } from 'utils';
import { Language } from 'types/enumTypes';
import { ServiceFormValues, ServiceVoucherLink, ServiceVoucherModel } from 'types/forms/serviceFormTypes';
import { createValidationError } from 'utils/rhf';
import * as fieldConfig from 'validation/fieldConfig';
import { maxLength, maxLengthNotEmpty, purgeEmptyErrors, requiredUrl } from 'validation/rhf';

export function validateVoucher(service: ServiceFormValues, voucher: ServiceVoucherModel): FieldErrors<ServiceVoucherModel> | undefined {
  if (service.voucherType === 'NotUsed') return undefined;

  if (service.voucherType === 'NoUrl') {
    return validateNoUrl(voucher);
  }

  return validateWithUrl(voucher, service);
}

function validateNoUrl(voucher: ServiceVoucherModel): FieldErrors<ServiceVoucherModel> | undefined {
  const infoError = maxLength(voucher.info, fieldConfig.MediumFieldLength);

  if (infoError) {
    return {
      info: infoError,
    };
  }

  return undefined;
}

function validateWithUrl(voucher: ServiceVoucherModel, service: ServiceFormValues): FieldErrors<ServiceVoucherModel> | undefined {
  const errors: FieldErrors<ServiceVoucherModel> = {};
  errors.links = validateUrls(voucher);

  // At least one language version needs to have link defined if service voucher with url is selected in service form
  if (!anyLanguageVersionHasVoucherLinks(service)) {
    errors.linksErrorTag = createValidationError('Ptv.Service.Form.Field.ServiceVouchers.InvalidWebpageList');
  }

  return purgeEmptyErrors(errors);
}

function anyLanguageVersionHasVoucherLinks(service: ServiceFormValues): boolean {
  return getKeys(service.languageVersions).some((lang: Language) => {
    const languageVersion = service.languageVersions[lang];
    return languageVersion.voucher.links.length > 0;
  });
}

function validateUrls(voucher: ServiceVoucherModel): FieldErrors<ServiceVoucherLink>[] | undefined {
  const errors: FieldErrors<ServiceVoucherLink>[] = new Array(voucher.links.length);

  let hasErrors = false;
  for (let index = 0; index < voucher.links.length; index++) {
    const result = validateVoucherLink(voucher.links[index]);
    if (result) {
      errors[index] = result;
      hasErrors = true;
    }
  }

  return hasErrors ? errors : undefined;
}

function validateVoucherLink(link: ServiceVoucherLink): FieldErrors<ServiceVoucherLink> | undefined {
  const error: FieldErrors<ServiceVoucherLink> = {
    name: maxLengthNotEmpty(link.name, fieldConfig.SmallFieldLength),
    url: requiredUrl(link.url, fieldConfig.LargeFieldLength),
    additionalInformation: maxLength(link.additionalInformation, fieldConfig.MediumFieldLength),
  };

  return purgeEmptyErrors(error);
}
