import { useContext } from 'react';
import { AppContext, IAppContext } from './AppContextProvider';

export function useAppContextOrThrow(): IAppContext {
  const ctx = useContext(AppContext);
  if (!ctx) {
    throw Error('AppContext has not been provided');
  }

  return ctx;
}
