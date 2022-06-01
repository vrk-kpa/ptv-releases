import React, { useContext } from 'react';
import { useMediaQuery } from '@mui/material';
import Grid from '@mui/material/Grid';
import { DefaultTheme, makeStyles } from '@mui/styles';
import { AppContext } from 'context/AppContextProvider';
import { getUserMainOrganization } from 'context/selectors';
import Logo from 'features/header/components/Logo';
import Env from './Env';
import LanguageSelector from './LanguageSelector';
import Menu from './Menu';
import UserInfo from './UserInfo';

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    flexGrow: 1,
  },
  appBar: {
    display: 'flex',
    flex: 1,
    paddingBottom: '10px',
  },
  logo: {
    display: 'flex',
    flex: 1,
  },
  common: {
    display: 'flex',
    marginTop: '10px',
  },
  userInfo: {
    display: 'flex',
    marginTop: '10px',
    marginRight: '50px',
  },
  languageSelector: {
    display: 'flex',
    marginTop: '10px',
  },
}));

export default function Header(): React.ReactElement {
  const classes = useStyles();
  const appContext = useContext(AppContext);
  const isWidthXs = useMediaQuery((theme: DefaultTheme) => theme.breakpoints.only('xs'));
  const rightSideJustify = isWidthXs ? 'flex-start' : 'flex-end';
  const organization = getUserMainOrganization(appContext);

  const hiddenXsDown = useMediaQuery((theme: DefaultTheme) => theme.breakpoints.down('xs'));
  const hiddenSmDown = useMediaQuery((theme: DefaultTheme) => theme.breakpoints.down('sm'));

  return (
    <header>
      <div className={classes.root}>
        <div className={classes.appBar}>
          <Grid container wrap='wrap' alignItems='center' spacing={0}>
            <Grid item xs={12} sm={3} md={3} lg={3}>
              <Logo />
            </Grid>

            <Grid item container wrap='wrap' justifyContent={rightSideJustify} xs={12} sm={9} md={9} lg={9}>
              {!hiddenXsDown && (
                <Grid item>
                  <Env environment={appContext.settings?.environmentType || 'Dev'} />
                </Grid>
              )}

              {!hiddenSmDown && (
                <Grid item className={classes.userInfo}>
                  <UserInfo firstName={appContext.userInfo?.name} lastName={appContext.userInfo?.surname} organization={organization} />
                </Grid>
              )}

              <Grid item className={classes.common}>
                <LanguageSelector environment={appContext.settings?.environmentType || 'Dev'} />
              </Grid>
              <Grid item className={classes.common}>
                <Menu />
              </Grid>
            </Grid>
          </Grid>
        </div>
      </div>
    </header>
  );
}
