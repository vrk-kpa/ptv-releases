import { RefObject, useRef } from 'react';

export function useRefOrCreate<T>(ref?: RefObject<T>): RefObject<T> | undefined {
  const newRef = useRef<T>(null);
  if (ref) return ref;
  return newRef;
}
