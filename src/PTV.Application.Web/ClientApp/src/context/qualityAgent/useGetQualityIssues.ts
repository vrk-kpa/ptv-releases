import { QualityResult } from 'types/qualityAgentResponses';
import { useQualityAgentContext } from '.';

export function useGetQualityIssues(): QualityResult[] {
  const ctx = useQualityAgentContext();
  return ctx.qualityIssues;
}
