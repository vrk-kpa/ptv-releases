import { FieldErrors } from 'react-hook-form';
import { ConnectionFormModel, HolidayServiceHourModel } from 'types/forms/connectionFormTypes';
import { containsErrors, createValidationError } from 'utils/rhf';
import { TimeSpan, isMidnight } from 'utils/timespan';

export function validateAllHolidayHours(input: ConnectionFormModel): FieldErrors<HolidayServiceHourModel>[] | undefined {
  if (!shouldValidate(input)) return undefined;

  const hours = input.openingHours.holidayHours;

  // errors Array must have same lenght otherwise errors are displayed for wrong items in the UI.
  const errors: FieldErrors<HolidayServiceHourModel>[] = new Array(hours.length);

  for (let index = 0; index < hours.length; index++) {
    const hour = hours[index];

    const validationResult = validateSpecialHour(hour);
    if (validationResult) {
      errors[index] = validationResult;
    }
  }

  return containsErrors(errors) ? errors : undefined;
}

function validateSpecialHour(hour: HolidayServiceHourModel): FieldErrors<HolidayServiceHourModel> | undefined {
  if (hour.isClosed) return undefined;

  const from = TimeSpan.ParseExact(hour.from);
  const to = TimeSpan.ParseExact(hour.to);

  if (isMidnight(from) || isMidnight(to)) return undefined;

  if (from.smallerThan(to)) return undefined;

  return {
    from: createValidationError('Ptv.Validation.Error.Common.StartTimeSmallerThanEndTime'),
  };
}

function shouldValidate(input: ConnectionFormModel): boolean {
  return input.channelType === 'EChannel' || input.channelType === 'ServiceLocation' || input.channelType === 'Phone';
}
