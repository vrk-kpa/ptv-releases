import React, { FunctionComponent } from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { StaticChip } from 'suomifi-ui-components';

const useStyles = makeStyles((theme) => ({
  chip: {
    '&.custom': {
      margin: '10px 10px 0 0',
    },
    '&.custom.custom': {
      backgroundColor: theme.suomifi.values.colors.accentTertiary.hsl,
    },
  },
}));

export const GDChip: FunctionComponent = ({ children }) => {
  const classes = useStyles();
  const chipCSS = clsx(classes.chip, 'custom');

  return <StaticChip className={chipCSS}>{children}</StaticChip>;
};
