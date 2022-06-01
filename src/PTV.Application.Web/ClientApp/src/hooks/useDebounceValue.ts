import { DependencyList, useEffect, useState } from 'react';

const useDebounceValue = <T extends number | string>(value: T, delay: number, deps?: DependencyList): T => {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, delay, deps]);

  return debouncedValue;
};

export { useDebounceValue };
