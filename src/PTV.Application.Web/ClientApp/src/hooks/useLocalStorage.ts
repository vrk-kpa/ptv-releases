import { useState } from 'react';
import { localStorageAvailable } from 'utils/localStorage';

export const useLocalStorage = (): [boolean, (path: string, data: string) => void, (path: string) => string | null] => {
  const [isAvailable, setIsAvailable] = useState<boolean>(!!localStorageAvailable());

  const setItem = (path: string, data: string) => {
    try {
      localStorage.setItem(path, data);
    } catch (error) {
      console.error(error);
      setIsAvailable(false);
    }
  };

  const getItem = (path: string): string | null => {
    try {
      return localStorage.getItem(path);
    } catch (error) {
      console.error(error);
      setIsAvailable(false);
      return null;
    }
  };

  return [isAvailable, setItem, getItem];
};
