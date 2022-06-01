import { FieldErrors } from 'react-hook-form';
import { ConnectionFormModel, SpecialServiceHourModel } from 'types/forms/connectionFormTypes';
import { compareWeekDays, toLocalDateTime } from 'utils/date';
import { containsErrors, createValidationError } from 'utils/rhf';
import { purgeEmptyErrors } from 'validation/rhf';
import { validateLanguageVersions } from './serviceHours';
import { bothAreMidnight, isStartTimeBeforeEndTime } from './utils';

export function validateSpecialHours(input: ConnectionFormModel): FieldErrors<SpecialServiceHourModel>[] | undefined {
  if (!shouldValidate(input)) return undefined;

  const hours = input.openingHours.specialHours;

  // errors Array must have same lenght otherwise errors are displayed for wrong items in the UI.
  const errors: FieldErrors<SpecialServiceHourModel>[] = new Array(hours.length);

  for (let index = 0; index < hours.length; index++) {
    const hour = hours[index];

    const validationResult = validateSpecialHour(hour);
    if (validationResult) {
      errors[index] = validationResult;
    }
  }

  return containsErrors(errors) ? errors : undefined;
}

function validateSpecialHour(hour: SpecialServiceHourModel): FieldErrors<SpecialServiceHourModel> | undefined {
  const errors: FieldErrors<SpecialServiceHourModel> = {};
  errors.languageVersions = validateLanguageVersions(hour.languageVersions);

  if (hour.validityPeriod === 'UntilFurtherNotice') {
    return validateUntilFurtherNoticeOption(hour);
  }

  if (hour.validityPeriod === 'BetweenDates') {
    return validateBetweenDatesOption(hour);
  }

  return undefined;
}

function validateUntilFurtherNoticeOption(hour: SpecialServiceHourModel): FieldErrors<SpecialServiceHourModel> | undefined {
  if (!hour.dayTo) return undefined;

  const errors: FieldErrors<SpecialServiceHourModel> = {};
  errors.languageVersions = validateLanguageVersions(hour.languageVersions);

  const result = compareWeekDays(hour.dayFrom, hour.dayTo);

  // Starts and ends on the same day
  if (result === 0) {
    // Normally start time must be before end time but UI allows user to
    // set 24h time (00:00 - 24:00) so allow that
    if (bothAreMidnight(hour.timeFrom, hour.timeTo)) {
    } else {
      if (!isStartTimeBeforeEndTime(hour.timeFrom, hour.timeTo)) {
        errors.dayFrom = createValidationError('Ptv.Validation.Error.Common.StartTimeSmallerThanEndTime');
      }
    }
  }

  return purgeEmptyErrors(errors);
}

function validateBetweenDatesOption(hour: SpecialServiceHourModel): FieldErrors<SpecialServiceHourModel> | undefined {
  const errors: FieldErrors<SpecialServiceHourModel> = {};
  errors.languageVersions = validateLanguageVersions(hour.languageVersions);

  if (!hour.openingHoursFrom) {
    errors.openingHoursFrom = createValidationError('Ptv.Validation.Error.Field.Empty');
  }

  if (!hour.openingHoursTo) {
    errors.openingHoursTo = createValidationError('Ptv.Validation.Error.Field.Empty');
  }

  if (hour.openingHoursFrom && hour.openingHoursTo) {
    const startDate = toLocalDateTime(hour.openingHoursFrom);
    const endDate = toLocalDateTime(hour.openingHoursTo);

    if (startDate > endDate) {
      errors.openingHoursFrom = createValidationError('Ptv.Validation.Error.Common.StartTimeSmallerThanEndTime');
    } else if (startDate.toMillis() === endDate.toMillis()) {
      if (!bothAreMidnight(hour.timeFrom, hour.timeTo)) {
        if (!isStartTimeBeforeEndTime(hour.timeFrom, hour.timeTo)) {
          errors.openingHoursFrom = createValidationError('Ptv.Validation.Error.Common.StartTimeSmallerThanEndTime');
        }
      }
    }
  }

  return purgeEmptyErrors(errors);
}

function shouldValidate(input: ConnectionFormModel): boolean {
  return input.channelType === 'EChannel' || input.channelType === 'ServiceLocation' || input.channelType === 'Phone';
}
