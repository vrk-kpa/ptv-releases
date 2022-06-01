import React from 'react';
import { Control, UseFormSetValue, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { Language } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { VisualHeading } from 'components/VisualHeading';
import { OtherProducersMainLevel } from './OtherProducersMainLevel';
import { PurchaseProducersMainLevel } from './PurchaseProducersMainLevel';
import { SelfProducersMainLevel } from './SelfProducersMainLevel';
import { getDefaultSelfProducers } from './utils';

interface ServiceProvidersViewInterface {
  tabLanguage: Language;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
  namespace: string;
}

const useStyles = makeStyles(() => ({
  label: {
    '&.custom': {
      marginTop: '17px',
    },
  },
}));

export default function ServiceProvidersView(props: ServiceProvidersViewInterface): React.ReactElement | null {
  const { t } = useTranslation();
  const classes = useStyles();

  const responsibleOrganization = useWatch({ control: props.control, name: `${cService.responsibleOrganization}` });
  const otherResponsibleOrganizations = useWatch({ control: props.control, name: `${cService.otherResponsibleOrganizations}` });
  const defaultSelfProducers = getDefaultSelfProducers(responsibleOrganization, otherResponsibleOrganizations);
  const responsibleOrganizationIds = defaultSelfProducers.map((x) => x.id);

  return (
    <Box>
      <VisualHeading variant='h5' className={clsx(classes.label, 'custom')}>
        {t('Ptv.Service.Form.Field.ServiceProducers.SelfProducers.Title')}
      </VisualHeading>
      <SelfProducersMainLevel
        control={props.control}
        tabLanguage={props.tabLanguage}
        setValue={props.setValue}
        defaultSelfProducers={defaultSelfProducers}
        namespace={props.namespace}
      />
      <VisualHeading variant='h5' className={clsx(classes.label, 'custom')}>
        {t('Ptv.Service.Form.Field.ServiceProducers.PurchaseProducers.Title')}
      </VisualHeading>
      <PurchaseProducersMainLevel
        control={props.control}
        tabLanguage={props.tabLanguage}
        responsibleOrganizationIds={responsibleOrganizationIds}
        setValue={props.setValue}
      />
      <VisualHeading variant='h5' className={clsx(classes.label, 'custom')}>
        {t('Ptv.Service.Form.Field.ServiceProducers.OtherProducers.Title')}
      </VisualHeading>
      <OtherProducersMainLevel
        control={props.control}
        tabLanguage={props.tabLanguage}
        responsibleOrganizationIds={responsibleOrganizationIds}
        namespace={props.namespace}
        setValue={props.setValue}
      />
    </Box>
  );
}
