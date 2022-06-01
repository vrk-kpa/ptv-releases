import React, { FunctionComponent } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box, Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { RhfTextarea } from 'fields';
import { Button } from 'suomifi-ui-components';
import { Mode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { cLink } from 'types/link';
import { VisualHeading } from 'components/VisualHeading';

const useStyles = makeStyles(() => ({
  root: {
    '& p.noTopMargin': {
      marginTop: 0,
    },
  },
}));

interface ServiceLawInterface {
  name: string;
  id: string;
  index: number;
  mode: Mode;
  onRemove?: () => void;
  control: Control<ServiceModel>;
}

export const ServiceLaw: FunctionComponent<ServiceLawInterface> = ({ name, id, index, mode, onRemove, control }) => {
  const { t } = useTranslation();

  const classes = useStyles();

  return (
    <Box key={index} className={classes.root}>
      <Grid container justifyContent='space-between' alignItems='center'>
        <VisualHeading className='noTopMargin' variant='h4'>
          {t('Ptv.Service.Form.Field.Laws.Law.Label')}
        </VisualHeading>
        <Button id={`${name}.${index}.remove`} key='remove' variant='secondaryNoBorder' onClick={onRemove}>
          {t('Ptv.Service.Form.Field.Laws.Remove.Label')}
        </Button>
      </Grid>
      <Box mt={2}>
        <RhfTextarea
          name={`${name}.${index}.${cLink.name}`}
          id={`${id}.${index}.${cLink.name}`}
          mode={mode}
          fullWidth={true}
          visualPlaceholder={t('Ptv.Service.Form.Field.Laws.Name.Placeholder')}
          labelText={t('Ptv.Service.Form.Field.Laws.Name.Label')}
          control={control}
        />
      </Box>
      <Box mt={2}>
        <RhfTextarea
          name={`${name}.${index}.${cLink.url}`}
          id={`${id}.${index}.${cLink.url}`}
          mode={mode}
          fullWidth={true}
          visualPlaceholder={t('Ptv.Service.Form.Field.Laws.Url.Placeholder')}
          hintText={t('Ptv.Service.Form.Field.Laws.Url.Hint')}
          labelText={t('Ptv.Service.Form.Field.Laws.Url.Label')}
          control={control}
        />
      </Box>
    </Box>
  );
};
