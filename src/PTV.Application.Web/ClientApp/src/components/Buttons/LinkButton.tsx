import React from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';

const useStyles = makeStyles((theme) => ({
  linkButton: {
    backgroundColor: 'transparent',
    border: 'none',
    cursor: 'pointer',
    display: 'inline',
    margin: 0,
    padding: 0,
    color: theme.colors.link,
    fontWeight: theme.suomifi.values.typography.bodyText.fontWeight,
    fontFamily: theme.suomifi.values.typography.bodyText.fontFamily,
    fontSize: theme.suomifi.values.typography.bodyText.fontSize.value,
  },
}));

type LinkButtonProps = {
  className?: string | undefined;
  onClick: React.MouseEventHandler<HTMLButtonElement>;
  label: string;
};

export default function LinkButton(props: LinkButtonProps): React.ReactElement {
  const classes = useStyles();
  const mixedClasses = clsx(classes.linkButton, props.className);

  return (
    <button type='button' className={mixedClasses} onClick={props.onClick}>
      {props.label}
    </button>
  );
}
