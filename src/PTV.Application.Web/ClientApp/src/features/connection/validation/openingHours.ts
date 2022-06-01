import { FieldErrors } from 'react-hook-form';
import { ConnectionFormModel, OpeningHoursModel } from 'types/forms/connectionFormTypes';
import { getNonEmptyKeys } from 'utils/objects';
import { validateAllExceptionalHours } from './exceptionalHours';
import { validateAllHolidayHours } from './holidayHours';
import { validateSpecialHours } from './specialHours';
import { validateStandardHours } from './standardHours';

export function validateOpeningHours(input: ConnectionFormModel): FieldErrors<OpeningHoursModel> | undefined {
  const errors: FieldErrors<OpeningHoursModel> = {
    standardHours: validateStandardHours(input),
    specialHours: validateSpecialHours(input),
    holidayHours: validateAllHolidayHours(input),
    exceptionalHours: validateAllExceptionalHours(input),
  };

  const keys = getNonEmptyKeys(errors);
  return keys.length === 0 ? undefined : errors;
}
