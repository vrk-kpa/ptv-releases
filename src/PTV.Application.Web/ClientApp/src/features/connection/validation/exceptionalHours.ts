import { FieldError, FieldErrors } from 'react-hook-form';
import { ConnectionFormModel, ExceptionalServiceHourModel } from 'types/forms/connectionFormTypes';
import { toOptionalDateTime } from 'utils/date';
import { getNonEmptyKeys } from 'utils/objects';
import { containsErrors, createValidationError } from 'utils/rhf';
import { validateLanguageVersions } from './serviceHours';
import { bothAreMidnight, isStartTimeBeforeEndTime } from './utils';

export function validateAllExceptionalHours(input: ConnectionFormModel): FieldErrors<ExceptionalServiceHourModel>[] | undefined {
  if (!shouldValidate(input)) return undefined;

  const hours = input.openingHours.exceptionalHours;

  // errors Array must have same lenght otherwise errors are displayed for wrong items in the UI.
  const errors: FieldErrors<ExceptionalServiceHourModel>[] = new Array(hours.length);

  for (let index = 0; index < hours.length; index++) {
    const hour = hours[index];

    const validationResult = validateExceptionalHour(hour);
    if (validationResult) {
      errors[index] = validationResult;
    }
  }

  return containsErrors(errors) ? errors : undefined;
}

function validateExceptionalHour(hour: ExceptionalServiceHourModel): FieldErrors<ExceptionalServiceHourModel> | undefined {
  const result: FieldErrors<ExceptionalServiceHourModel> = {
    languageVersions: validateLanguageVersions(hour.languageVersions),
    validityPeriod: getValidityPeriodError(hour),
    openingHoursFrom: getOpeningHoursFromError(hour),
    openingHoursTo: getOpeningHoursToError(hour),
    timeFrom: getOpeningTimeError(hour),
  };

  const keys = getNonEmptyKeys(result);
  return keys.length === 0 ? undefined : result;
}

function getValidityPeriodError(hour: ExceptionalServiceHourModel): FieldError | undefined {
  if (!hour.validityPeriod) {
    return createValidationError('Ptv.Validation.Error.Field.Empty');
  }

  return undefined;
}

function getOpeningTimeError(hour: ExceptionalServiceHourModel): FieldError | undefined {
  if (hour.validityPeriod === 'Day') {
    if (hour.isClosed) return undefined;

    // Allow user to create 24 hour time
    if (bothAreMidnight(hour.timeFrom, hour.timeTo)) return undefined;

    if (!isStartTimeBeforeEndTime(hour.timeFrom, hour.timeTo)) {
      return createValidationError('Ptv.Validation.Error.Common.StartTimeSmallerThanEndTime');
    }
  }

  if (hour.validityPeriod === 'BetweenDates') {
    const from = toOptionalDateTime(hour.openingHoursFrom);
    const to = toOptionalDateTime(hour.openingHoursTo);

    // If dates are equal then start time must be before end time
    if (from && to && from.ordinal === to.ordinal) {
      // Allow user to create 24 hour time
      if (bothAreMidnight(hour.timeFrom, hour.timeTo)) return undefined;

      if (!isStartTimeBeforeEndTime(hour.timeFrom, hour.timeTo)) {
        return createValidationError('Ptv.Validation.Error.Common.StartTimeSmallerThanEndTime');
      }
    }
  }

  return undefined;
}

function getOpeningHoursFromError(hour: ExceptionalServiceHourModel): FieldError | undefined {
  if (hour.validityPeriod === 'Day') {
    if (!hour.openingHoursFrom) {
      return createValidationError('Ptv.Validation.Error.Field.Empty');
    }
  }

  if (hour.validityPeriod === 'BetweenDates') {
    const from = toOptionalDateTime(hour.openingHoursFrom);
    const to = toOptionalDateTime(hour.openingHoursTo);

    if (!from) {
      return createValidationError('Ptv.Validation.Error.Field.Empty');
    }

    if (to) {
      if (from > to) {
        return createValidationError('Ptv.Validation.Error.Common.StartTimeSmallerThanEndTime');
      }
    }
  }

  return undefined;
}

function getOpeningHoursToError(hour: ExceptionalServiceHourModel): FieldError | undefined {
  if (hour.validityPeriod === 'Day') return undefined;

  if (hour.validityPeriod === 'BetweenDates') {
    const from = toOptionalDateTime(hour.openingHoursFrom);
    const to = toOptionalDateTime(hour.openingHoursTo);
    if (!to) {
      return createValidationError('Ptv.Validation.Error.Field.Empty');
    }

    if (from) {
      if (from > to) {
        return createValidationError('Ptv.Validation.Error.Common.StartTimeSmallerThanEndTime');
      }
    }
  }

  return undefined;
}

function shouldValidate(input: ConnectionFormModel): boolean {
  return input.channelType === 'EChannel' || input.channelType === 'ServiceLocation' || input.channelType === 'Phone';
}
