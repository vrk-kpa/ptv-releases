import { QueryClient } from 'react-query';
import { HttpError } from 'types/miscellaneousTypes';

const MaxFailureCount = 2; // Max 3 failures because first time failureCount is zero

function shouldRetry(failureCount: number, error: unknown): boolean {
  if (error instanceof HttpError) {
    if (!error.isRetryableError()) {
      return false;
    }
  }

  return failureCount < MaxFailureCount;
}

export const createQueryClient = (): QueryClient => {
  return new QueryClient({
    defaultOptions: {
      queries: {
        refetchOnMount: false,
        refetchOnWindowFocus: false,
        refetchOnReconnect: false,
        retry: shouldRetry,
      },
    },
  });
};
