import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Expander, ExpanderContent, ExpanderTitleButton, Heading } from 'suomifi-ui-components';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { ServiceFormValues, ServiceModel } from 'types/forms/serviceFormTypes';
import { FormDivider } from 'components/formLayout/FormDivider';
import Archive from './Archive';
import Copy from './Copy';
import Translations from './Translations';

const useStyles = makeStyles((theme) => ({
  title: {
    '& h2.custom': {
      color: theme.colors.link,
    },
  },
  titleHint: {
    color: 'rgb(95, 104, 109)',
    fontSize: '16px',
    fontWeight: 'normal',
  },
}));

type FormFunctionsProps = {
  control: Control<ServiceModel>;
  canUpdate: boolean;
  hasTranslationOrder: boolean;
  getFormValues: () => ServiceFormValues;
  onServiceCopied: (copy: ServiceFormValues) => void;
  onServiceArchivedOrRestored: (data: ServiceApiModel) => void;
  translationOrderSuccess: (data: ServiceApiModel) => void;
};

export default function FormFunctions(props: FormFunctionsProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  return (
    <Expander>
      <ExpanderTitleButton className={classes.title}>
        <Heading variant='h4' as='h2' className='custom'>
          {t('Ptv.Form.Header.Functions')}
        </Heading>
        <span className={classes.titleHint}>{t('Ptv.Form.Header.Functions.Hint')}</span>
      </ExpanderTitleButton>
      <ExpanderContent>
        <Translations {...props} />
        <FormDivider my={3} />
        <Copy control={props.control} onServiceCopied={props.onServiceCopied} getFormValues={props.getFormValues} />
        <FormDivider my={3} />
        <Archive
          hasTranslationOrder={props.hasTranslationOrder}
          canChangeState={props.canUpdate}
          control={props.control}
          onServiceArchivedOrRestored={props.onServiceArchivedOrRestored}
        />
      </ExpanderContent>
    </Expander>
  );
}
