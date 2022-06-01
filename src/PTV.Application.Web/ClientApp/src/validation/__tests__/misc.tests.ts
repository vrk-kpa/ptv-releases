import { containsOnlyNumbers } from 'validation/misc';

describe('containsOnlyNumbers', () => {
  it.each`
    value     | expectedValue
    ${'0'}    | ${true}
    ${'1'}    | ${true}
    ${'01'}   | ${true}
    ${'10'}   | ${true}
    ${'101'}  | ${true}
    ${' 101'} | ${false}
    ${'101 '} | ${false}
    ${'1 2'}  | ${false}
    ${''}     | ${false}
    ${'  '}   | ${false}
  `('should return: $expectedValue for: $value', ({ value, expectedValue }) => {
    const result = containsOnlyNumbers(value);
    expect(result).toBe(expectedValue);
  });
});
