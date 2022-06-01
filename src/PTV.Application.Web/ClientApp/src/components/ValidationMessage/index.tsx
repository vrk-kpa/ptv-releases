import React from 'react';
import { DefaultTheme, makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { MessageType } from 'types/enumTypes';

type ValidationMessageProps = {
  message: string | null | undefined;
  type?: MessageType;
};

interface StyleProps {
  type: MessageType;
}

const useStyles = makeStyles<DefaultTheme, StyleProps>((theme) => ({
  message: {
    wordBreak: 'break-word',
    color: ({ type }) => (type === 'error' ? theme.suomifi.values.colors.alertBase.hsl : theme.suomifi.values.colors.successBase.hsl),
    fontSize: 14,
    fontWeight: theme.suomifi.values.typography.bodySemiBoldSmall.fontWeight,
    lineHeight: '20px',
  },
}));

export default function ValidationMessage({ message, type = 'error' }: ValidationMessageProps): React.ReactElement | null {
  const classes = useStyles({ type });

  if (!message) {
    return null;
  }

  const messageClass = clsx(classes.message);
  return <span className={messageClass}>{message}</span>;
}
