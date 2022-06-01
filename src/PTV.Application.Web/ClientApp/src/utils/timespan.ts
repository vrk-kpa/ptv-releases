import { padStart } from 'utils';

// Format must be hh:mm:ss e.g. 01:27:59
const ParseValues = /^(\d{2})(?::)(\d{2})(?::)(\d{2})$/;

export class TimeSpan {
  readonly hours: number;
  readonly minutes: number;
  readonly seconds: number;
  readonly totalSeconds: number;

  constructor(hours: number, minutes: number, seconds: number) {
    this.validate(hours, minutes, seconds);
    this.hours = hours;
    this.minutes = minutes;
    this.seconds = seconds;
    this.totalSeconds = hours * 3600 + minutes * 60 + seconds;
  }

  public toString(): string {
    return TimeSpan.toString(this.hours, this.minutes, this.seconds);
  }

  public compareTo(other: TimeSpan): number {
    if (this.smallerThan(other)) return -1;
    if (this.largerThan(other)) return 1;
    return 0;
  }

  public smallerThan(ts: TimeSpan): boolean {
    return this.totalSeconds < ts.totalSeconds;
  }

  public largerThan(ts: TimeSpan): boolean {
    return this.totalSeconds > ts.totalSeconds;
  }

  public static toString(hours: number, minutes: number, seconds: number): string {
    return `${padValue(hours)}:${padValue(minutes)}:${padValue(seconds)}`;
  }

  public static ParseExact(value: string): TimeSpan {
    return parseOrThrow(value);
  }

  private validate(hours: number, minutes: number, seconds: number) {
    this.validateOrThrow(hours, 0, 23, 'hours');
    this.validateOrThrow(minutes, 0, 59, 'minutes');
    this.validateOrThrow(seconds, 0, 59, 'seconds');
  }

  private validateOrThrow(value: number, min: number, max: number, fieldName: string): void {
    if (value < min || value > max) {
      throw Error(`TimeSpan is invalid. Value ${value} for ${fieldName} is not between ${min} and ${max}`);
    }
  }
}

function parseOrThrow(value: string): TimeSpan {
  if (!value) {
    throw Error('Cannot create TimeSpan from null/undefined/empty value');
  }

  const re = new RegExp(ParseValues);
  const result = re.exec(value);
  if (!result || result.length !== 4) {
    throw Error(`TimeSpan ${value} is invalid. It must be in format hh:mm:ss e.g. 03:27:00`);
  }

  const hours = parseInt(result[1], 10);
  const minutes = parseInt(result[2], 10);
  const seconds = parseInt(result[3], 10);

  return new TimeSpan(hours, minutes, seconds);
}

function padValue(value: number): string {
  return padStart(value, 2, '0');
}

export function isMidnight(ts: TimeSpan): boolean {
  return ts.hours === 0 && ts.minutes === 0 && ts.seconds === 0;
}

export function isMidnightRawValue(value: string): boolean {
  return isMidnight(TimeSpan.ParseExact(value));
}

export const createDefaultStartTime = () => new TimeSpan(8, 0, 0);
export const createDefaultEndTime = () => new TimeSpan(16, 0, 0);
