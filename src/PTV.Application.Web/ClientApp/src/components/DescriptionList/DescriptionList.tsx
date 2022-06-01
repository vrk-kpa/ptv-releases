import React, { HTMLProps, ReactElement, ReactNode } from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';

type DescriptionListProps = HTMLProps<HTMLDListElement> & {
  children: ReactNode;
};

const useStyles = makeStyles(() => ({
  dlLayout: {
    padding: 0,
    margin: 0,
  },
}));

export function DescriptionList({ children, ...rest }: DescriptionListProps): ReactElement {
  const classes = useStyles();

  const className = clsx(classes.dlLayout, rest.className);
  return (
    <dl {...rest} className={className}>
      {children}
    </dl>
  );
}
