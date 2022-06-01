import { createContext } from 'react';
import { QualityResponse, SkippedIssue } from 'types/qualityAgentResponses';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type Payload = any | null;

export type Action =
  | { type: 'QualityChecked'; payload: QualityResponse }
  | { type: 'QualityCheckLoading'; payload: boolean }
  | { type: 'QualityCheckIssueSkipped'; payload: SkippedIssue };

export type DispatchQualityAgent = (action: Action) => void;

// eslint-disable-next-line @typescript-eslint/no-empty-function
export const DispatchQualityAgentContext = createContext<DispatchQualityAgent>(() => {});
