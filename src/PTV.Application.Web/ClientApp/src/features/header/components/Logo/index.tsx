import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import palveluhallinta from 'images/logo.svg';
import logo from 'images/suomifi-symbol.svg';

const useStyles = makeStyles(() => ({
  image: {
    marginRight: '10px',
  },
  content: {
    display: 'flex',
  },
}));

export default function Logo(): React.ReactElement {
  const classes = useStyles();
  const { t } = useTranslation();
  return (
    <div className={classes.content}>
      <img src={logo} className={classes.image} alt='Suomi.fi logo' />
      <img src={palveluhallinta} alt={t('Ptv.Logo.Alt')} />
    </div>
  );
}
