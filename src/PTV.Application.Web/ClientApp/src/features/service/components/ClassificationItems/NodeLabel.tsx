import React, { FunctionComponent } from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { Button } from 'suomifi-ui-components';

const useStyles = makeStyles(() => ({
  count: {
    marginLeft: '5px',
  },
  infoTrigger: {
    '&.icon': {
      padding: '0 5px',
      minHeight: '20px',
      marginLeft: '5px',
    },
  },
}));

interface NodeLabelInterface {
  label: string;
  count?: number;
  info?: string | null;
}

export const NodeLabel: FunctionComponent<NodeLabelInterface> = ({ label, count, info }) => {
  const classes = useStyles();
  const triggerClass = clsx(classes.infoTrigger, 'icon');

  return (
    <span>
      <span>{label}</span>
      {count && <span className={classes.count}>({count})</span>}
      {info && <Button icon='infoFilled' key='info' className={triggerClass} variant='secondaryNoBorder' />}
    </span>
  );
};
