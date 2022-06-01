import { useContext } from 'react';
import { FormMetaContext, IFormMetaContext } from './FormMetaContext';

export function useFormMetaContext(): IFormMetaContext {
  const ctx = useContext(FormMetaContext);
  if (!ctx) {
    throw Error('FormMetaContext has not been provided.');
  }

  return ctx;
}
