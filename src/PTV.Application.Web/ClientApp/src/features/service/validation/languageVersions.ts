import { FieldError, FieldErrors } from 'react-hook-form';
import _ from 'lodash';
import { ServiceType } from 'types/enumTypes';
import { ChargeModel } from 'types/forms/chargeType';
import { ServiceFormValues, ServiceLanguageVersionValues, ServiceModelLanguageVersion } from 'types/forms/serviceFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { getPlainText, parseRawDescription } from 'utils/draftjs';
import { getNonEmptyKeys } from 'utils/objects';
import { createValidationError } from 'utils/rhf';
import { getEnabledLanguages } from 'utils/service';
import * as fieldConfig from 'validation/fieldConfig';
import { TooManyCharactersErrorKey } from 'validation/keys';
import { maxLength, maxLengthRichText } from 'validation/rhf';
import { validateAllLaws } from './laws';
import { validateVoucher } from './voucher';

export function validateLanguageVersions(
  service: ServiceFormValues
): LanguageVersionType<FieldErrors<ServiceModelLanguageVersion>> | undefined {
  const languages = getEnabledLanguages(service.languageVersions);

  const errors: LanguageVersionType<FieldErrors<ServiceModelLanguageVersion>> = {};

  for (const lang of languages) {
    errors[lang] = validateLanguageVersion(service, service.languageVersions[lang]);
  }

  const keys = getNonEmptyKeys(errors);
  return keys.length === 0 ? undefined : _.omitBy(errors, _.isNil);
}

function validateLanguageVersion(
  service: ServiceFormValues,
  ln: ServiceLanguageVersionValues
): FieldErrors<ServiceModelLanguageVersion> | undefined {
  const errors: FieldErrors<ServiceModelLanguageVersion> = {};
  errors.name = validateName(ln.name, ln.summary);
  errors.alternativeName = validateAlternativeName(ln.alternativeName, ln.hasAlternativeName);
  errors.summary = validateSummary(ln.summary, ln.name);
  errors.description = maxLengthRichText(ln.description, fieldConfig.ExtraLargeFieldLength);
  errors.userInstructions = maxLengthRichText(ln.userInstructions, fieldConfig.ExtraLargeFieldLength);
  errors.conditions = maxLengthRichText(ln.conditions, fieldConfig.ExtraLargeFieldLength);
  errors.deadline = optionalRichText(ln.deadline, fieldConfig.LargeFieldLength, service.serviceType);
  errors.processingTime = optionalRichText(ln.processingTime, fieldConfig.LargeFieldLength, service.serviceType);
  errors.periodOfValidity = optionalRichText(ln.periodOfValidity, fieldConfig.LargeFieldLength, service.serviceType);
  errors.charge = validateCharge(ln.charge);
  errors.laws = validateAllLaws(ln.laws);
  errors.voucher = validateVoucher(service, ln.voucher);

  const keys = getNonEmptyKeys(errors);
  return keys.length === 0 ? undefined : _.omitBy(errors, _.isNil);
}

function validateName(name: string, summary: string): FieldError | undefined {
  if (!name) return createValidationError('Ptv.Validation.Error.Field.Empty');
  if (name === summary) return createValidationError('Ptv.Validation.Error.NameAndSummaryEqual');
  return maxLength(name, fieldConfig.SmallFieldLength);
}

function validateAlternativeName(value: string, hasAlternativeName: boolean): FieldError | undefined {
  if (!hasAlternativeName) return undefined;
  return maxLength(value, fieldConfig.SmallFieldLength);
}

function validateSummary(summary: string, name: string): FieldError | undefined {
  if (summary === name) return createValidationError('Ptv.Validation.Error.NameAndSummaryEqual');
  return maxLength(summary, fieldConfig.MediumFieldLength);
}

function validateCharge(data: ChargeModel): FieldErrors<ChargeModel> | undefined {
  const raw = parseRawDescription(data.info);
  const text = getPlainText(raw);
  if (text.length <= fieldConfig.LargeFieldLength) return undefined;

  return {
    info: createValidationError(TooManyCharactersErrorKey, { current: text.length, limit: fieldConfig.LargeFieldLength }),
  };
}

function optionalRichText(value: string | null | undefined, max: number, serviceType: ServiceType): FieldError | undefined {
  if (serviceType === 'Service') return undefined;
  return maxLengthRichText(value, max);
}
