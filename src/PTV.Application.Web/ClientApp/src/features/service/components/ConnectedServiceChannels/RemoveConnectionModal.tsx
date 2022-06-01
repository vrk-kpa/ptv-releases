import React from 'react';
import { useTranslation } from 'react-i18next';
import { useQueryClient } from 'react-query';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Button, Modal, ModalContent, ModalFooter, ModalTitle, Paragraph, Text } from 'suomifi-ui-components';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import LoadingIndicator from 'components/LoadingIndicator';
import { Message } from 'components/Message';
import { useFormMetaContext } from 'context/formMeta';
import { Parameters, useExecuteRemoveConnection } from 'hooks/queries/useRemoveConnection';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getServiceModelValue } from 'utils/service';
import { getConnectableChannelValue } from 'utils/serviceChannel';

const useStyles = makeStyles(() => ({
  block: {
    marginTop: '20px',
  },
  action: {
    marginRight: '15px',
  },
  value: {
    marginTop: '5px',
  },
}));

type RemoveConnectionModalProps = {
  channel: ConnectableChannel;
  visible: boolean;
  onClose: () => void;
  getFormValues: () => ServiceFormValues;
  removeServiceChannel: (serviceChannelUnificRootId: string) => void;
};

export function RemoveConnectionModal(props: RemoveConnectionModalProps): React.ReactElement {
  const { t } = useTranslation();
  const { selectedLanguageCode } = useFormMetaContext();
  const classes = useStyles();
  const query = useExecuteRemoveConnection();
  const lang = useGetUiLanguage();
  const service = props.getFormValues();
  const serviceName = getServiceModelValue(service.languageVersions, selectedLanguageCode, (x) => x.name, '');
  const queryClient = useQueryClient();

  function onRemovedSuccessfully(data: unknown, variables: Parameters, context: unknown) {
    query.reset();
    props.onClose();
    props.removeServiceChannel(variables.serviceChannelUnificRootId);
    queryClient.invalidateQueries(['next/connection/for-service'], { active: true });
  }

  function onRemove() {
    if (!service.id || !service.unificRootId) {
      return;
    }

    query.mutate(
      {
        serviceId: service.id,
        serviceChannelUnificRootId: props.channel.unificRootId,
      },
      { onSuccess: onRemovedSuccessfully }
    );
  }

  return (
    <Modal appElementId='root' scrollable={false} visible={props.visible} onEscKeyDown={() => props.onClose()}>
      <ModalContent>
        <ModalTitle>{t('Ptv.Service.Form.RemoveConnection.Dialog.Title')}</ModalTitle>
        <Paragraph>{t('Ptv.Service.Form.RemoveConnection.Dialog.Description')}</Paragraph>
        <div className={classes.block}>
          <Text variant='bold'>{t('Ptv.Service.Form.RemoveConnection.Dialog.Service.Title')}</Text>
        </div>
        <div className={classes.value}>
          <Text>{serviceName}</Text>
        </div>
        <div className={classes.block}>
          <Text variant='bold'>{t('Ptv.Service.Form.RemoveConnection.Dialog.Channel.Title')}</Text>
        </div>
        <div className={classes.value}>
          <Text>{getConnectableChannelValue(props.channel.languageVersions, lang, (x) => x.name, '??')}</Text>
        </div>
        {query.error && (
          <Message className={classes.block} type='error'>
            {t('Ptv.Error.ServerError.Generic', { statusCode: query.error.response?.status })}
          </Message>
        )}
      </ModalContent>
      <ModalFooter>
        <Grid container>
          <Grid item className={classes.action}>
            <Button disabled={query.isLoading} onClick={onRemove}>
              {t('Ptv.Common.Yes')}
            </Button>
          </Grid>
          <Grid item className={classes.action}>
            <Button variant='secondary' onClick={() => props.onClose()}>
              {t('Ptv.Action.Cancel.Label')}
            </Button>
          </Grid>
          {query.isLoading && (
            <Grid item>
              <LoadingIndicator />
            </Grid>
          )}
        </Grid>
      </ModalFooter>
    </Modal>
  );
}
