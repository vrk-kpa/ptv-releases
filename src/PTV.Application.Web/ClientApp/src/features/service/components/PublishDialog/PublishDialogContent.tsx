import React, { useState } from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { styled } from '@mui/material/styles';
import { DateTime } from 'luxon';
import { Block, Button, InlineAlert, ModalContent, ModalFooter, ModalTitle, Paragraph } from 'suomifi-ui-components';
import { getKeys } from 'utils';
import { ServiceApiLanguageModel, ServiceApiModel } from 'types/api/serviceApiModel';
import { ValidationModel } from 'types/api/validationModel';
import { Language, language } from 'types/enumTypes';
import { ServiceFormValues, ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { LanguageVersionType, RequiredLanguageVersionType } from 'types/languageVersionTypes';
import { Message } from 'components/Message';
import { toServiceApiModel } from 'mappers/serviceMapper';
import { PublishTable } from './PublishTable';
import { PublishDialogItem } from './index';

const StyledMessage = styled(Message)(({ theme }) => ({
  display: 'inline-block',
  marginRight: theme.suomifi.spacing.s,
}));

const minScheduleDate = DateTime.now().plus({ days: 1 });
const maxScheduleDate = minScheduleDate.plus({ months: 18 });

const isValidDate = (date: DateTime) => {
  return date.startOf('day') >= minScheduleDate.startOf('day') && date.startOf('day') <= maxScheduleDate.startOf('day');
};

function getInitialState(apiModel: ServiceApiModel) {
  const initialState = language.reduce((items, lang: Language) => {
    items[lang] = createInitialDialogItem(lang, apiModel.languageVersions[lang]);
    return items;
  }, {} as RequiredLanguageVersionType<PublishDialogItem>);
  validatePublishStateAndSetErrors(initialState);
  return initialState;
}

function createInitialDialogItem(language: Language, model: ServiceApiLanguageModel | undefined): PublishDialogItem {
  const archiveDate = !!model?.scheduledArchive ? DateTime.fromISO(model.scheduledArchive) : maxScheduleDate;
  const publishDate = !!model?.scheduledPublish ? DateTime.fromISO(model.scheduledPublish) : minScheduleDate;
  return {
    language,
    checked: !!model?.scheduledArchive || !!model?.scheduledPublish,
    scheduledArchive: !!model?.scheduledArchive,
    scheduledArchiveDate: archiveDate,
    isValidArchiveDate: true,
    scheduledPublish: !!model?.scheduledPublish,
    scheduledPublishDate: publishDate,
    isValidPublishDate: true,
    isValidDateCombination: true,
  };
}

function getHasValidPublishAndArchiveDates(languageItem: PublishDialogItem) {
  if (languageItem.scheduledArchive && languageItem.scheduledPublish) {
    return languageItem.scheduledPublishDate < languageItem.scheduledArchiveDate;
  }
  return true;
}

function validatePublishStateAndSetErrors(state: RequiredLanguageVersionType<PublishDialogItem>) {
  language.forEach((lang) => {
    state[lang].isValidPublishDate = isValidDate(state[lang].scheduledPublishDate);
    state[lang].isValidDateCombination =
      !state[lang].scheduledPublishDate.isValid || !state[lang].scheduledArchiveDate.isValid
        ? true
        : getHasValidPublishAndArchiveDates(state[lang]);
    state[lang].isValidArchiveDate = isValidDate(state[lang].scheduledArchiveDate);
  });
}

type PublishDialogContentProps = {
  control: Control<ServiceModel>;
  isLoading: boolean;
  hasError: boolean;
  errorKey: string;
  validationResult: ValidationModel<ServiceApiModel> | undefined;
  onPublish: (publishState: LanguageVersionType<PublishDialogItem>) => void;
  onCancel: () => void;
  getFormValues: () => ServiceFormValues;
};

export function PublishDialogContent(props: PublishDialogContentProps): React.ReactElement {
  const { t } = useTranslation();
  const lastTranslations = useWatch({ control: props.control, name: `${cService.lastTranslations}` });
  const serviceApiModel = toServiceApiModel(props.getFormValues());

  // Keep track of the changes user makes (e.g. publish date, archiving date etc.)
  const [publishState, setPublishState] = useState<RequiredLanguageVersionType<PublishDialogItem>>(getInitialState(serviceApiModel));
  const [dialogHasValidationError, setDialogHasValidationError] = useState(false);

  const validateAndSetPublishState = (state: RequiredLanguageVersionType<PublishDialogItem>) => {
    const newState = { ...state };
    validatePublishStateAndSetErrors(newState);
    setPublishState(newState);
    setDialogHasValidationError(!areAllDatesValid(newState));
  };

  const areAllDatesValid = (state: RequiredLanguageVersionType<PublishDialogItem>) => {
    return getKeys(state).every(
      (language) =>
        !state[language].checked ||
        (((state[language].scheduledPublish && isValidDate(state[language].scheduledPublishDate)) || !state[language].scheduledPublish) &&
          ((state[language].scheduledArchive && isValidDate(state[language].scheduledArchiveDate)) || !state[language].scheduledArchive) &&
          getHasValidPublishAndArchiveDates(state[language]))
    );
  };

  const isAnySelected = (state: RequiredLanguageVersionType<PublishDialogItem>) => {
    for (const lang of language) {
      if (state[lang].checked) return true;
    }
    return false;
  };

  const publishButtonEnabled =
    !props.isLoading && isAnySelected(publishState) && !serviceApiModel.otherModifiedVersion && !dialogHasValidationError;

  return (
    <>
      <ModalContent>
        <ModalTitle>{t(`Ptv.PublishingDialog.Label`)}</ModalTitle>
        <Block>
          <Box mb={2}>
            <Paragraph>{t(`Ptv.PublishingDialog.Description`)}</Paragraph>
          </Box>
          <PublishTable
            lastTranslations={lastTranslations}
            validationResult={props.validationResult}
            isLoading={props.isLoading}
            publishState={publishState}
            minScheduleDate={minScheduleDate}
            maxScheduleDate={maxScheduleDate}
            setPublishState={(data) => validateAndSetPublishState(data)}
            languageVersions={serviceApiModel.languageVersions}
          />
        </Block>
      </ModalContent>
      <ModalFooter>
        {dialogHasValidationError && <InlineAlert status='error'>{t('Ptv.PublishingDialog.InvalidDateOrScheduleAlert')}</InlineAlert>}
        <Button
          onClick={() => areAllDatesValid(publishState) && props.onPublish(publishState)}
          id='publishing-dialog-publish'
          disabled={!publishButtonEnabled}
        >
          {t('Ptv.Form.Publish.Text')}
        </Button>

        <Button variant='secondary' onClick={props.onCancel} id='publishing-dialog-cancel'>
          {t('Ptv.Form.Cancel.Text')}
        </Button>
        {props.hasError && <StyledMessage type='error'>{t(props.errorKey)}</StyledMessage>}
      </ModalFooter>
    </>
  );
}
