import React, { useEffect, useState } from 'react';
import { useUnmount } from 'react-use';
import { Box, Grid } from '@mui/material';
import { GuidedCorrectionItem, QualityResult } from 'types/qualityAgentResponses';
import { useFormMetaContext } from 'context/formMeta';
import GuidedLanguageCorrection from './GuidedLanguageCorrection';
import QualityAgentErrors from './QualityAgentErrors';
import { issuesForGuidedCorrection } from './utility';

type QualityIssuesProps = {
  issues: QualityResult[];
  handleTextChangeSuggestion?: (value: string, guidedCorrectionItem: GuidedCorrectionItem) => void;
  onLostFocusWithZeroGuidedErrors?: () => void;
};

export default function QualityIssues(props: QualityIssuesProps): React.ReactElement {
  const onLostFocusWithZeroGuidedErrors = props.onLostFocusWithZeroGuidedErrors;
  const issuesforLanguageCorrection = issuesForGuidedCorrection(props.issues);
  const { displayComparison, compareLanguageCode } = useFormMetaContext();
  const [guidedCorrectionHasFocus, setGuidedCorrectionHasFocus] = useState<boolean>(false);

  const isComparisonEnabled = displayComparison || compareLanguageCode !== undefined;
  const issueCount = issuesforLanguageCorrection.length;

  useUnmount(() => {
    if (guidedCorrectionHasFocus) {
      onLostFocusWithZeroGuidedErrors?.();
    }
  });

  useEffect(() => {
    if (guidedCorrectionHasFocus && issueCount === 0) {
      setGuidedCorrectionHasFocus(false);
      onLostFocusWithZeroGuidedErrors?.();
    }
  }, [guidedCorrectionHasFocus, issueCount, onLostFocusWithZeroGuidedErrors]);

  return (
    <Box>
      {!isComparisonEnabled && issueCount > 0 && (
        <Grid container>
          <Grid item xs={12} onFocus={() => setGuidedCorrectionHasFocus(true)} onBlur={() => setGuidedCorrectionHasFocus(false)}>
            <GuidedLanguageCorrection issues={issuesforLanguageCorrection} handleTextChangeSuggestion={props.handleTextChangeSuggestion} />
          </Grid>
        </Grid>
      )}
      {!isComparisonEnabled && (
        <Grid container>
          <QualityAgentErrors issues={props.issues} />
        </Grid>
      )}
    </Box>
  );
}
