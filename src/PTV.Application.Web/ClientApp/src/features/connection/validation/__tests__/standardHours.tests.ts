import { TimeRangeModel } from 'types/forms/connectionFormTypes';
import { doesTimeOverlapWithOtherTimes } from 'features/connection/validation/standardHours';
import { isStartTimeBeforeEndTime } from 'features/connection/validation/utils';

describe('isStartTimeBeforeEndTime', () => {
  it.each`
    from          | to            | expected | msg
    ${'00:00:00'} | ${'00:00:00'} | ${false} | ${'Midnight is not before midnight'}
    ${'08:00:00'} | ${'07:59:59'} | ${false} | ${'From is one second larger'}
    ${'07:59:59'} | ${'08:00:00'} | ${true}  | ${'From is one second smaller'}
    ${'00:00:01'} | ${'00:00:00'} | ${true}  | ${'From is before next midnight'}
    ${'01:00:00'} | ${'00:00:00'} | ${true}  | ${'From is before next midnight'}
    ${'05:00:00'} | ${'00:00:00'} | ${true}  | ${'From is before next midnight'}
    ${'23:59:59'} | ${'00:00:00'} | ${true}  | ${'From is before next midnight'}
  `('should return $expected when using $start - $end because $msg', ({ from, to, expected }) => {
    const time: TimeRangeModel = {
      from: from,
      to: to,
    };
    const result = isStartTimeBeforeEndTime(time.from, time.to);
    expect(result).toBe(expected);
  });
});

describe('doesTimeOverlapWithOtherTimes', () => {
  it.each`
    from          | to            | otherFrom     | otherTo       | expected | msg
    ${'08:00:00'} | ${'16:00:00'} | ${'07:00:00'} | ${'17:00:00'} | ${true}  | ${'Start time overlaps'}
    ${'08:00:00'} | ${'16:00:00'} | ${'09:00:00'} | ${'17:00:00'} | ${true}  | ${'Start time overlaps'}
    ${'08:00:00'} | ${'16:00:00'} | ${'08:15:00'} | ${'15:00:00'} | ${true}  | ${'Start and end between existing time'}
    ${'08:00:00'} | ${'16:00:00'} | ${'07:45:00'} | ${'15:00:00'} | ${true}  | ${'End time before existing end time'}
    ${'08:00:00'} | ${'16:00:00'} | ${'16:00:00'} | ${'18:00:00'} | ${true}  | ${'Starts same time as previous one ends'}
    ${'08:00:00'} | ${'16:00:00'} | ${'07:00:00'} | ${'08:00:00'} | ${true}  | ${'Ends same time as next one starts'}
    ${'08:00:00'} | ${'16:00:00'} | ${'07:00:00'} | ${'07:30:00'} | ${false} | ${'Before existing time'}
    ${'08:00:00'} | ${'16:00:00'} | ${'17:00:00'} | ${'18:00:00'} | ${false} | ${'After existing time'}
    ${'10:00:00'} | ${'18:00:00'} | ${'09:00:00'} | ${'17:00:00'} | ${true}  | ${'Inside existing time'}
    ${'10:00:00'} | ${'16:00:00'} | ${'09:00:00'} | ${'17:00:00'} | ${true}  | ${'Starts before existing time'}
    ${'08:00:00'} | ${'16:00:00'} | ${'09:00:00'} | ${'17:00:00'} | ${true}  | ${'Starts after existing time'}
    ${'08:00:00'} | ${'18:00:00'} | ${'09:00:00'} | ${'17:00:00'} | ${true}  | ${'Ends before existing time'}
    ${'00:30:00'} | ${'03:00:00'} | ${'15:00:00'} | ${'00:00:00'} | ${false} | ${'Two separate times'}
    ${'15:00:00'} | ${'00:00:00'} | ${'00:30:00'} | ${'03:00:00'} | ${false} | ${'Two separate times'}
    ${'14:00:00'} | ${'00:00:00'} | ${'00:30:00'} | ${'00:00:00'} | ${true}  | ${'Same end time'}
    ${'14:00:00'} | ${'23:45:00'} | ${'00:30:00'} | ${'00:00:00'} | ${true}  | ${'Time inside other time'}
    ${'00:30:00'} | ${'00:00:00'} | ${'14:00:00'} | ${'23:45:00'} | ${true}  | ${'Time inside other time'}
    ${'04:00:00'} | ${'00:00:00'} | ${'19:15:00'} | ${'00:00:00'} | ${true}  | ${'Both times end at midnight'}
    ${'19:15:00'} | ${'00:00:00'} | ${'04:00:00'} | ${'00:00:00'} | ${true}  | ${'Both times end at midnight'}
    ${'04:00:00'} | ${'23:59:59'} | ${'19:15:00'} | ${'00:00:00'} | ${true}  | ${'Time inside other time'}
  `(
    'should return $expected when checking if $from - $to overlaps with $otherFrom - $otherTo because $msg',
    ({ from, to, otherFrom, otherTo, expected }) => {
      const time: TimeRangeModel = {
        from: otherFrom,
        to: otherTo,
      };

      const times: TimeRangeModel[] = [{ from: from, to: to }, time];
      const result = doesTimeOverlapWithOtherTimes(times, time, 1);
      expect(result).toBe(expected);
    }
  );
});
