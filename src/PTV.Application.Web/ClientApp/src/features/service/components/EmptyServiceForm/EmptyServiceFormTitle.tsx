import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Heading } from 'suomifi-ui-components';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';
import { getKeyForServiceType } from 'utils/translations';
import { getUserOrganization } from 'utils/userInfo';

const useStyles = makeStyles(() => ({
  preTitle: {
    color: 'rgb(95, 104, 109)',
    fontSize: '16px',
  },
  subTitle: {
    color: 'rgb(95, 104, 109)',
    fontSize: '12px',
    fontWeight: 600,
    textTransform: 'uppercase',
  },
}));

export function EmptyServiceFormTitle(): React.ReactElement {
  const appContext = useAppContextOrThrow();
  const translate = useTranslateLocalizedText();
  const { t } = useTranslation();
  const classes = useStyles();

  const userOrganization = getUserOrganization(appContext);
  const serviceTypeText = t(getKeyForServiceType('Service'));

  const getOrganizationName = (): string => {
    return !!userOrganization ? translate(userOrganization.texts, userOrganization.name) : t('Ptv.Service.Header.UnknownOrganization');
  };

  const organizationName = getOrganizationName();

  return (
    <div>
      <div className={classes.preTitle}>{organizationName}</div>
      <Heading variant='h1'>{t('Ptv.Service.Header.NewService')}</Heading>
      <div className={classes.subTitle}>{serviceTypeText}</div>
    </div>
  );
}
