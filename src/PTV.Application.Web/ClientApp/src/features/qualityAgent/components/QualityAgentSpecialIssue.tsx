import React from 'react';
import { useTranslation } from 'react-i18next';
import { Divider } from '@mui/material';
import { Paragraph, Text } from 'suomifi-ui-components';
import { QualityMatch, QualityResult } from 'types/qualityAgentResponses';
import {
  LingsoftRuleCounterPart,
  LingsoftRuleGrammarError,
  LingsoftRuleLongSentence,
  LingsoftRulePassiveVerb,
  LingsoftRuleTypingError,
} from 'types/qualityAgentRuleId';
import { Message } from 'components/Message';
import { useQualityStyles } from 'features/qualityAgent/qualityStyles';

type QualityAgentSpecialIssueProps = {
  issue: QualityResult;
};

export default function QualityAgentSpecialIssue(props: QualityAgentSpecialIssueProps): React.ReactElement {
  const classes = useQualityStyles();
  const { t } = useTranslation();
  const { issue } = props;

  const specialIssueMatch = issue.result.info?.matches.filter((x) => x.suggestions == null || !x.suggestions.length) || [];
  const explanationPerRuleId: { [id: string]: string } = {
    [LingsoftRulePassiveVerb]: t('Ptv.QualityAgent.Service.InvalidWord'),
    [LingsoftRuleCounterPart]: t('Ptv.QualityAgent.Service.InvalidWord'),
    [LingsoftRuleLongSentence]: t('Ptv.QualityAgent.Service.LongSentenceError'),
    [LingsoftRuleGrammarError]: t('Ptv.QualityAgent.Service.GrammarError'),
    [LingsoftRuleTypingError]: t('Ptv.QualityAgent.Service.GrammarError'),
  };
  const ruleId: string = issue.ruleId;

  if (!specialIssueMatch.length) {
    return <div />;
  }
  return (
    <div>
      <Paragraph marginBottomSpacing='s'>
        <Text variant='lead'>{issue.rule}</Text>
      </Paragraph>
      {specialIssueMatch.map((match: QualityMatch) => {
        const { start, title, token, explanation } = match;
        const ruleText: string = explanationPerRuleId[ruleId];
        return (
          <div key={start + title + token}>
            <Paragraph marginBottomSpacing='xs'>
              <Text variant='bold'>{ruleText}</Text>
            </Paragraph>
            <Paragraph marginBottomSpacing='s'>
              <Text>{token}</Text>
            </Paragraph>
            <Paragraph>
              <Text>{t('Ptv.QualityAgent.Service.GuideNotice')}</Text>
            </Paragraph>
            <Message className={classes.message} icon='info'>
              <Paragraph>
                <Text variant='bold'>{t('Ptv.QualityAgent.Service.Guide')}</Text>
              </Paragraph>
              <Paragraph>
                <Text>
                  {issue.explanation} {explanation}
                </Text>
              </Paragraph>
            </Message>
            <Divider className={classes.divider} />
          </div>
        );
      })}
    </div>
  );
}
