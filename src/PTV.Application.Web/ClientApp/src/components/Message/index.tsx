import React, { FunctionComponent } from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { BaseIconKeys, Icon } from 'suomifi-ui-components';
import { MessageType } from 'types/enumTypes';

interface MessageInterface {
  type?: MessageType;
  className?: string;
  icon?: BaseIconKeys;
}

const useStyles = makeStyles((theme) => ({
  message: {
    wordBreak: 'break-word',
    borderLeft: '4px solid',
    fontWeight: theme.suomifi.values.typography.bodyText.fontWeight,
    lineHeight: '20px',
    padding: '20px 20px 0',
    display: 'flex',
    alignItems: 'flex-start',
  },
  info: {
    borderLeftColor: theme.suomifi.values.colors.accentSecondary.hsl,
    backgroundColor: theme.suomifi.values.colors.accentSecondaryLight1.hsl,
  },
  error: {
    borderLeftColor: theme.suomifi.values.colors.alertBase.hsl,
    backgroundColor: theme.suomifi.values.colors.alertLight1.hsl,
  },
  generalDescription: {
    borderLeftColor: theme.suomifi.values.colors.accentTertiaryDark1.hsl,
    backgroundColor: '#f4eef7',
  },
  errorIcon: {
    height: '20px',
    width: '20px',
    marginRight: '15px',
    fill: theme.colors.error,
  },
  icon: {
    height: '20px',
    width: '20px',
    marginRight: '15px',
    fill: theme.colors.info,
  },
  content: {
    marginBottom: '20px',
  },
}));

export const Message: FunctionComponent<MessageInterface> = ({ children, type = 'info', className, icon }) => {
  const classes = useStyles();
  const messageClass = clsx(classes.message, classes[type], className);

  return (
    <div className={messageClass}>
      {type === 'error' && (
        <div>
          <Icon className={classes.errorIcon} icon='error' />
        </div>
      )}
      {icon !== undefined && type !== 'error' && (
        <div>
          <Icon className={classes.errorIcon} icon={icon} />
        </div>
      )}
      <div className={classes.content}>{children}</div>
    </div>
  );
};
