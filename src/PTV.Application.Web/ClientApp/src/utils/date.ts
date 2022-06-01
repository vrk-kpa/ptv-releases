import { DateTime } from 'luxon';
import { Weekday, weekDayType } from 'types/enumTypes';

export function toDateAndTime(value: string): string {
  return DateTime.fromISO(value, { zone: 'utc' }).toLocal().setLocale('fi-FI').toFormat('dd.LL.yyyy HH:mm');
}

export function toTime(value: string): string {
  return fromTimeZoneToTime('utc', value);
}

export function toTimeNoTimeZone(value: string): string {
  return fromTimeZoneToTime(undefined, value);
}

export function fromTimeZoneToTime(timeZone: string | undefined, value: string): string {
  return DateTime.fromISO(value, { zone: timeZone }).toLocal().setLocale('fi-FI').toFormat('HH:mm');
}

export function toDateNoTimeZone(value: string): string {
  return fromTimeZoneToDate(undefined, value);
}

export function toDate(value: string): string {
  return fromTimeZoneToDate('utc', value);
}

export function fromTimeZoneToDate(timeZone: string | undefined, value: string): string {
  return DateTime.fromISO(value, { zone: timeZone }).toLocal().setLocale('fi-FI').toFormat('dd.LL.yyyy');
}

export function toOptionalDateTime(value?: string | null | undefined): DateTime | null {
  if (!value) return null;
  return DateTime.fromISO(value, { zone: 'utc' }).toLocal();
}

export function toLocalDateTime(value: string): DateTime {
  return DateTime.fromISO(value, { zone: 'utc' }).toLocal();
}

export function displayDateTime(value: DateTime): string {
  return value.setLocale('fi-FI').toFormat('dd.LL.yyyy HH:mm');
}

export function displayDate(value: DateTime | null | undefined): string {
  if (!value) {
    return '';
  }

  return value.setLocale('fi-FI').toFormat('dd.LL.yyyy');
}

export function utcNowISO(): string {
  return DateTime.utc().toISO();
}

export function toWeekDay(date: DateTime): Weekday {
  return weekDayType[date.toUTC().weekday - 1];
}

export function compareWeekDays(left: Weekday, right: Weekday): number {
  const l = weekDayType.indexOf(left);
  const r = weekDayType.indexOf(right);
  if (l === r) return 0;
  return l < r ? -1 : 1;
}
