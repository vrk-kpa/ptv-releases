import React, { FunctionComponent, RefObject } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { RhfTextarea } from 'fields';
import { Button } from 'suomifi-ui-components';
import { Mode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';

const useStyles = makeStyles((theme) => ({
  item: {
    display: 'flex',
    flexGrow: 1,
    border: `1px solid rgb(200, 205, 208)`,
    padding: '12px 20px 20px 20px',
    marginBottom: '20px',
  },
  left: {
    display: 'flex',
    flexGrow: 1,
    paddingTop: '8px',
  },
  right: {
    display: 'flex',
    flexShrink: 0,
    marginLeft: 'auto',
    alignItems: 'flex-start',
    paddingTop: '0px',
  },
}));

interface AdditionalInfoInterface {
  forwardedRef?: RefObject<HTMLTextAreaElement>;
  name: string;
  onRemove?: () => void;
  mode: Mode;
  control: Control<ServiceModel>;
}

export const AdditionalInfo: FunctionComponent<AdditionalInfoInterface> = ({ name, mode, control, onRemove, forwardedRef }) => {
  const { t } = useTranslation();
  const classes = useStyles();

  return (
    <div className={classes.item}>
      <div className={classes.left}>
        <RhfTextarea
          forwardedRef={forwardedRef}
          control={control}
          name={name}
          id={name}
          mode={mode}
          fullWidth={true}
          visualPlaceholder={t('Ptv.Service.Form.Field.ServiceProviders.ProducerAdditionalInformation.Placeholder')}
          labelText={t('Ptv.Service.Form.Field.ServiceProviders.ProducerAdditionalInformation.Label')}
        />
      </div>
      <div className={classes.right}>
        <Button icon='remove' key='remove' variant='secondaryNoBorder' onClick={onRemove}>
          {t('Ptv.Service.Form.Field.ServiceProviders.ProducerAdditionalInformation.Remove.Label')}
        </Button>
      </div>
    </div>
  );
};
