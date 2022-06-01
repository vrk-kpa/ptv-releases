import { Draft } from 'immer';
import { QualityResponse, SkippedIssue } from 'types/qualityAgentResponses';
import { assertUnreachable } from 'utils/reducer';
import { Action } from './dispatchQualityAgentContext';
import { IQualityAgentContext } from './qualityAgentContext';

export function qualityAgentContextReducer(draft: Draft<IQualityAgentContext>, action: Action): void {
  switch (action.type) {
    case 'QualityChecked':
      setQualityResult(draft, action.payload);
      break;
    case 'QualityCheckLoading':
      setQualityCheckLoading(draft, action.payload);
      break;
    case 'QualityCheckIssueSkipped':
      addSkippedIssue(draft, action.payload);
      break;
    default:
      assertUnreachable(action as never);
  }
}

function setQualityCheckLoading(draft: Draft<IQualityAgentContext>, payload: boolean): void {
  draft.loading = payload;
}

function setQualityResult(draft: Draft<IQualityAgentContext>, payload: QualityResponse): void {
  if (!!payload.error) {
    draft.qualityIssues = [];
    return;
  }

  draft.qualityIssues = payload.result;
}

function addSkippedIssue(draft: Draft<IQualityAgentContext>, payload: SkippedIssue): void {
  if (payload) {
    draft.skippedIssues.push(payload);
  }
}
