import { FieldErrors } from 'react-hook-form';
import { getKeys } from 'utils';
import { ServiceHourLvModel } from 'types/forms/connectionFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import * as fieldConfig from 'validation/fieldConfig';
import { maxLength, purgeEmptyErrors } from 'validation/rhf';

export function validateLanguageVersions(
  lvs: LanguageVersionType<ServiceHourLvModel>
): LanguageVersionType<FieldErrors<ServiceHourLvModel>> | undefined {
  const languages = getKeys(lvs);

  const errors: LanguageVersionType<FieldErrors<ServiceHourLvModel>> = {};

  let hasErrors = false;
  for (const lang of languages) {
    const validationResult = validateLanguageVersion(lvs[lang]);
    if (validationResult) {
      hasErrors = true;
      errors[lang] = validationResult;
    }
  }

  return hasErrors ? errors : undefined;
}

function validateLanguageVersion(lv: ServiceHourLvModel | undefined): FieldErrors<ServiceHourLvModel> | undefined {
  if (!lv) return undefined;
  const errors: FieldErrors<ServiceHourLvModel> = {};
  errors.additionalInformation = maxLength(lv.additionalInformation, fieldConfig.MediumFieldLength);

  return purgeEmptyErrors(errors);
}
