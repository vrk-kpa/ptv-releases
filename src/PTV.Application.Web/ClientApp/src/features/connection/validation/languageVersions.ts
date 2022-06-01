import { FieldErrors } from 'react-hook-form';
import _ from 'lodash';
import { getKeys } from 'utils';
import { ChargeApiModel } from 'types/api/chargeApiModel';
import { ConnectionFormLvModel, ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { getNonEmptyKeys } from 'utils/objects';
import * as fieldConfig from 'validation/fieldConfig';
import { maxLengthRichText } from 'validation/rhf';

export function validateAllLanguageVersions(
  input: ConnectionFormModel
): LanguageVersionType<FieldErrors<ConnectionFormLvModel>> | undefined {
  const languages = getKeys(input.languageVersions);

  const errors: LanguageVersionType<FieldErrors<ConnectionFormLvModel>> = {};

  let hasErrors = false;
  for (const lang of languages) {
    const validationResult = validateLanguageVersion(input.languageVersions[lang]);
    if (validationResult) {
      hasErrors = true;
      errors[lang] = validationResult;
    }
  }

  return hasErrors ? errors : undefined;
}

function validateLanguageVersion(lv: ConnectionFormLvModel | undefined): FieldErrors<ConnectionFormLvModel> | undefined {
  if (!lv) return undefined;
  const errors: FieldErrors<ConnectionFormLvModel> = {};
  errors.description = maxLengthRichText(lv.description, fieldConfig.LargeFieldLength);
  errors.charge = validateCharge(lv);

  const keys = getNonEmptyKeys(errors);
  return keys.length === 0 ? undefined : _.omitBy(errors, _.isNil);
}

function validateCharge(lv: ConnectionFormLvModel): FieldErrors<ChargeApiModel> | undefined {
  const infoError = maxLengthRichText(lv.charge.info, fieldConfig.LargeFieldLength);
  if (infoError) {
    return {
      info: infoError,
    };
  }

  return undefined;
}
