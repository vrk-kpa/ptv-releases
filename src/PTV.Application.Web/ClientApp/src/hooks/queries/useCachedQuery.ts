import { UseQueryOptions, useQuery } from 'react-query';
import { HttpError } from 'types/miscellaneousTypes';
import { useLocalStorage } from 'hooks/useLocalStorage';
import { get } from 'utils/request';

const defaultStaleTime = 2 * 60 * 60 * 1000; // 2 hours

type CachedQueryProps<T> = {
  path: string;
  staleTime?: number | null | undefined;
  forceRefresh?: boolean | null | undefined;
  options?: UseQueryOptions<T, HttpError, T> | undefined;
};

type CachedQueryResult<T> = {
  data: T | undefined;
  isLoading: boolean;
  error: HttpError | null;
};

type CachedObject<T> = {
  data: T;
  refreshTime: number;
};

export function useCachedQuery<T>(props: CachedQueryProps<T>): CachedQueryResult<T> {
  const [isAvailable, setItem, getItem] = useLocalStorage();

  const now = new Date().getTime();
  const lowestAllowedTime = now - (props.staleTime ?? defaultStaleTime);
  const item = isAvailable ? getItem(props.path) : null;
  const result = (item && (JSON.parse(item) as CachedObject<T> | null)) || null;
  const refresh = !result || result.refreshTime < lowestAllowedTime || !!props.forceRefresh || !isAvailable;

  const query = useQuery<T, HttpError>(props.path, () => get<T>(props.path), { ...props.options, enabled: refresh });

  if (query.isSuccess) {
    const newData = { data: query.data, refreshTime: now };
    if (isAvailable) {
      setItem(props.path, JSON.stringify(newData));
    }
    return query;
  }

  return { data: result?.data, isLoading: false, error: null };
}
