import React from 'react';
import Grid from '@mui/material/Grid';
import { makeStyles } from '@mui/styles';
import { Heading, Icon } from 'suomifi-ui-components';

type ErrorBoxProps = {
  title: string;
  children?: React.ReactNode | null | undefined;
};

const useStyles = makeStyles((theme) => ({
  root: {
    border: '1px solid',
    borderColor: theme.colors.border,
    borderTop: '4px solid',
    borderTopColor: theme.colors.error,
    borderRadius: '8px',
    backgroundColor: theme.colors.boxBackground,
    paddingTop: '18px',
    paddingBottom: '18px',
    paddingLeft: '30px',
  },
  icon: {
    height: '30px',
    width: '30px',
    marginRight: '18px',
    fill: theme.colors.error,
  },
}));

export function ErrorBox(props: ErrorBoxProps): React.ReactElement {
  const classes = useStyles();

  return (
    <div className={classes.root}>
      <Grid container>
        <Grid item>
          <Icon className={classes.icon} icon='error' />
        </Grid>
        <Grid item xs container>
          <Grid item>
            <Heading variant='h3'>{props.title}</Heading>
            {props.children}
          </Grid>
        </Grid>
      </Grid>
    </div>
  );
}
