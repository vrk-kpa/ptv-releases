import { SkippedIssue } from 'types/qualityAgentResponses';
import { useQualityAgentContext } from '.';

export function useGetSkippedIssues(): SkippedIssue[] {
  const ctx = useQualityAgentContext();
  return ctx.skippedIssues;
}
