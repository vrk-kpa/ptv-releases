import React, { useContext, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Divider, Grid } from '@mui/material';
import {
  Button,
  Expander,
  ExpanderContent,
  ExpanderTitleButton,
  Paragraph,
  RadioButton,
  RadioButtonGroup,
  Text,
} from 'suomifi-ui-components';
import { GuidedCorrectionItem, SkippedIssue } from 'types/qualityAgentResponses';
import { GuideMessage } from 'components/GuideMessage';
import LoadingIndicator from 'components/LoadingIndicator';
import { DispatchQualityAgentContext } from 'context/qualityAgent';
import { issueSkipped } from 'context/qualityAgent/actions';
import { useGetQualityAgentLoading } from 'context/qualityAgent/useGetQualityAgentLoading';
import { useQualityStyles } from '.';

type GuidedLanguageCorrectionProps = {
  issues: GuidedCorrectionItem[];
  handleTextChangeSuggestion?: (value: string, guidedCorrectionItem: GuidedCorrectionItem) => void;
};

export default function GuidedLanguageCorrection(props: GuidedLanguageCorrectionProps): React.ReactElement | null {
  const { issues, handleTextChangeSuggestion } = props;
  const { t } = useTranslation();
  const classes = useQualityStyles();
  const dispatch = useContext(DispatchQualityAgentContext);
  const isQualityCheckLoading = useGetQualityAgentLoading();

  const [selectedSuggestion, setSelectedSuggestion] = useState<string>('');

  const filteredIssues = issues.sort((a, b) => a.matchStart - b.matchStart);

  const anySuggestions = filteredIssues.length > 0;
  const currentIssue = anySuggestions ? filteredIssues[0] : null;

  const suggestionOptions = currentIssue?.matchSuggestions.slice(0, 4) ?? [];

  const handleSuggestionAccepted = (value: string, guidedCorrectionItem: GuidedCorrectionItem): void => {
    if (guidedCorrectionItem && handleTextChangeSuggestion) {
      handleTextChangeSuggestion(value, guidedCorrectionItem);
    }
  };

  const handleSkipCorrection = (item: GuidedCorrectionItem): void => {
    const skippedIssue: SkippedIssue = { token: item.matchToken, ruleId: item.ruleId, fieldId: item.fieldId };
    issueSkipped(dispatch, skippedIssue);
  };

  const replaceWithRegularQuotation = (text: string): string => {
    const regExpRule = new RegExp('[{}»]', 'g');
    return text.replaceAll(regExpRule, '"');
  };

  if (!anySuggestions || !currentIssue) {
    return null;
  }

  return (
    <div className={classes.expander}>
      <Expander>
        <ExpanderTitleButton>
          {t('Ptv.QualityAgent.Service.GuidedCorrectionTitle')}
          <Paragraph>
            <Text color='blackLight1'>{t('Ptv.QualityAgent.Service.GuidedCorrectionSubheading')}</Text>
          </Paragraph>
          {isQualityCheckLoading && <LoadingIndicator />}
        </ExpanderTitleButton>
        <ExpanderContent>
          <>
            <Grid item xs={12}>
              <Paragraph>
                <Text>{`${t('Ptv.QualityAgent.Service.IssuesRemaining')} ${filteredIssues.length}`}</Text>
              </Paragraph>
              <Paragraph marginBottomSpacing='s'>
                <Text variant='lead'>{currentIssue.rule}</Text>
              </Paragraph>
              <Paragraph marginBottomSpacing='xs'>
                <Text variant='bold'>{currentIssue.matchTitle ?? t('Ptv.QualityAgent.Service.GrammaticalError')}</Text>
              </Paragraph>
              <Paragraph marginBottomSpacing='s'>
                <Text>{currentIssue.matchToken}</Text>
              </Paragraph>
              <Paragraph>
                <Text>{t('Ptv.QualityAgent.Service.GuideNotice')}</Text>
              </Paragraph>
              <GuideMessage icon='hint'>
                <Paragraph>
                  <Text variant='bold'>{t('Ptv.QualityAgent.Service.Guide')}</Text>
                </Paragraph>
                <Paragraph>
                  <Text>
                    {currentIssue.matchExplanation
                      ? replaceWithRegularQuotation(currentIssue.matchExplanation)
                      : currentIssue.issueExplanataion}
                  </Text>
                </Paragraph>
              </GuideMessage>
              <Paragraph style={{ marginTop: '20px' }}>
                <Text variant='bold'>{t('Ptv.QualityAgent.Service.SentenceWhereUnkownWordIsLocated')}</Text>
              </Paragraph>
              <Paragraph marginBottomSpacing='xs'>
                <Text>{`"${currentIssue.matchSentence}"`}</Text>
              </Paragraph>
              <Grid item xs={12}>
                {suggestionOptions.length === 1 && (
                  <>
                    <Paragraph>
                      <Text variant='bold'>{t('Ptv.QualityAgent.Service.Suggestion')}</Text>
                    </Paragraph>
                    <Paragraph>
                      <Text>{suggestionOptions[0]}</Text>
                    </Paragraph>
                  </>
                )}
                {suggestionOptions.length > 1 && (
                  <RadioButtonGroup
                    name={`${currentIssue.fieldId}_suggestion_group`}
                    labelText={t('Ptv.QualityAgent.Service.Options')}
                    value={selectedSuggestion}
                    onChange={(value) => setSelectedSuggestion(value)}
                  >
                    {suggestionOptions.map((suggestion) => {
                      return (
                        <RadioButton key={suggestion} value={suggestion}>
                          {suggestion}
                        </RadioButton>
                      );
                    })}
                  </RadioButtonGroup>
                )}
              </Grid>
            </Grid>
            <Divider className={classes.divider} />
            <Grid container spacing={2}>
              <Grid item>
                <Button
                  aria-disabled={isQualityCheckLoading || (selectedSuggestion.length === 0 && suggestionOptions.length > 1)}
                  onClick={() => {
                    if (suggestionOptions.length === 1) {
                      handleSuggestionAccepted(suggestionOptions[0], currentIssue);
                    } else {
                      handleSuggestionAccepted(selectedSuggestion, currentIssue);
                    }
                    setSelectedSuggestion('');
                  }}
                >
                  {t('Ptv.QualityAgent.Service.Accept')}
                </Button>
              </Grid>
              <Grid item>
                <Button
                  disabled={isQualityCheckLoading}
                  variant='secondary'
                  onClick={() => {
                    handleSkipCorrection(currentIssue);
                    setSelectedSuggestion('');
                  }}
                >
                  {t('Ptv.QualityAgent.Service.SkipCorrection')}
                </Button>
              </Grid>
            </Grid>
          </>
        </ExpanderContent>
      </Expander>
    </div>
  );
}
