import { useEffect, useState } from 'react';

export const useDefaultValue = <T>(value: T): T | null => {
  const [defaultValue, setDefaultValue] = useState<T | null>(null);

  useEffect(() => {
    if (!defaultValue) setDefaultValue(value);
  }, [defaultValue, value]);
  return defaultValue;
};
