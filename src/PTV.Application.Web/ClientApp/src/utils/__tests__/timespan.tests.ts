import { TimeSpan, isMidnight, isMidnightRawValue } from 'utils/timespan';

describe('TimeSpan creation with valid input for ParseExact', () => {
  it.each`
    value         | hours | minutes | seconds
    ${'00:00:00'} | ${0}  | ${0}    | ${0}
    ${'01:02:03'} | ${1}  | ${2}    | ${3}
    ${'10:11:12'} | ${10} | ${11}   | ${12}
    ${'23:59:59'} | ${23} | ${59}   | ${59}
  `('should return $hours:$minutes:$seconds when $value is used', ({ value, hours, minutes, seconds }) => {
    const ts = TimeSpan.ParseExact(value);
    expect(ts.hours).toBe(hours);
    expect(ts.minutes).toBe(minutes);
    expect(ts.seconds).toBe(seconds);
  });
});

describe('TimeSpan creation with valid input values', () => {
  it.each`
    hours | minutes | seconds | totalSeconds
    ${0}  | ${0}    | ${0}    | ${0}
    ${1}  | ${2}    | ${3}    | ${3723}
    ${10} | ${11}   | ${12}   | ${36672}
    ${23} | ${59}   | ${59}   | ${86399}
  `('should succeed when using $hours:$minutes:$seconds', ({ hours, minutes, seconds, totalSeconds }) => {
    const ts = new TimeSpan(hours, minutes, seconds);
    expect(ts.hours).toBe(hours);
    expect(ts.minutes).toBe(minutes);
    expect(ts.seconds).toBe(seconds);
    expect(ts.totalSeconds).toBe(totalSeconds);
  });
});

describe('TimeSpan creation with invalid input values', () => {
  it.each`
    hours | minutes | seconds | msg
    ${-1} | ${0}    | ${0}    | ${'hours is too small'}
    ${24} | ${0}    | ${0}    | ${'hours is too large'}
    ${0}  | ${-1}   | ${0}    | ${'minutes is too small'}
    ${0}  | ${60}   | ${0}    | ${'minutes is too large'}
    ${0}  | ${0}    | ${-1}   | ${'seconds is too small'}
    ${0}  | ${0}    | ${60}   | ${'seconds is too large'}
  `('should fail when using $hours:$minutes:$seconds because $msg', ({ hours, minutes, seconds }) => {
    expect(() => new TimeSpan(hours, minutes, seconds)).toThrow();
  });
});

describe('TimeSpan creation with invalid input for ParseExact', () => {
  it.each`
    value         | msg
    ${''}         | ${'value is empty string'}
    ${' '}        | ${'value is string with spaces '}
    ${'00:00'}    | ${'value is missing seconds'}
    ${'00:00:'}   | ${'value is missing seconds with trailing semicolon'}
    ${'00'}       | ${'value is missing minutes and seconds'}
    ${'00:'}      | ${'value is missing minutes and seconds with trailing semicolon'}
    ${'24:00:00'} | ${'hours value is too large'}
    ${'-1:00:00'} | ${'hours value is too small'}
    ${'00:60:00'} | ${'minutes value is too large'}
    ${'00:-1:00'} | ${'minutes value is too small'}
    ${'00:00:60'} | ${'seconds value is too large'}
    ${'00:00:-1'} | ${'seconds value is too small'}
    ${'aa:00:00'} | ${'hours contain characters'}
    ${'00:1a:00'} | ${'minutes contain characters'}
    ${'00:00:1a'} | ${'seconds contain characters'}
  `('should fail because $msg', ({ value }) => {
    expect(() => TimeSpan.ParseExact(value)).toThrow();
  });
});

describe('toString()', () => {
  it.each`
    value
    ${'00:00:00'}
    ${'01:02:03'}
    ${'10:11:12'}
    ${'23:59:59'}
  `('should return $value', ({ value }) => {
    const ts = TimeSpan.ParseExact(value);
    const str = ts.toString();
    expect(str).toBe(value);
  });
});

describe('TimeSpan smallerThan', () => {
  it.each`
    left          | right         | expected
    ${'00:00:00'} | ${'00:00:00'} | ${false}
    ${'23:59:59'} | ${'23:59:59'} | ${false}
    ${'00:00:01'} | ${'00:00:02'} | ${true}
    ${'00:01:00'} | ${'00:02:00'} | ${true}
    ${'01:00:00'} | ${'02:00:00'} | ${true}
    ${'15:10:02'} | ${'15:10:03'} | ${true}
    ${'15:11:59'} | ${'15:12:30'} | ${true}
    ${'15:12:30'} | ${'15:11:59'} | ${false}
    ${'17:00:00'} | ${'16:00:00'} | ${false}
    ${'17:02:00'} | ${'17:01:00'} | ${false}
    ${'17:01:02'} | ${'17:01:01'} | ${false}
    ${'18:20:59'} | ${'18:19:45'} | ${false}
  `('should return $expected when comparing if $left is smaller than $right', ({ left, right, expected }) => {
    const l = TimeSpan.ParseExact(left);
    const r = TimeSpan.ParseExact(right);
    const result = l.smallerThan(r);
    expect(result).toBe(expected);
  });
});

describe('TimeSpan largerThan', () => {
  it.each`
    left          | right         | expected
    ${'00:00:00'} | ${'00:00:00'} | ${false}
    ${'23:59:59'} | ${'23:59:59'} | ${false}
    ${'00:00:01'} | ${'00:00:02'} | ${false}
    ${'00:01:00'} | ${'00:02:00'} | ${false}
    ${'01:00:00'} | ${'02:00:00'} | ${false}
    ${'15:10:02'} | ${'15:10:03'} | ${false}
    ${'15:11:59'} | ${'15:12:30'} | ${false}
    ${'17:00:00'} | ${'16:00:00'} | ${true}
    ${'17:02:00'} | ${'17:01:00'} | ${true}
    ${'17:01:02'} | ${'17:01:01'} | ${true}
    ${'18:20:59'} | ${'18:19:45'} | ${true}
  `('should return $expected when comparing if $left is larger than $right', ({ left, right, expected }) => {
    const l = TimeSpan.ParseExact(left);
    const r = TimeSpan.ParseExact(right);
    const result = l.largerThan(r);
    expect(result).toBe(expected);
  });
});

describe('TimeSpan compareTo', () => {
  it.each`
    left          | right         | expected
    ${'00:00:00'} | ${'00:00:00'} | ${0}
    ${'23:59:59'} | ${'23:59:59'} | ${0}
    ${'00:00:01'} | ${'00:00:02'} | ${-1}
    ${'00:01:00'} | ${'00:02:00'} | ${-1}
    ${'01:00:00'} | ${'02:00:00'} | ${-1}
    ${'15:10:02'} | ${'15:10:03'} | ${-1}
    ${'15:11:59'} | ${'15:12:30'} | ${-1}
    ${'15:12:30'} | ${'15:11:59'} | ${1}
    ${'17:00:00'} | ${'16:00:00'} | ${1}
    ${'17:02:00'} | ${'17:01:00'} | ${1}
    ${'17:01:02'} | ${'17:01:01'} | ${1}
    ${'18:20:59'} | ${'18:19:45'} | ${1}
  `('should return $expected when $left comparedTo $right', ({ left, right, expected }) => {
    const l = TimeSpan.ParseExact(left);
    const r = TimeSpan.ParseExact(right);
    const result = l.compareTo(r);
    expect(result).toBe(expected);
  });
});

describe('TimeSpan.toString()', () => {
  it.each`
    value         | hours | minutes | seconds
    ${'00:00:00'} | ${0}  | ${0}    | ${0}
    ${'01:02:03'} | ${1}  | ${2}    | ${3}
    ${'10:11:12'} | ${10} | ${11}   | ${12}
    ${'23:59:59'} | ${23} | ${59}   | ${59}
  `('should return $value when $hours:$minutes:$seconds is used', ({ value, hours, minutes, seconds }) => {
    const str = TimeSpan.toString(hours, minutes, seconds);
    expect(str).toBe(value);
  });
});

describe('isMidnight', () => {
  it.each`
    value         | expectedValue
    ${'00:00:00'} | ${true}
    ${'00:00:01'} | ${false}
    ${'01:00:00'} | ${false}
    ${'00:01:00'} | ${false}
    ${'23:59:59'} | ${false}
  `('should return $expectedValue for $value ', ({ expectedValue, value }) => {
    const result = isMidnight(TimeSpan.ParseExact(value));
    expect(result).toBe(expectedValue);
  });
});

describe('isMidnightRawValue', () => {
  it.each`
    value         | expectedValue
    ${'00:00:00'} | ${true}
    ${'00:00:01'} | ${false}
    ${'01:00:00'} | ${false}
    ${'00:01:00'} | ${false}
    ${'23:59:59'} | ${false}
  `('should return $expectedValue for $value ', ({ expectedValue, value }) => {
    const result = isMidnightRawValue(value);
    expect(result).toBe(expectedValue);
  });
});
