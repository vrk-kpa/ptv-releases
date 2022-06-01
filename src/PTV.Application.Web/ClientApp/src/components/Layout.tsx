import React, { ReactNode } from 'react';
import Grid from '@mui/material/Grid';
import { makeStyles } from '@mui/styles';
import Footer from 'features/footer';
import Header from 'features/header';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    flexDirection: 'column',
    flex: 1,
    height: '100vh',
    margin: '0 auto',
  },
  contentSpace: {
    display: 'flex',
    justifyContent: 'center',
    flex: 1,
    alignItems: 'flex-start',
  },
  content: {
    display: 'flex',
    flex: 1,
    maxWidth: theme.layout.maxWidth,
  },
  headerSpace: {
    display: 'flex',
    justifyContent: 'center',
    borderTop: `4px solid rgb(0, 52, 122)`,
    background: theme.aside.background,
    borderBottom: theme.aside.border,
  },
  header: {
    display: 'flex',
    flex: 1,
    maxWidth: theme.layout.maxWidth,
  },
  footerSpace: {
    display: 'flex',
    justifyContent: 'center',
    background: theme.aside.background,
    borderTop: theme.aside.border,
  },
  footer: {
    display: 'flex',
    flex: 1,
    maxWidth: theme.layout.maxWidth,
  },
}));

type LayoutProps = {
  children: ReactNode;
};

export default function Layout(props: LayoutProps): React.ReactElement {
  const classes = useStyles();

  return (
    <div className={classes.root}>
      <div className={classes.headerSpace}>
        <div className={classes.header}>
          <Grid container justifyContent='center' alignContent='center'>
            <Grid item xs={12} sm={11} md={10} lg={9} xl={7}>
              <Header />
            </Grid>
          </Grid>
        </div>
      </div>
      <div className={classes.contentSpace}>
        <div className={classes.content}>
          <Grid container justifyContent='center' alignContent='center'>
            <Grid item xs={12} sm={11} md={10} lg={9} xl={7}>
              {props.children}
            </Grid>
          </Grid>
        </div>
      </div>
      <div className={classes.footerSpace}>
        <div className={classes.footer}>
          <Grid container justifyContent='center' alignContent='center'>
            <Grid item xs={12} sm={11} md={10} lg={9} xl={7}>
              <Footer />
            </Grid>
          </Grid>
        </div>
      </div>
    </div>
  );
}
