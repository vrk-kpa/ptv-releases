import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { Button, Modal, ModalContent, ModalFooter, ModalTitle, Paragraph } from 'suomifi-ui-components';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { Language } from 'types/enumTypes';
import LoadingIndicator from 'components/LoadingIndicator';
import { Message } from 'components/Message';
import { useRestoreService } from 'hooks/service/useRestoreService';
import { getKeyForLanguage } from 'utils/translations';

type RestoreServiceModalProps = {
  serviceId: string | null | undefined;
  language: Language | null | undefined;
  isOpen: boolean;
  success: (data: ServiceApiModel) => void;
  cancel: () => void;
};

export function RestoreServiceModal(props: RestoreServiceModalProps): React.ReactElement {
  const { t } = useTranslation();
  const restoreMutation = useRestoreService(props.serviceId ?? '');

  function restoreService() {
    restoreMutation.mutate(props.language, { onSuccess: props.success });
  }

  function onCancel() {
    restoreMutation.reset();
    props.cancel();
  }

  const title = props.language ? 'Ptv.Service.Form.Restore.RestoreLanguageVersion.Title' : 'Ptv.Service.Form.Restore.RestoreService.Title';
  const description = props.language
    ? 'Ptv.Service.Form.Restore.RestoreLanguageVersion.Description'
    : 'Ptv.Service.Form.Restore.RestoreService.Description';
  const langText = props.language ? t(getKeyForLanguage(props.language)) : '';

  return (
    <Modal appElementId='root' visible={props.isOpen} onEscKeyDown={onCancel}>
      <ModalContent>
        <ModalTitle>{t(title)}</ModalTitle>
        <Paragraph>{t(description, { language: langText })}</Paragraph>
        {restoreMutation.error && <Message type='error'>{t('Ptv.Error.ServerError.RestoreFailed')}</Message>}
      </ModalContent>
      <ModalFooter>
        <Grid container columnSpacing={1}>
          <Grid item>
            <Button variant='default' disabled={restoreMutation.isLoading} onClick={restoreService}>
              {t('Ptv.Common.Yes')}
            </Button>
          </Grid>
          <Grid item>
            <Button variant='secondary' onClick={onCancel}>
              {t('Ptv.Action.Cancel.Label')}
            </Button>
          </Grid>
          {restoreMutation.isLoading && (
            <Grid item>
              <LoadingIndicator />
            </Grid>
          )}
        </Grid>
      </ModalFooter>
    </Modal>
  );
}
