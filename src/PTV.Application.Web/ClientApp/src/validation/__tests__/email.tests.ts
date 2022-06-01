import { looksLikeValidEmail } from 'validation/email';

describe('looksLikeValidEmail', () => {
  it.each`
    value                            | expectedValue
    ${''}                            | ${false}
    ${'  '}                          | ${false}
    ${'first'}                       | ${false}
    ${'first@'}                      | ${false}
    ${'first.lastname'}              | ${false}
    ${'first.lastname@'}             | ${false}
    ${'@domain.com'}                 | ${false}
    ${'@'}                           | ${false}
    ${'first.lastname@domain.com'}   | ${true}
    ${'first.lastname@domain.co.uk'} | ${true}
    ${'first.last.name@domain.com'}  | ${true}
    ${'first.last+name@domain.com'}  | ${true}
  `('should return: $expectedValue for email: $value', ({ value, expectedValue }) => {
    const result = looksLikeValidEmail(value);
    expect(result).toBe(expectedValue);
  });
});
