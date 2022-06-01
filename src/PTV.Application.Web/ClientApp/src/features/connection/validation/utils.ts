import { TimeSpan, isMidnight, isMidnightRawValue } from 'utils/timespan';

export function isStartTimeBeforeEndTime(start: string, end: string): boolean {
  const from = TimeSpan.ParseExact(start);
  const to = TimeSpan.ParseExact(end);

  // The UI uses 00:00 as first and last time of the day even though
  // it is the exact same time. If time range ends at midnight then
  // any from-time that is before midnight is valid.
  if (isMidnight(to)) {
    return !isMidnight(from);
  }

  return from.smallerThan(to);
}

export function bothAreMidnight(from: string, to: string): boolean {
  return isMidnightRawValue(from) && isMidnightRawValue(to);
}
