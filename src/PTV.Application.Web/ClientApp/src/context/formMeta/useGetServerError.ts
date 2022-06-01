import { ProblemDetails } from 'types/miscellaneousTypes';
import { useFormMetaContext } from './';

export function useGetServerError(): ProblemDetails | undefined {
  const ctx = useFormMetaContext();
  return ctx.serverError;
}
