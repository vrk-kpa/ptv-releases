import React, { useContext, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { makeStyles } from '@mui/styles';
import {
  Button,
  Checkbox,
  Dropdown,
  DropdownItem,
  HintText,
  Label,
  Link,
  Modal,
  ModalContent,
  ModalFooter,
  ModalTitle,
  Paragraph,
  TextInput,
  Textarea,
} from 'suomifi-ui-components';
import { getKeys } from 'utils';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { Language, translatableLanguage } from 'types/enumTypes';
import { ServiceModelLangaugeVersionsValuesType } from 'types/forms/serviceFormTypes';
import { LastTranslationType } from 'types/forms/translationTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import LoadingIndicator from 'components/LoadingIndicator';
import { AppContext } from 'context/AppContextProvider';
import { Fieldset } from 'fields/Fieldset/Fieldset';
import { Legend } from 'fields/Legend/Legend';
import { useOrderTranslation } from 'hooks/translationOrders/useOrderTranslation';
import { getUniqueInTranslationSourceLangs } from 'utils/languageVersions';
import { getOrderedLanguageVersionKeys, languagesSort } from 'utils/languages';
import { getKeyForLanguage } from 'utils/translations';

const useStyles = makeStyles(() => ({
  button: {
    marginTop: '20px !important',
  },
  modalContent: {
    '& .fi-hint-text.custom': {
      marginBottom: '10px;',
    },
    '& .fi-label-text.custom': {
      marginTop: '20px',
      marginBottom: '10px',
    },
  },
  label: {
    marginBottom: '10px',
  },
}));

type TranslationsModalProps = {
  serviceId: string | null | undefined;
  languageVersions: ServiceModelLangaugeVersionsValuesType;
  lastTranslations: LastTranslationType[];
  canTranslate: boolean;
  translationOrderSuccess: (data: ServiceApiModel) => void;
};

export default function TranslationsModal(props: TranslationsModalProps): React.ReactElement {
  const { serviceId, languageVersions } = props;
  const { t } = useTranslation();
  const classes = useStyles();
  const { userInfo } = useContext(AppContext);

  // Refs for submitting data
  const additionalInfoRef = useRef<HTMLTextAreaElement>(null);
  const subscriberRef = useRef<HTMLInputElement>(null);
  const emailRef = useRef<HTMLInputElement>(null);

  const orderMutation = useOrderTranslation(serviceId ?? '', 'Service');

  const [isOpen, setIsOpen] = useState<boolean>(false);

  /*
   * Source langs must meet translation criteria,
   * be defined as translatable,
   * cannot be target langs for "in progress" translations
   */
  const getSourceLanguages = () =>
    getKeys(languageVersions)
      .filter(
        (langVersion) =>
          translatableLanguage.some((translatableLang) => langVersion === translatableLang) &&
          languageVersions[langVersion]?.translationAvailability?.canBeTranslated &&
          !languageVersions[langVersion]?.translationAvailability?.isInTranslation &&
          !languageVersions[langVersion].scheduledArchive
      )
      .sort(languagesSort);

  const inTranslationSourceLangs = getUniqueInTranslationSourceLangs(props.lastTranslations);

  /**
   * Target langs must be defined as translatable,
   * cannot be sources or targets for "in progress" translations
   */
  const targetLanguages: LanguageVersionType<boolean> = translatableLanguage
    .filter((lang) => !inTranslationSourceLangs.some((ln) => ln === lang))
    .reduce((targets, current) => ({ ...targets, [current]: false }), {});

  const [source, setSource] = useState<Language>(getSourceLanguages()[0]);

  const [targets, setTargets] = useState<LanguageVersionType<boolean>>(
    getKeys(targetLanguages).reduce(
      (targets, targetLang) => (targetLang === getSourceLanguages()[0] ? targets : { ...targets, [targetLang]: false }),
      {}
    )
  );

  const handleClose = () => setIsOpen(false);

  const changeSource = (newSource: Language) => {
    setSource(newSource);
    const newTargets: LanguageVersionType<boolean> = {};
    getKeys(targetLanguages).forEach((language) => {
      if (language !== newSource) {
        newTargets[language] = targets[language] ?? false;
      }
    });
    setTargets(newTargets);
  };

  const getTargetKeys = () => getOrderedLanguageVersionKeys(targets);

  const toggleTarget = (language: Language) => {
    const newTargets = { ...targets };
    newTargets[language] = !targets[language];
    setTargets(newTargets);
  };

  const canSendOrder = getTargetKeys().some((ln) => targets[ln]) && !!source;

  const onSuccess = (data: ServiceApiModel) => {
    // reset the target state after successful request.
    const newTargets = { ...targets };
    getKeys(newTargets).forEach((language) => (newTargets[language] = false));
    setTargets(newTargets);
    setIsOpen(false);
    props.translationOrderSuccess(data);
  };

  const sendOrder = () => {
    orderMutation.mutate(
      {
        additionalInformation: additionalInfoRef.current?.value,
        email: emailRef.current?.value ?? '',
        source: source,
        subscriber: subscriberRef.current?.value ?? '',
        targets: getTargetKeys().filter((key) => !!targets[key]),
      },
      { onSuccess: onSuccess }
    );
  };

  return (
    <div>
      <Button
        variant='secondary'
        disabled={!props.canTranslate}
        icon='chatQuestion'
        className={classes.button}
        onClick={() => setIsOpen(true)}
      >
        {t('Ptv.Form.Header.Translate.Button')}
      </Button>

      <Modal appElementId='root' visible={isOpen} onEscKeyDown={handleClose}>
        <ModalContent className={classes.modalContent}>
          <ModalTitle>{t('Ptv.Form.Header.Translate.Modal.Title')}</ModalTitle>
          <Paragraph>
            {t('Ptv.Form.Header.Translate.Modal.Description')}
            <Link href='mailto:ptv@lingsoft.fi'>ptv@lingsoft.fi</Link>
          </Paragraph>
          <Label className='custom' id='translate-modal-source-label'>
            {t('Ptv.Form.Header.Translate.Modal.Source')}
          </Label>
          <HintText className='custom' id='translate-modal-source-hint-text'>
            {t('Ptv.Form.Header.Translate.Modal.Dropdown')}
          </HintText>
          <Dropdown
            labelText=''
            aria-labelledby='translate-modal-source-label translate-modal-source-hint-text'
            name='translation-source'
            labelMode='hidden'
            defaultValue={source}
            onChange={(value) => changeSource(value as Language)}
          >
            {getSourceLanguages().map((language) => (
              <DropdownItem key={language} value={language}>
                {t(getKeyForLanguage(language))}
              </DropdownItem>
            ))}
          </Dropdown>
          <Box mt={2} mb={1}>
            <Fieldset>
              <Legend>{t('Ptv.Form.Header.Translate.Modal.Target')}</Legend>
              {getTargetKeys().map((key) => {
                // if translation is already ordered, disable and show as checked. Not reflected to state / new orders.
                // if scheduled for publish or archive, disable and show as not checked.
                const translationOrderInProgress = languageVersions[key].translationAvailability?.isInTranslation;
                const scheduledPublishOrArchive = !!languageVersions[key].scheduledPublish || !!languageVersions[key].scheduledArchive;
                return (
                  <Checkbox
                    key={key}
                    checked={!!(targets[key] || translationOrderInProgress)}
                    disabled={translationOrderInProgress || scheduledPublishOrArchive}
                    onClick={() => toggleTarget(key)}
                  >
                    {t(getKeyForLanguage(key))}
                  </Checkbox>
                );
              })}
            </Fieldset>
          </Box>
          <Box mt={2}>
            <Textarea
              name='translation-additional-info'
              id='translation-additional-info'
              fullWidth={true}
              ref={additionalInfoRef}
              optionalText={t('Ptv.Common.Optional')}
              visualPlaceholder={t('Ptv.Form.Header.Translate.Modal.AdditionalDescription')}
              labelText={t('Ptv.Form.Header.Translate.Modal.AdditionalTitle')}
            />
          </Box>
          <Box mt={2}>
            <TextInput
              name='translation-subscriber'
              id='translation-subscriber'
              fullWidth={true}
              ref={subscriberRef}
              labelText={t('Ptv.Form.Header.Translate.Modal.Subscriber.Title')}
              defaultValue={userInfo?.name}
            />
          </Box>
          <Box mt={2}>
            <TextInput
              name='translation-email'
              id='translation-email'
              fullWidth={true}
              ref={emailRef}
              labelText={t('Ptv.Form.Header.Translate.Modal.Email.Title')}
              defaultValue={userInfo?.email}
            />
          </Box>
        </ModalContent>
        <ModalFooter>
          <Button onClick={sendOrder} disabled={!canSendOrder || orderMutation.isLoading}>
            {orderMutation.isLoading ? <LoadingIndicator size='20px' /> : t('Ptv.Form.Header.Translate.Modal.SendOrder')}
          </Button>
          <Button onClick={handleClose} variant='secondary' disabled={orderMutation.isLoading}>
            {t('Ptv.Action.Cancel.Label')}
          </Button>
        </ModalFooter>
      </Modal>
    </div>
  );
}
