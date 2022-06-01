type QualityResultProcessed = {
  Data: string;
  Path: string[];
};

export type QualityMatch = {
  explanation: string;
  length: number;
  start: number;
  status: string;
  suggestions: string[] | null;
  title: string;
  token: string;
};

type QualityInfo = {
  message: string;
  matches: QualityMatch[];
};

type QualitySubresult = {
  passed: boolean;
  info: QualityInfo | null | undefined;
};

export type QualityResult = {
  explanation: string;
  fieldId: string;
  processed: QualityResultProcessed;
  result: QualitySubresult;
  rule: string;
  ruleId: string;
};

export type GuidedCorrectionItem = {
  rule: string;
  ruleId: string;
  issueData: string;
  issueExplanataion: string;
  matchTitle: string;
  matchToken: string;
  matchExplanation: string;
  matchSuggestions: string[];
  matchStart: number;
  matchLength: number;
  matchSentence: string;
  fieldId: string;
};

export type SkippedIssue = {
  token: string;
  ruleId: string;
  fieldId: string;
};

export type QualityResponse = {
  result: QualityResult[];
  error: { message: string };
};
