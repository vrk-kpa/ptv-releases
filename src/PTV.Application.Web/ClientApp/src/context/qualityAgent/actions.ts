import { QualityResponse, SkippedIssue } from 'types/qualityAgentResponses';
import { DispatchQualityAgent } from './dispatchQualityAgentContext';

function qualityChecked(dispatch: DispatchQualityAgent, data: QualityResponse): void {
  dispatch({
    type: 'QualityChecked',
    payload: data,
  });
}

function qualityCheckLoading(dispatch: DispatchQualityAgent, data: boolean): void {
  dispatch({
    type: 'QualityCheckLoading',
    payload: data,
  });
}

function issueSkipped(dispatch: DispatchQualityAgent, data: SkippedIssue): void {
  dispatch({
    type: 'QualityCheckIssueSkipped',
    payload: data,
  });
}

export { qualityChecked, qualityCheckLoading, issueSkipped };
