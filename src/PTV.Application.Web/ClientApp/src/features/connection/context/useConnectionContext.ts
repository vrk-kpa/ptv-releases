import { useContext } from 'react';
import { ConnectionContext, IConnectionContext } from 'features/connection/context/ConnectionContextProvider';

export function useConnectionContext(): IConnectionContext {
  const ctx = useContext(ConnectionContext);
  if (!ctx) {
    throw Error('ConnectionContext has not been provided');
  }

  return ctx;
}
