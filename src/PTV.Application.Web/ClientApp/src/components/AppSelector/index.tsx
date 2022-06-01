import React, { useContext, useEffect } from 'react';
import { useCookies } from 'react-cookie';
import { MatomoProvider } from '@jonkoops/matomo-tracker-react';
import { CssBaseline, ThemeProvider } from '@mui/material';
import { createTheme } from '@mui/material/styles';
import { makeStyles } from '@mui/styles';
import { createStyled } from '@mui/system';
import { ServerErrorBox } from 'components/ErrorBox/ServerErrorBox';
import Layout from 'components/Layout';
import LoadingIndicator from 'components/LoadingIndicator';
import { DispatchContext } from 'context/DispatchContextProvider';
import { useGetEnvironmentSettings } from 'hooks/queries/useGetEnvironmentSettings';
import { themeOptions } from 'styles/theme';
import { PtvCookieName, hasTokenExpired } from 'utils/auth';
import { createPtvMatomoInstance } from 'utils/matomo';
import { setSettings } from 'utils/settings';
import Login from 'features/login';
import AuthenticatedApp from './AuthenticatedApp';

export const theme = createTheme(themeOptions);

export const styled = createStyled({ defaultTheme: themeOptions });

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    flex: 1,
  },
  loadingIndicator: {
    display: 'flex',
    flex: 1,
    justifyContent: 'center',
    alignItems: 'flex-start',
  },
}));

export default function AppSelector(): React.ReactElement {
  const classes = useStyles();
  const [cookies] = useCookies([PtvCookieName]);
  const dispatch = useContext(DispatchContext);

  const { isLoading, error, data } = useGetEnvironmentSettings();

  useEffect(() => {
    if (data) {
      dispatch({ type: 'SettingsUpdated', payload: data });
    }
  }, [data, dispatch]);

  // Note done outside useEffect() because that is executed after
  // render and <Login/> needs to know the settings so that it
  // can fetch needed data.
  if (data) {
    setSettings(data);
  }

  function getContent() {
    if (isLoading) {
      return (
        <div className={classes.loadingIndicator}>
          <LoadingIndicator />
        </div>
      );
    }

    if (error) {
      return <ServerErrorBox httpError={error} />;
    }

    if (hasTokenExpired(cookies[PtvCookieName])) {
      return <Login />;
    }

    if (!data) {
      // There should always be data defined, so we shound't end here.
      return <AuthenticatedApp />;
    }

    const matomoInstance = createPtvMatomoInstance(data.environmentType);

    return (
      <MatomoProvider value={matomoInstance}>
        <AuthenticatedApp />
      </MatomoProvider>
    );
  }

  return (
    <ThemeProvider theme={theme}>
      <div className={classes.root}>
        <CssBaseline />
        <Layout>{getContent()}</Layout>
      </div>
    </ThemeProvider>
  );
}
