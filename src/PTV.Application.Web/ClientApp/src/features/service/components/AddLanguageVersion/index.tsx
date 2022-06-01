import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Button } from 'suomifi-ui-components';
import { LanguageVersionWithName } from 'types/languageVersionTypes';
import { AddLanguageVersionModal } from './AddLanguageVersionModal';

type AddLanguageVersionProps = {
  disabled: boolean;
  addLanguages: (languages: LanguageVersionWithName[]) => void;
  getExistingLanguages: () => LanguageVersionWithName[];
  hasGeneralDescription: boolean;
};

export function AddLanguageVersion(props: AddLanguageVersionProps): React.ReactElement {
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const { t } = useTranslation();

  function closeSaveChanges(languages: LanguageVersionWithName[]) {
    props.addLanguages(languages);
    setIsOpen(false);
  }

  return (
    <div>
      <Button id='add-languageversion-button' variant='secondary' icon='plus' onClick={() => setIsOpen(true)} disabled={props.disabled}>
        {t('Ptv.Service.Form.Empty.AddLanguage')}
      </Button>
      <AddLanguageVersionModal
        isOpen={isOpen}
        closeDiscardChanges={() => setIsOpen(false)}
        closeSaveChanges={closeSaveChanges}
        getExistingLanguages={props.getExistingLanguages}
        hasGeneralDescription={props.hasGeneralDescription}
      />
    </div>
  );
}
