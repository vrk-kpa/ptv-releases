import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { Expander, ExpanderContent, ExpanderTitleButton, Text } from 'suomifi-ui-components';
import { QualityResult } from 'types/qualityAgentResponses';
import { lingsoftRulePriority } from 'types/qualityAgentRuleId';
import QualityAgentIssue from './components/QualityAgentIssue';
import QualityAgentSpecialIssue from './components/QualityAgentSpecialIssue';

type QualityAgentErrorProps = {
  issues: QualityResult[];
};

export default function QualityAgentErrors(props: QualityAgentErrorProps): React.ReactElement {
  const { t } = useTranslation();

  const renderExpander = (issue: QualityResult): React.ReactElement | null => {
    const errorsWithoutSuggestions =
      issue.result.info?.matches?.filter(({ suggestions }) => suggestions == null || !suggestions?.length) || [];
    const errorCount = Number(errorsWithoutSuggestions?.length);

    if (!errorCount) {
      return null;
    }

    return (
      <Expander>
        <ExpanderTitleButton>
          <Text color='blackLight1'>
            {t('Ptv.QualityAgent.Service.OtherErrors')} {errorCount || ''}
          </Text>
        </ExpanderTitleButton>
        <ExpanderContent>
          <Text>{t('Ptv.QualityAgent.Service.OtherErrorTitle')}</Text>
          <Grid item xs={12}>
            <QualityAgentSpecialIssue issue={issue} />
          </Grid>
        </ExpanderContent>
      </Expander>
    );
  };

  const sortIssues = (issues: QualityResult[]): QualityResult[] => {
    const qualityResultsDict: { [key: string]: QualityResult } = {};
    issues.forEach((issue: QualityResult) => {
      qualityResultsDict[issue.ruleId] = issue;
    });

    return lingsoftRulePriority
      .map((ruleId: string) => qualityResultsDict[ruleId])
      .filter((issue: QualityResult | undefined) => issue !== undefined);
  };

  const sortedIssues = sortIssues(props.issues);
  return (
    <div>
      {sortedIssues.map((issue, index) => {
        return (
          <QualityAgentIssue key={issue.ruleId} issue={issue} index={index}>
            {renderExpander(issue)}
          </QualityAgentIssue>
        );
      })}
    </div>
  );
}
