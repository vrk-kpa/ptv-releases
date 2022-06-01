import * as React from 'react';
import { FunctionComponent } from 'react';
import { makeStyles } from '@mui/styles';

const useStyles = makeStyles(() => ({
  parent: {
    whiteSpace: 'pre-wrap',
  },
}));
export const DebugPreCode: FunctionComponent<{
  title: string;
  object: unknown;
}> = ({ title, object }) => {
  const classes = useStyles();
  return (
    <>
      {title}:
      <pre className={classes.parent}>
        <code>{JSON.stringify(object, null, 2)}</code>
      </pre>
    </>
  );
};
