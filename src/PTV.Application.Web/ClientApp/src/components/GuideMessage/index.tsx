import React, { FunctionComponent, ReactNode } from 'react';
import { makeStyles } from '@mui/styles';
import { BaseIconKeys, Icon } from 'suomifi-ui-components';

type GuideMessageProps = {
  icon: BaseIconKeys;
  children: ReactNode;
};

const useStyles = makeStyles((theme) => ({
  guideMessage: {
    wordBreak: 'break-word',
    border: '1px solid',
    borderRadius: '2px',
    borderColor: theme.colors.hint,
    backgroundColor: '#f7fafd',
    fontWeight: theme.suomifi.values.typography.bodyText.fontWeight,
    lineHeight: '20px',
    padding: '20px 20px 0',
    display: 'flex',
    alignItems: 'flex-start',
  },
  icon: {
    height: '25px',
    width: '25px',
    marginRight: '15px',
    fill: theme.colors.info,
    color: theme.colors.hint,
  },
  content: {
    marginBottom: '20px',
  },
}));

export const GuideMessage: FunctionComponent<GuideMessageProps> = (props: GuideMessageProps) => {
  const classes = useStyles();

  return (
    <div className={classes.guideMessage}>
      <div>
        <Icon className={classes.icon} icon={props.icon} />
      </div>
      <div className={classes.content}>{props.children}</div>
    </div>
  );
};
