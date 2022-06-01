export function itemToArray<T>(item: T | null | undefined): T[] {
  if (!item) {
    return [];
  }

  return [item];
}
