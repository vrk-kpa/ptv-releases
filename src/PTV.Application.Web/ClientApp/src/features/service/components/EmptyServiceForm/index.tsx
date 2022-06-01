import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router';
import { styled } from '@mui/material/styles';
import { Button, Heading, Paragraph, Text } from 'suomifi-ui-components';
import { LanguageVersionWithName } from 'types/languageVersionTypes';
import { FormFieldArea } from 'components/formLayout/FormFieldArea';
import { AddLanguageVersionContent } from 'features/service/components/AddLanguageVersion/AddLanguageVersionContent';

type EmptyServiceFormProps = {
  className?: string;
  addLanguages: (languages: LanguageVersionWithName[]) => void;
};

export const EmptyServiceForm = styled((props: EmptyServiceFormProps): React.ReactElement => {
  const { t } = useTranslation();
  const [selectedLanguageVersions, setSelectedLanguageVersions] = useState<LanguageVersionWithName[]>([]);
  const navigate = useNavigate();

  const goToSearch = () => {
    navigate('/frontpage/search');
  };

  const getExistingLanguages = (): LanguageVersionWithName[] => [];

  const addButtonEnabled = () => {
    return (
      selectedLanguageVersions.length > 0 &&
      selectedLanguageVersions.every((languageVersion) => {
        return languageVersion.name.length > 0;
      })
    );
  };

  return (
    <FormFieldArea className={props.className}>
      <Heading variant='h1'>{t('Ptv.Service.Form.Empty.Title')}</Heading>
      <Paragraph>
        <Text className='custom-message'>{t('Ptv.Service.Form.Empty.Message')}</Text>
      </Paragraph>
      <AddLanguageVersionContent
        hasGeneralDescription={false}
        getExistingLanguages={getExistingLanguages}
        onChange={(languageVersions) => setSelectedLanguageVersions(languageVersions)}
      />
      <div className='button-bar'>
        <Button
          id='add.language.versions.confirm'
          onClick={() => props.addLanguages(selectedLanguageVersions)}
          aria-disabled={!addButtonEnabled()}
        >
          {t('Ptv.Service.Form.Empty.CreateService')}
        </Button>
        <Button id='add.language.versions.cancel' onClick={goToSearch} variant='secondary'>
          {t('Ptv.Service.Form.Empty.Modal.Cancel')}
        </Button>
      </div>
    </FormFieldArea>
  );
})(({ theme }) => ({
  '& .button-bar': {
    display: 'flex',
    justifyContent: 'flex-start',
    gap: '15px',
    marginTop: '30px',
    marginBottom: '15px',
  },
  '& .custom-message': {
    fontSize: '16px',
    marginTop: '10px',
    marginBottom: '20px',
  },
}));
