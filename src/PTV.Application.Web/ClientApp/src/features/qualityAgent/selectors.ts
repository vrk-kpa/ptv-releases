import { DescriptionType, DomainEnumType, Language } from 'types/enumTypes';
import { QualityMatch, QualityResult, SkippedIssue } from 'types/qualityAgentResponses';
import { lingsoftRule } from 'types/qualityAgentRuleId';

export const filterRelatedIssues = (
  allIssues: QualityResult[] | null | undefined,
  selector: string,
  skippedIssues: SkippedIssue[]
): QualityResult[] => {
  const filtered = (allIssues ?? []).filter((x) => x.fieldId.includes(selector) && !x.result.passed);
  return filterSkippedIssues(filtered, skippedIssues);
};

export const getSubIssues = (issue: QualityResult): QualityMatch[] => {
  return issue.result.info?.matches?.filter((x) => !!x.explanation) ?? [];
};

export const getSpecialIssueTypes = (allIssues: QualityResult[]): QualityResult[] => {
  return allIssues.filter((issue) => lingsoftRule.find((ruleId) => ruleId === issue.ruleId));
};

export const getDescriptionSelector = (formType: DomainEnumType, descriptionType: DescriptionType, language: Language): string => {
  switch (formType) {
    case 'Services':
      return `serviceDescriptions.${descriptionType}.${language}`;
    case 'Relations':
      return `connectionDescriptions.${descriptionType}.${language}`;
    default:
      return '';
  }
};

// Filters out issues that user has skipped by using the GuidedLanguageCorrection component
const filterSkippedIssues = (qualityResults: QualityResult[], skippedIssues: SkippedIssue[]) => {
  const result: QualityResult[] = [];

  qualityResults.forEach((issue) => {
    if (issue.result.info?.matches) {
      const matches = issue.result.info.matches.filter(
        (match) =>
          !skippedIssues.some(
            (skippedIssue) =>
              skippedIssue.fieldId === issue.fieldId && skippedIssue.ruleId === issue.ruleId && skippedIssue.token === match.token
          )
      );

      if (matches.length > 0) {
        result.push({
          explanation: issue.explanation,
          fieldId: issue.fieldId,
          processed: issue.processed,
          result: {
            passed: issue.result.passed,
            info:
              // If original result info is null, there are no matches
              issue.result.info == null
                ? null
                : {
                    message: issue.result.info.message,
                    matches: matches,
                  },
          },
          rule: issue.rule,
          ruleId: issue.ruleId,
        });
      }
    } else {
      result.push(issue);
    }
  });
  return result;
};
