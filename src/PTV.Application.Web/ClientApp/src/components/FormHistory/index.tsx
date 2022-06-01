import React, { useState } from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Expander, ExpanderContent, ExpanderTitleButton, Heading } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { FormDivider } from 'components/formLayout/FormDivider';
import ConnectionsHistory from './ConnectionsHistory';
import EditHistory from './EditHistory';
import IdInfo from './IdInfo';
import TranslationHistory from './TranslationHistory';

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

type FormHistoryProps = {
  selectedLanguage: Language;
  control: Control<ServiceModel>;
};

export default function FormHistory(props: FormHistoryProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const [isOpen, setIsOpen] = useState<boolean>(false);

  const serviceId = useWatch({ control: props.control, name: `${cService.id}` });
  const serviceUnificRootId = useWatch({ control: props.control, name: `${cService.unificRootId}` });
  const serviceVersion = useWatch({ control: props.control, name: `${cService.version}` });
  const serviceStatus = useWatch({ control: props.control, name: `${cService.status}` });

  const handleChange = (value: boolean) => {
    setIsOpen(!value);
  };

  return (
    <Expander open={isOpen} onOpenChange={handleChange}>
      <ExpanderTitleButton className={classes.title}>
        <Heading variant='h4' as='h2' className='custom'>
          {t('Ptv.Form.Header.History')}
        </Heading>
        <span className={classes.titleHint}>{t('Ptv.Form.Header.History.Hint')}</span>
      </ExpanderTitleButton>
      <ExpanderContent>
        <Box mt={2}>
          <EditHistory id={serviceId || ''} entityType='Service' {...props} isVisible={isOpen} />
        </Box>
        <Box mt={2}>
          <ConnectionsHistory id={serviceId || ''} entityType='Service' {...props} isVisible={isOpen} />
        </Box>
        <Box mt={2}>
          <TranslationHistory id={serviceId || ''} entityType='Service' {...props} isVisible={isOpen} />
        </Box>
        <FormDivider my={3} />
        <IdInfo
          id={serviceId || ''}
          unificRootId={serviceUnificRootId || ''}
          version={serviceVersion}
          entityStatus={serviceStatus}
          {...props}
        />
      </ExpanderContent>
    </Expander>
  );
}
