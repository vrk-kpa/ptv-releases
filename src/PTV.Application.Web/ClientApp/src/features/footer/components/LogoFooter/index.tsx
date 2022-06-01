import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import logoFooter from 'images/icon-logo-footer.svg';

const useStyles = makeStyles(() => ({
  image: {
    marginRight: '10px',
    height: '32px',
    width: '121px',
  },
  content: {
    display: 'flex',
  },
}));

export default function LogoFooter(): React.ReactElement {
  const classes = useStyles();
  const { t } = useTranslation();
  return (
    <div className={classes.content}>
      <a target='_blank' href={t('Ptv.LogoFooter.Link')} rel='noreferrer noopener'>
        <img src={logoFooter} alt={t('Ptv.SuomiFi.Logo.Alt')} className={classes.image} />
      </a>
    </div>
  );
}
