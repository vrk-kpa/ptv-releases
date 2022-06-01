import { FieldError, FieldErrors } from 'react-hook-form';
import { ConnectionFormModel, DailyOpeningTimeModel, StandardServiceHourModel, TimeRangeModel } from 'types/forms/connectionFormTypes';
import { toOptionalDateTime } from 'utils/date';
import { containsErrors, createValidationError } from 'utils/rhf';
import { TimeSpan, isMidnight, isMidnightRawValue } from 'utils/timespan';
import { purgeEmptyErrors } from 'validation/rhf';
import { validateLanguageVersions } from './serviceHours';
import { isStartTimeBeforeEndTime } from './utils';

export const MaxDailyOpeningTimes = 2;

export function validateStandardHours(input: ConnectionFormModel): FieldErrors<StandardServiceHourModel>[] | undefined {
  if (!shouldValidate(input)) return undefined;

  const hours = input.openingHours.standardHours;

  // errors Array must have same lenght otherwise errors are displayed for wrong items in the UI.
  const errors: FieldErrors<StandardServiceHourModel>[] = new Array(hours.length);

  for (let index = 0; index < hours.length; index++) {
    const hour = hours[index];

    const validationResult = validateStandardHour(hour);
    if (validationResult) {
      errors[index] = validationResult;
    }
  }

  return containsErrors(errors) ? errors : undefined;
}

function validateStandardHour(hour: StandardServiceHourModel): FieldErrors<StandardServiceHourModel> | undefined {
  const errors: FieldErrors<StandardServiceHourModel> = {};
  errors.openingType = hour.openingType
    ? undefined
    : createValidationError('Ptv.ConnectionDetails.StandardServiceHour.Validity.OpeningType.Missing');
  errors.dailyOpeningTimes = validateDailyOpeningTimes(hour);
  errors.openingHoursFrom = validateOpeningHourFromAndTo(hour);
  errors.languageVersions = validateLanguageVersions(hour.languageVersions);
  return purgeEmptyErrors(errors);
}

function validateOpeningHourFromAndTo(hour: StandardServiceHourModel): FieldError | undefined {
  if (hour.validityPeriod !== 'BetweenDates') return undefined;

  const from = toOptionalDateTime(hour.openingHoursFrom);
  const to = toOptionalDateTime(hour.openingHoursTo);

  if (!from || !to) return undefined;
  if (from < to) return undefined;

  return createValidationError('Ptv.Validation.Error.Common.StartTimeSmallerThanEndTime');
}

function validateDailyOpeningTimes(hour: StandardServiceHourModel): FieldErrors<DailyOpeningTimeModel>[] | undefined {
  const dailyOpeningTimes = hour.dailyOpeningTimes;
  const errors: FieldErrors<DailyOpeningTimeModel>[] = new Array(dailyOpeningTimes.length);

  if (hour.openingType !== 'DaysAndTimes') return undefined;

  for (let index = 0; index < dailyOpeningTimes.length; index++) {
    const dailyOpeningTime = dailyOpeningTimes[index];

    if (!dailyOpeningTime.active || dailyOpeningTime.times.length === 0) {
      continue;
    }

    const error: FieldErrors<DailyOpeningTimeModel> = {};

    const tooManyOpeningTimes = dailyOpeningTime.times.length > MaxDailyOpeningTimes;
    const timeErrors = validateTimeRanges(dailyOpeningTime.times);

    if (containsErrors(timeErrors)) error.times = timeErrors;
    if (tooManyOpeningTimes) {
      error.day = createValidationError('Ptv.ConnectionDetails.StandardServiceHour.Validation.Error.TooManyOpeningTimes');
    }

    if (error.day || error.times) {
      errors[index] = error;
    }
  }

  return containsErrors(errors) ? errors : undefined;
}

function validateTimeRanges(times: TimeRangeModel[]): FieldErrors<TimeRangeModel>[] {
  const errors: FieldErrors<TimeRangeModel>[] = new Array(times.length);

  for (let timeIndex = 0; timeIndex < times.length; timeIndex++) {
    const time = times[timeIndex];

    // Normally start time must be before end time but UI allows user to
    // set 24h time (00:00 - 24:00) so allow that
    if (!isMidnightRawValue(time.from) && !isMidnightRawValue(time.to)) {
      if (!isStartTimeBeforeEndTime(time.from, time.to)) {
        errors[timeIndex] = {
          from: createValidationError('Ptv.Validation.Error.Common.StartTimeSmallerThanEndTime'),
        };
      }
    }

    if (doesTimeOverlapWithOtherTimes(times, time, timeIndex)) {
      errors[timeIndex] = {
        from: createValidationError('Ptv.ConnectionDetails.StandardServiceHour.Validation.Error.OverlappingTime'),
      };
    }
  }

  return errors;
}

export function doesTimeOverlapWithOtherTimes(times: TimeRangeModel[], time: TimeRangeModel, timeIndex: number): boolean {
  const astart = TimeSpan.ParseExact(time.from);
  const aend = TimeSpan.ParseExact(time.to);

  for (let index = 0; index < times.length; index++) {
    // Do not validate against myself
    if (index === timeIndex) continue;

    const bstart = TimeSpan.ParseExact(times[index].from);
    const bend = TimeSpan.ParseExact(times[index].to);

    // This check is due to a fact that UI allows to specify 00:00 as start and end time
    if (isMidnight(aend)) {
      // If a time ends at midnight then other time must start and end before a time starts
      if (!startsAndEndsBefore(bstart, bend, astart)) {
        return true;
      }
    }

    // This check is due to a fact that UI allows to specify 00:00 as start and end time
    if (isMidnight(bend)) {
      // If b time ends at midnight then other time must start and end before b time starts
      if (!startsAndEndsBefore(astart, aend, bstart)) {
        return true;
      }
    }

    const overlap = astart.compareTo(bend) <= 0 && bstart.compareTo(aend) <= 0;
    if (overlap) {
      return true;
    }
  }

  return false;
}

function startsAndEndsBefore(start: TimeSpan, end: TimeSpan, before: TimeSpan): boolean {
  return start.compareTo(before) < 0 && end.compareTo(before) < 0;
}

function shouldValidate(input: ConnectionFormModel): boolean {
  return input.channelType === 'EChannel' || input.channelType === 'ServiceLocation' || input.channelType === 'Phone';
}
