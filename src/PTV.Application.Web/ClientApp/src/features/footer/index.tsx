import React, { useContext } from 'react';
import { useTranslation } from 'react-i18next';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import { makeStyles } from '@mui/styles';
import { ExternalLink, Text } from 'suomifi-ui-components';
import { AppContext } from 'context/AppContextProvider';
import LogoFooter from 'features/footer/components/LogoFooter';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    flexGrow: 1,
    background: theme.aside.background,
  },
  footerContainer: {
    display: 'flex',
    flexGrow: 1,
    marginTop: '20px',
    marginBottom: '20px',
  },
  text: {
    marginTop: '20px',
    marginBottom: '20px',
  },
  divider: {
    marginBottom: '20px',
  },
  link: {
    marginRight: '40px',
  },
}));

export default function Footer(): React.ReactElement {
  const appContext = useContext(AppContext);
  const classes = useStyles();
  const { t } = useTranslation();

  function createLink(textKey: string, urlKey: string): React.ReactElement {
    return (
      <div className={classes.link}>
        <ExternalLink href={t(urlKey)} labelNewWindow={t('Ptv.Link.Label.NewWindow')}>
          {t(textKey)}
        </ExternalLink>
      </div>
    );
  }

  const env = appContext.settings?.environmentType;
  const showVersion = env !== 'Prod' && env !== 'Trn';
  const version = appContext.settings?.releaseNumber || appContext.settings?.version || '';
  const versionStr = t('Ptv.Version') + ': ' + version;

  return (
    <div className={classes.root}>
      <div className={classes.footerContainer}>
        <Grid container direction='column'>
          <Grid item>
            <LogoFooter />
          </Grid>
          <Grid item className={classes.text}>
            <Text>{t('Ptv.Footer.Text')}</Text>
          </Grid>
          <Grid item>
            <Divider className={classes.divider} />
          </Grid>
          <Grid item container>
            <Grid item>{createLink('Ptv.Footer.GiveFeedBack.Text', 'Ptv.Footer.GiveFeedBack.Link')}</Grid>
            <Grid item>{createLink('Ptv.Footer.GiveAnonymousFeedBack.Text', 'Ptv.Footer.GiveAnonymousFeedBack.Link')}</Grid>
            <Grid item>{createLink('Ptv.Footer.DataProtection.Text', 'Ptv.Footer.DataProtection.Link')}</Grid>
            <Grid item>{createLink('Ptv.Footer.Accessibility.Text', 'Ptv.Footer.Accessibility.Link')}</Grid>
            <Grid item>{createLink('Ptv.Footer.DriverLicense.Text', 'Ptv.Footer.DriverLicense.Link')}</Grid>
            <Grid item>{createLink('Ptv.Footer.Reports.Text', 'Ptv.Footer.Reports.Link')}</Grid>
            <Grid item>{createLink('Ptv.Footer.InfoAboutPtv.Text', 'Ptv.Footer.InfoAboutPtv.Link')}</Grid>
          </Grid>
          {showVersion && (
            <Grid item container>
              <Grid item>
                <Text>{versionStr}</Text>
              </Grid>
            </Grid>
          )}
        </Grid>
      </div>
    </div>
  );
}
