import React from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Heading } from 'suomifi-ui-components';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';
import { getKeyForServiceType } from 'utils/translations';
import { useGetServiceName } from './useGetServiceName';

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

type ServiceFormTitleProps = {
  control: Control<ServiceModel>;
};

export function ServiceFormTitle(props: ServiceFormTitleProps): React.ReactElement {
  const translate = useTranslateLocalizedText();
  const { t } = useTranslation();
  const classes = useStyles();

  const serviceType = useWatch({ control: props.control, name: `${cService.serviceType}` });
  const serviceName = useGetServiceName(props.control);

  const responsibleOrganization = useWatch({ control: props.control, name: `${cService.responsibleOrganization}` });
  const connectedChannels = useWatch({ control: props.control, name: `${cService.connectedChannels}` });

  const serviceNameText = serviceName.length !== 0 ? serviceName : t('Ptv.Service.Header.NewService');
  const serviceTypeText = t(getKeyForServiceType(serviceType));
  const subtitle = `${serviceTypeText} · ${connectedChannels.length} ${t('Ptv.Service.Header.NumberOfConnections')}`;

  const getOrganizationName = (): string => {
    return !!responsibleOrganization
      ? translate(responsibleOrganization.texts, responsibleOrganization.name)
      : t('Ptv.Service.Header.UnknownOrganization');
  };

  return (
    <div>
      <div className={classes.preTitle}>{getOrganizationName()}</div>
      <Heading id='serviceName' variant='h1'>
        {serviceNameText}
      </Heading>
      <div className={classes.subTitle}>{subtitle}</div>
    </div>
  );
}
