import React from 'react';
import { useTranslation } from 'react-i18next';
import { Button, Modal, ModalContent, ModalFooter, ModalTitle, Paragraph } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { getGdValueOrDefault } from 'utils/gd';

type RemoveGdModalProps = {
  gd: GeneralDescriptionModel;
  visible: boolean;
  onCancel: () => void;
  onOk: () => void;
};

export function RemoveGdModal(props: RemoveGdModalProps): React.ReactElement {
  const { t, i18n } = useTranslation();

  const name = getGdValueOrDefault(props.gd.languageVersions, i18n.language as Language, (lv) => lv.name, '');

  return (
    <Modal appElementId='root' scrollable={false} visible={props.visible} onEscKeyDown={() => props.onCancel()}>
      <ModalContent>
        <ModalTitle>{t('Ptv.Service.Form.GdRemove.Title.Label')}</ModalTitle>
        <Paragraph>{t('Ptv.Service.Form.GdRemove.Content.Text', { name: name })}</Paragraph>
      </ModalContent>
      <ModalFooter>
        <Button onClick={() => props.onOk()}>{t('Ptv.Common.Yes')}</Button>
        <Button variant='secondary' onClick={() => props.onCancel()}>
          {t('Ptv.Action.Cancel.Label')}
        </Button>
      </ModalFooter>
    </Modal>
  );
}
