import React, { FunctionComponent, HTMLProps, ReactNode } from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';

type FieldsetProps = HTMLProps<HTMLFieldSetElement> & {
  children: ReactNode;
};

const useStyles = makeStyles(() => ({
  fieldsetLayout: {
    border: 0,
    padding: '0.01em 0 0 0',
    margin: 0,
    minWidth: 0,
  },
}));

export const Fieldset: FunctionComponent<FieldsetProps> = ({ children, ...rest }) => {
  const classes = useStyles();

  const className = clsx(classes.fieldsetLayout, rest.className);
  return (
    <fieldset {...rest} className={className}>
      {children}
    </fieldset>
  );
};
