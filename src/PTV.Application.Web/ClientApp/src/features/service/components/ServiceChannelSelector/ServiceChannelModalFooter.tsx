import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Button, ModalFooter } from 'suomifi-ui-components';
import LoadingIndicator from 'components/LoadingIndicator';
import { ViewType } from './utils';

const useStyles = makeStyles(() => ({
  action: {
    '& .fi-button': {
      marginRight: '15px',
    },
  },
  close: {
    '& .fi-button': {
      marginRight: '15px',
    },
  },
}));

type ServiceChannelModalFooterProps = {
  selectedChannelsCount: number;
  showSummary: () => void;
  closeAndDiscardChanges: () => void;
  saveChanges: () => void;
  activeView: ViewType;
  isLoading: boolean;
};

export default function ServiceChannelModalFooter(props: ServiceChannelModalFooterProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const cannotSave = props.isLoading || props.selectedChannelsCount === 0;

  function allowToViewSummary(): boolean {
    return props.activeView === 'SearchView' && props.selectedChannelsCount > 0;
  }

  return (
    <ModalFooter>
      {allowToViewSummary() && (
        <Button onClick={props.showSummary} icon='arrowRight' variant='secondaryNoBorder'>
          {t('Ptv.Service.Form.ServiceChannelSelector.ShowSelectedChannels.Button.Label') + ` (${props.selectedChannelsCount})`}
        </Button>
      )}

      <Grid container>
        <Grid item className={classes.action}>
          <Button onClick={props.saveChanges} disabled={cannotSave}>
            {t('Ptv.Service.Form.ServiceChannelSelector.SaveChanges.Button.Label')}
          </Button>
        </Grid>
        <Grid item className={classes.close}>
          <Button variant='secondary' onClick={() => props.closeAndDiscardChanges()}>
            {t('Ptv.Service.Form.ServiceChannelSelector.DischardChanges.Button.Label')}
          </Button>
        </Grid>
        {props.isLoading && (
          <Grid item>
            <LoadingIndicator />
          </Grid>
        )}
      </Grid>
    </ModalFooter>
  );
}
