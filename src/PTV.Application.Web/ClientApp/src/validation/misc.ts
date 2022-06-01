const ValidNumber = /^[0-9]*$/;

export function containsOnlyNumbers(value: string): boolean {
  if (!value) return false; // test() with empty returns true
  return new RegExp(ValidNumber).test(value);
}
