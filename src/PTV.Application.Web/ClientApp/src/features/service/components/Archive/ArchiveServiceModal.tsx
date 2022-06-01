import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { Button, Modal, ModalContent, ModalFooter, ModalTitle, Paragraph } from 'suomifi-ui-components';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { Language } from 'types/enumTypes';
import LoadingIndicator from 'components/LoadingIndicator';
import { Message } from 'components/Message';
import { useArchiveService } from 'hooks/service/useArchiveService';
import { getKeyForLanguage } from 'utils/translations';

type ArchiveServiceModalProps = {
  serviceId: string | null | undefined;
  language: Language | null | undefined;
  isOpen: boolean;
  success: (data: ServiceApiModel) => void;
  cancel: () => void;
};

export function ArchiveServiceModal(props: ArchiveServiceModalProps): React.ReactElement {
  const { t } = useTranslation();
  const archiveMutation = useArchiveService(props.serviceId ?? '');

  function archiveService() {
    archiveMutation.mutate(props.language, { onSuccess: props.success });
  }

  function onCancel() {
    archiveMutation.reset();
    props.cancel();
  }

  const title = props.language ? 'Ptv.Service.Form.Archive.ArchiveLanguageVersion.Title' : 'Ptv.Service.Form.Archive.ArchiveService.Title';
  const description = props.language
    ? 'Ptv.Service.Form.Archive.ArchiveLanguageVersion.Description'
    : 'Ptv.Service.Form.Archive.ArchiveService.Description';
  const langText = props.language ? t(getKeyForLanguage(props.language)) : '';

  return (
    <Modal appElementId='root' visible={props.isOpen} onEscKeyDown={onCancel}>
      <ModalContent>
        <ModalTitle>{t(title)}</ModalTitle>
        <Paragraph>{t(description, { language: langText })}</Paragraph>
        {archiveMutation.error && <Message type='error'>{t('Ptv.Error.ServerError.ArchiveFailed')}</Message>}
      </ModalContent>
      <ModalFooter>
        <Grid container columnSpacing={1}>
          <Grid item>
            <Button variant='default' disabled={archiveMutation.isLoading} onClick={archiveService}>
              {t('Ptv.Common.Yes')}
            </Button>
          </Grid>
          <Grid item>
            <Button variant='secondary' onClick={onCancel}>
              {t('Ptv.Action.Cancel.Label')}
            </Button>
          </Grid>
          {archiveMutation.isLoading && (
            <Grid item>
              <LoadingIndicator />
            </Grid>
          )}
        </Grid>
      </ModalFooter>
    </Modal>
  );
}
