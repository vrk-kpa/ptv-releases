import React, { ReactNode } from 'react';
import { Icon } from 'suomifi-ui-components';
import { QualityResult } from 'types/qualityAgentResponses';
import { useQualityStyles } from '../';

type QualityAgentIssueProps = {
  issue: QualityResult;
  index: number;
  children: ReactNode;
};

export default function QualityAgentIssue(props: QualityAgentIssueProps): React.ReactElement {
  const classes = useQualityStyles();
  const { issue, index } = props;

  return (
    <div key={index}>
      <div className={classes.warningBanner}>
        <Icon icon='warning' fill='red' className={classes.warningIcon} />
        <span>{issue.explanation}</span>
        <div className={classes.extension}>{props.children}</div>
      </div>
    </div>
  );
}
