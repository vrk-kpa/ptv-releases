import React, { useState } from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Button, Checkbox, Heading, Modal, ModalContent, ModalFooter, ModalTitle, Paragraph } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceFormValues, ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { useFormMetaContext } from 'context/formMeta';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { Fieldset } from 'fields/Fieldset/Fieldset';
import { Legend } from 'fields/Legend/Legend';
import { copyService, getEnabledLanguagesByPriority } from 'utils/service';
import { getKeyForLanguage } from 'utils/translations';
import { getUserOrganization } from 'utils/userInfo';

const useStyles = makeStyles((_theme) => ({
  button: {
    marginTop: '20px !important',
  },
}));

type CopyProps = {
  control: Control<ServiceModel>;
  getFormValues: () => ServiceFormValues;
  onServiceCopied: (copy: ServiceFormValues) => void;
};

export default function Copy(props: CopyProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const { mode } = useFormMetaContext();

  const serviceId = useWatch({ control: props.control, name: `${cService.id}` });
  const serviceStatus = useWatch({ control: props.control, name: `${cService.status}` });
  const languageVersions = useWatch({ control: props.control, name: `${cService.languageVersions}`, exact: true });

  const canCopy = !!serviceId && serviceStatus !== 'Removed' && mode === 'view';
  const appContext = useAppContextOrThrow();

  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [selectedLanguages, setSelectedLanguages] = useState<Language[]>([]);
  const languages: Language[] = getEnabledLanguagesByPriority(languageVersions);

  const handleClose = () => setIsOpen(false);

  const handleOpen = () => setIsOpen(true);

  const toggleSelected = (language: Language) => {
    if (selectedLanguages.includes(language)) {
      setSelectedLanguages(selectedLanguages.filter((x) => x !== language));
    } else {
      setSelectedLanguages([...selectedLanguages, language]);
    }
  };

  const isAnySelected = () => selectedLanguages.length !== 0;

  const createCopy = () => {
    const copy = copyService(props.getFormValues(), getUserOrganization(appContext), selectedLanguages);
    props.onServiceCopied(copy);
  };

  return (
    <div>
      <Heading variant='h4' as='h3'>
        {t('Ptv.Form.Header.Copy.Title')}
      </Heading>
      <Paragraph>{t('Ptv.Form.Header.Copy.Description')}</Paragraph>
      <Button disabled={!canCopy} variant='secondary' icon='copy' className={classes.button} onClick={handleOpen}>
        {t('Ptv.Form.Header.Copy.Button')}
      </Button>

      <Modal appElementId='root' visible={isOpen} onEscKeyDown={handleClose}>
        <ModalContent>
          <ModalTitle>{t('Ptv.Form.Header.Copy.Modal.Title')}</ModalTitle>
          <Paragraph>{t('Ptv.Form.Header.Copy.Modal.Description')}</Paragraph>
          <Box mt={2}>
            <Fieldset>
              <Legend>{t('Ptv.Form.Header.Copy.Modal.Subtitle')}</Legend>
              {languages.map((ln) => {
                const languageVersion = languageVersions[ln];
                return (
                  <Checkbox
                    id={`copy-language-version-${languageVersion.language}`}
                    key={languageVersion.language}
                    hintText={languageVersion.name}
                    checked={selectedLanguages.includes(ln)}
                    onClick={() => toggleSelected(ln)}
                  >
                    {t(getKeyForLanguage(ln))}
                  </Checkbox>
                );
              })}
            </Fieldset>
          </Box>
        </ModalContent>
        <ModalFooter>
          <Button onClick={createCopy} disabled={!isAnySelected()}>
            {t('Ptv.Form.Header.Copy.Modal.Button')}
          </Button>
          <Button onClick={handleClose} variant='secondary'>
            {t('Ptv.Action.Cancel.Label')}
          </Button>
        </ModalFooter>
      </Modal>
    </div>
  );
}
