import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Button, Modal, ModalContent, ModalFooter, ModalTitle, Paragraph } from 'suomifi-ui-components';
import { LanguageVersionWithName } from 'types/languageVersionTypes';
import { AddLanguageVersionContent } from './AddLanguageVersionContent';

type AddLanguageVersionModalProps = {
  isOpen: boolean;
  closeDiscardChanges: () => void;
  closeSaveChanges: (languages: LanguageVersionWithName[]) => void;
  getExistingLanguages: () => LanguageVersionWithName[];
  hasGeneralDescription: boolean;
};

export function AddLanguageVersionModal(props: AddLanguageVersionModalProps): React.ReactElement | null {
  const [selected, setSelected] = useState<LanguageVersionWithName[]>([]);
  const { t } = useTranslation();

  const addLanguages = () => {
    props.closeSaveChanges(selected);
  };

  const addButtonEnabled = () => {
    return (
      selected.length > 0 &&
      selected.every((languageVersion) => {
        return languageVersion.name.length > 0;
      })
    );
  };

  const handleChange = (selected: LanguageVersionWithName[]) => {
    setSelected(selected);
  };

  return (
    <Modal appElementId='root' visible={props.isOpen} onEscKeyDown={props.closeDiscardChanges}>
      <ModalContent>
        <ModalTitle>{t('Ptv.Service.Form.Empty.Modal.Title')}</ModalTitle>
        <Paragraph>{t('Ptv.Service.Form.Empty.Modal.Description')}</Paragraph>
        <AddLanguageVersionContent
          getExistingLanguages={props.getExistingLanguages}
          hasGeneralDescription={props.hasGeneralDescription}
          onChange={handleChange}
        />
      </ModalContent>
      <ModalFooter>
        <Button id='add.language.versions.confirm' onClick={addLanguages} disabled={!addButtonEnabled()}>
          {t('Ptv.Service.Form.Empty.Modal.Add')}
        </Button>
        <Button id='add.language.versions.cancel' onClick={props.closeDiscardChanges} variant='secondary'>
          {t('Ptv.Service.Form.Empty.Modal.Cancel')}
        </Button>
      </ModalFooter>
    </Modal>
  );
}
