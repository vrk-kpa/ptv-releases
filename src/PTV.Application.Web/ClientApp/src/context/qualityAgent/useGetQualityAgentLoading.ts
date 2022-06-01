import { useQualityAgentContext } from './';

export function useGetQualityAgentLoading(): boolean {
  const ctx = useQualityAgentContext();
  return ctx.loading;
}
