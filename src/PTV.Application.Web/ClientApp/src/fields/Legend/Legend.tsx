import React, { HTMLProps, ReactElement, ReactNode } from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';

type LegendProps = HTMLProps<HTMLLegendElement> & {
  children: ReactNode;
};

const useStyles = makeStyles((theme) => ({
  legendTypography: {
    fontWeight: theme.suomifi.values.typography.heading5.fontWeight,
    fontFamily: theme.suomifi.values.typography.heading5.fontFamily,
    fontSize: theme.suomifi.values.typography.heading5.fontSize.value,
  },
  legendLayout: {
    marginBottom: '10px',
    paddingLeft: 0,
  },
}));

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function Legend({ children, ...rest }: LegendProps): ReactElement {
  const classes = useStyles();

  const className = clsx(classes.legendLayout, classes.legendTypography, rest.className);
  return (
    <legend {...rest} className={className}>
      {children}
    </legend>
  );
}
