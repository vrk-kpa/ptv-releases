export function getPageCount(totalCount: number, itemsPerPage: number): number {
  return Math.ceil(totalCount / itemsPerPage);
}

export function getItemsForPage<T>(allItems: T[], currentPageNumber: number, itemsPerPage: number): T[] {
  const start = currentPageNumber === 1 ? 0 : (currentPageNumber - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  return allItems.slice(start, end);
}
