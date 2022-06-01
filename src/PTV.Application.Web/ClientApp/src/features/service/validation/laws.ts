import { FieldErrors } from 'react-hook-form';
import { LinkModel } from 'types/link';
import { containsErrors } from 'utils/rhf';
import * as fieldConfig from 'validation/fieldConfig';
import { maxLengthNotEmpty, purgeEmptyErrors, requiredUrl } from 'validation/rhf';

export function validateAllLaws(laws: LinkModel[]): FieldErrors<LinkModel>[] | undefined {
  // errors Array must have same lenght otherwise errors are displayed for wrong items in the UI.
  const errors: FieldErrors<LinkModel>[] = new Array(laws.length);

  for (let index = 0; index < laws.length; index++) {
    const result = validateLaw(laws[index]);
    if (result) {
      errors[index] = result;
    }
  }

  return containsErrors(errors) ? errors : undefined;
}

function validateLaw(law: LinkModel): FieldErrors<LinkModel> | undefined {
  const errors: FieldErrors<LinkModel> = {
    url: requiredUrl(law.url, fieldConfig.LargeFieldLength),
    name: maxLengthNotEmpty(law.name, fieldConfig.LargeFieldLength),
  };

  return purgeEmptyErrors(errors);
}
