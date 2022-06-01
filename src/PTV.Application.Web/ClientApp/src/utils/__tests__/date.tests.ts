import { DateTime } from 'luxon';
import { compareWeekDays, toWeekDay } from 'utils/date';

describe('toWeekDay', () => {
  it.each`
    date                               | day
    ${'2021-08-02T00:00:00.000+00:00'} | ${'Monday'}
    ${'2021-08-03T00:00:00.000+00:00'} | ${'Tuesday'}
    ${'2021-08-04T00:00:00.000+00:00'} | ${'Wednesday'}
    ${'2021-08-05T00:00:00.000+00:00'} | ${'Thursday'}
    ${'2021-08-06T00:00:00.000+00:00'} | ${'Friday'}
    ${'2021-08-07T00:00:00.000+00:00'} | ${'Saturday'}
    ${'2021-08-08T00:00:00.000+00:00'} | ${'Sunday'}
  `('should return $day for $date', ({ date, day }) => {
    const result = toWeekDay(DateTime.fromISO(date));
    expect(result).toBe(day);
  });
});

describe('compareWeekDays', () => {
  it.each`
    left           | right          | expected
    ${'Monday'}    | ${'Tuesday'}   | ${-1}
    ${'Tuesday'}   | ${'Wednesday'} | ${-1}
    ${'Wednesday'} | ${'Thursday'}  | ${-1}
    ${'Thursday'}  | ${'Friday'}    | ${-1}
    ${'Friday'}    | ${'Saturday'}  | ${-1}
    ${'Saturnday'} | ${'Sunday'}    | ${-1}
    ${'Sunday'}    | ${'Sunday'}    | ${0}
    ${'Sunday'}    | ${'Monday'}    | ${1}
  `('should return $expected when comparing $left with $right', ({ left, right, expected }) => {
    const result = compareWeekDays(left, right);
    expect(result).toBe(expected);
  });
});
