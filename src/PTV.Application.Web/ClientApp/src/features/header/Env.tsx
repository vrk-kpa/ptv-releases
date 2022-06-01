import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Text } from 'suomifi-ui-components';
import { AppEnvironment } from 'types/enumTypes';

const useStyles = makeStyles(() => ({
  env: {
    display: 'flex',
    backgroundColor: 'rgb(234, 113, 37)',
    paddingLeft: '16px',
    paddingRight: '16px',
    marginRight: '40px',
  },
}));

type EnvProps = {
  environment: AppEnvironment;
};

const TranslationKeys = new Map<AppEnvironment, string>([
  ['Dev', 'Ptv.Environment.Development'],
  ['Prod', 'Ptv.Environment.Production'],
  ['Qa', 'Ptv.Environment.Qa'],
  ['Test', 'Ptv.Environment.Test'],
  ['Trn', 'Ptv.Environment.Training'],
]);

export default function Env(props: EnvProps): React.ReactElement {
  const classes = useStyles();
  const { t } = useTranslation();

  const key = TranslationKeys.get(props.environment);

  return (
    <span className={classes.env}>
      <Text smallScreen={true}>{t(key || 'Ptv.Environment.Development')}</Text>
    </span>
  );
}
