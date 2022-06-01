import { DialCode } from 'types/enumItemType';

export function getDialCodeOrDefault(id: string | null | undefined, dialCodes: DialCode[]): DialCode | null | undefined {
  if (!id) return null;
  return dialCodes.find((x) => x.id === id);
}

export function getFinnishDialCode(dialCodes: DialCode[]): DialCode | null | undefined {
  return dialCodes.find((x) => x.code === '+358');
}
