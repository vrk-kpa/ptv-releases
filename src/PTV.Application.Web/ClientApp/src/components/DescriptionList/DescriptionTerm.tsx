import React, { HTMLProps, ReactElement, ReactNode } from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';

type DescriptionTermProps = HTMLProps<HTMLElement> & {
  children: ReactNode;
};

const useStyles = makeStyles((theme) => ({
  dtLayout: {
    margin: 0,
    marginTop: '20px',
  },
  dtTypography: {
    fontWeight: theme.suomifi.values.typography.heading5.fontWeight,
    fontFamily: theme.suomifi.values.typography.heading5.fontFamily,
    fontSize: theme.suomifi.values.typography.heading5.fontSize.value,
  },
}));

export function DescriptionTerm({ children, ...rest }: DescriptionTermProps): ReactElement {
  const classes = useStyles();

  const className = clsx(classes.dtLayout, classes.dtTypography, rest.className);
  return (
    <dt {...rest} className={className}>
      {children}
    </dt>
  );
}
