import { DependencyList, useState } from 'react';
import { useDeepCompareEffect } from 'react-use';

const useDebounceObject = <T>(value: T, delay: number, deps?: DependencyList): T => {
  const [debouncedValue, setDebouncedValue] = useState(value);

  useDeepCompareEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, delay, deps]);

  return debouncedValue;
};

export { useDebounceObject };
