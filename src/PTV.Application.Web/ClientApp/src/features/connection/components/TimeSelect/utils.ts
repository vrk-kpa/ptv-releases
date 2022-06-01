import { padStart } from 'utils';
import { TimeSpan, isMidnight } from 'utils/timespan';

export type TimeSelectorType = 'StartTime' | 'EndTime';
const EndTimeMidnight = '24.00';

const Minutes = [0, 15, 30, 45];

export function getDisplayValue(type: TimeSelectorType, ts: TimeSpan): string {
  if (isMidnight(ts) && type === 'EndTime') return EndTimeMidnight;
  return `${padStart(ts.hours, 2, '0')}.${padStart(ts.minutes, 2, '0')}`;
}

export function generateTimespans(type: TimeSelectorType): TimeSpan[] {
  const timespans = generate();

  // StartTime can be between 00:00 - 23:45
  if (type === 'StartTime') return timespans;

  // For EndTime we want to display 00:15 - 00:00 so midnight is
  // actually the last value
  const min = new TimeSpan(0, 15, 0);
  const filtered = timespans.filter((x) => x.compareTo(min) >= 0);
  filtered.push(new TimeSpan(0, 0, 0));
  return filtered;
}

function generate(): TimeSpan[] {
  const values: TimeSpan[] = [];

  for (let hour = 0; hour < 24; hour++) {
    for (const min of Minutes) {
      values.push(new TimeSpan(hour, min, 0));
    }
  }

  return values;
}
