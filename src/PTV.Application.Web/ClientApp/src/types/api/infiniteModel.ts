export type InfiniteModel<T> = {
  page: number;
  isMoreAvailable: boolean;
  data: T[];
};
