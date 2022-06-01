import React, { ReactElement } from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { Heading, HeadingProps } from 'suomifi-ui-components';

type VisualHeadingProps = HeadingProps;

const useStyles = makeStyles(() => ({
  visualHeadingMargins: {
    '&.fi-heading': {
      margin: 0,
      marginTop: '20px',
    },
  },
}));

export function VisualHeading({ children, as = 'p', className, ...rest }: VisualHeadingProps): ReactElement {
  const classes = useStyles();

  const classNames = clsx(classes.visualHeadingMargins, className);

  return (
    <Heading as={as} {...rest} className={classNames}>
      {children}
    </Heading>
  );
}
