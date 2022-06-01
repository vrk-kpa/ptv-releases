import React, { HTMLProps, ReactElement, ReactNode } from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';

type DescriptionDetailsProps = HTMLProps<HTMLElement> & {
  children: ReactNode;
};

const useStyles = makeStyles((theme) => ({
  ddTypography: {
    fontWeight: theme.suomifi.values.typography.bodyText.fontWeight,
    fontFamily: theme.suomifi.values.typography.bodyText.fontFamily,
    fontSize: theme.suomifi.values.typography.bodyText.fontSize.value,
  },
  ddLayout: {
    margin: 0,
    marginTop: '5px',
  },
}));

export function DescriptionDetails({ children, ...rest }: DescriptionDetailsProps): ReactElement {
  const classes = useStyles();

  const className = clsx(classes.ddTypography, classes.ddLayout, rest.className);
  return (
    <dd {...rest} className={className}>
      {children}
    </dd>
  );
}
