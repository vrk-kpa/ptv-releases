import React from 'react';
import { useTranslation } from 'react-i18next';
import { Button, Modal, ModalContent, ModalFooter, ModalTitle, Paragraph } from 'suomifi-ui-components';

export type ConfirmationModalProps = {
  isOpen: boolean;
  title: string;
  content: string;
  cancel: () => void;
  confirm: () => void;
};

export function ConfirmationModal(props: ConfirmationModalProps): React.ReactElement {
  const { t } = useTranslation();
  return (
    <Modal appElementId='root' visible={props.isOpen} onEscKeyDown={props.cancel}>
      <ModalContent>
        <ModalTitle>{props.title}</ModalTitle>
        <Paragraph>{props.content}</Paragraph>
      </ModalContent>
      <ModalFooter>
        <Button onClick={props.confirm} id='confirm-cancel-yes'>
          {t('Ptv.Form.Cancel.ConfirmCancelDialog.Yes')}
        </Button>
        <Button variant='secondary' onClick={props.cancel} id='confirm-cancel-no'>
          {t('Ptv.Form.Cancel.ConfirmCancelDialog.No')}
        </Button>
      </ModalFooter>
    </Modal>
  );
}
