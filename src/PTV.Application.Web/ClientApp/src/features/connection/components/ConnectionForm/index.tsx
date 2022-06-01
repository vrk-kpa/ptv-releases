import React from 'react';
import { Control, UseFormSetValue, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import { makeStyles } from '@mui/styles';
import { Heading } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, LanguageVersionExpanderTypes, StateLanguageVersionExpander } from 'types/forms/connectionFormTypes';
import { Message } from 'components/Message';
import { useFormMetaContext } from 'context/formMeta';
import { CompareLanguageSelector } from 'features/connection/components/CompareLanguageSelector';
import { FormBlock } from 'features/connection/components/FormLayout';
import { LanguageVersionForm } from 'features/connection/components/LanguageVersionForm';
import ServiceConnectionQualityChecker from 'features/qualityAgent/ServiceConnectionQualityChecker';

const useStyles = makeStyles(() => ({
  languageSelectorContainerBox: {
    backgroundColor: 'rgb(247, 247, 248)',
    borderTop: '1px solid rgb(200, 205, 208)',
    borderLeft: '1px solid rgb(200, 205, 208)',
    borderRight: '1px solid rgb(200, 205, 208)',
    padding: '16px 20px 16px 20px',
  },
}));

type ConnectionFormProps = {
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
  setValue: UseFormSetValue<ConnectionFormModel>;
  isComparing: boolean;
  enabledLanguages?: Language[];
  getFormValues?: () => ConnectionFormModel;
  expanderStates: StateLanguageVersionExpander;
  toggleExpander: (states: LanguageVersionExpanderTypes) => void;
};

export function ConnectionForm(props: ConnectionFormProps): React.ReactElement {
  const classes = useStyles();
  const { t } = useTranslation();
  const meta = useFormMetaContext();

  return (
    <form>
      <ServiceConnectionQualityChecker
        formType='Services'
        language={meta.selectedLanguageCode}
        enabled={meta.mode === 'edit' && !meta.displayComparison && meta.compareLanguageCode === undefined}
        control={props.control}
      />

      <Grid container direction='column'>
        {props.isComparing && (
          <Grid item>
            <FormBlock marginTop='20px' marginBottom='13px' className={classes.languageSelectorContainerBox}>
              <Box mb={2}>
                <CompareLanguageSelector />
              </Box>
              <Message type='info'>{t('Ptv.ConnectionDetails.LanguageComparison.ErrorMessagesHint')}</Message>
            </FormBlock>
          </Grid>
        )}
        <Grid item>
          <FormBlock marginTop='20px' marginBottom='13px'>
            <Heading variant='h3'>{t('Ptv.ConnectionDetails.Expanders.Title')}</Heading>
          </FormBlock>
        </Grid>
        <Grid item>
          <LanguageVersionForm
            control={props.control}
            trigger={props.trigger}
            setValue={props.setValue}
            language={meta.selectedLanguageCode}
            expanderStates={props.expanderStates}
            toggleExpander={props.toggleExpander}
          />
        </Grid>
      </Grid>
    </form>
  );
}
