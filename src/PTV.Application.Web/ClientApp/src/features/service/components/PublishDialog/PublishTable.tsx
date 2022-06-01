import React from 'react';
import { useTranslation } from 'react-i18next';
import { styled } from '@mui/material/styles';
import { DateTime } from 'luxon';
import { Block, Checkbox, Icon, Label, RadioButton, RadioButtonGroup, Text } from 'suomifi-ui-components';
import { LanguageVersionBaseModel } from 'types/api/entityBaseModel';
import { ServiceApiLanguageModel, ServiceApiModel } from 'types/api/serviceApiModel';
import { ValidationModel } from 'types/api/validationModel';
import { Language, allowedLanguageVersionStatusesToPublish } from 'types/enumTypes';
import { LastTranslationType } from 'types/forms/translationTypes';
import { LanguageVersionType, RequiredLanguageVersionType } from 'types/languageVersionTypes';
import LoadingIndicator from 'components/LoadingIndicator';
import { Message } from 'components/Message';
import { VisualHeading } from 'components/VisualHeading';
import { FormDivider } from 'components/formLayout/FormDivider';
import { getUniqueInTranslationSourceLangs } from 'utils/languageVersions';
import { getOrderedLanguageVersionKeys } from 'utils/languages';
import { getKeyForLanguage, getKeysForStatusType } from 'utils/translations';
import { ScheduleLangVersion } from './ScheduleLangVersion';
import { PublishDialogItem, StyledDiv } from './index';

const LangVersionTitleContent = styled('div')`
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: baseline;
`;

const ScheduleWrapper = styled(Block)(({ theme }) => ({
  '&.fi-block': {
    paddingLeft: '25px',
    marginTop: theme.suomifi.spacing.m,
    marginBottom: theme.suomifi.spacing.m,
    '& .fi-radio-button-group': {
      marginTop: theme.suomifi.spacing.m,
      marginBottom: theme.suomifi.spacing.m,
    },
    '& .fi-checkbox': {
      marginTop: theme.suomifi.spacing.m,
      marginBottom: theme.suomifi.spacing.m,
    },
    '& .fi-alert': {
      display: 'inline-block',
      marginBottom: theme.suomifi.spacing.m,
    },
  },
}));

const LanguageVersionWrapper = styled(Block)(({ theme }) => ({
  '& .divider': {
    marginTop: theme.suomifi.spacing.m,
    marginBottom: theme.suomifi.spacing.m,
  },
}));

const ErrorInfo = styled((props: { errorText: string; className?: string }) => (
  <Text className={props.className}>
    <Icon icon='radioButtonOn' fill='red' />
    {props.errorText}
  </Text>
))(({ theme }) => ({
  '&.fi-text': {
    fontSize: '14px',
  },
  '& .fi-icon': {
    marginRight: `${theme.suomifi.spacing.xs}`,
  },
}));

type PublishTableProps = {
  validationResult: ValidationModel<ServiceApiModel> | undefined;
  isLoading: boolean;
  publishState: RequiredLanguageVersionType<PublishDialogItem>;
  setPublishState: (state: RequiredLanguageVersionType<PublishDialogItem>) => void;
  minScheduleDate: DateTime;
  maxScheduleDate: DateTime;
  languageVersions: LanguageVersionType<ServiceApiLanguageModel>;
  lastTranslations: LastTranslationType[];
};

export function PublishTable(props: PublishTableProps): React.ReactElement {
  const { t, i18n } = useTranslation();
  const entityLanguages = getOrderedLanguageVersionKeys(props.languageVersions);
  const inTranslationSourceLangs = getUniqueInTranslationSourceLangs(props.lastTranslations);

  const isSelected = (language: Language) => props.publishState[language]?.checked;

  const toggleSelectedLanguage = (language: Language) => {
    const newState = { ...props.publishState };
    newState[language].checked = !newState[language].checked;
    props.setPublishState(newState);
  };

  const toggleScheduledPublish = (language: Language, defaultDate: DateTime) => {
    const newState = { ...props.publishState };
    newState[language].scheduledPublishDate = defaultDate;
    newState[language].scheduledPublish = !newState[language].scheduledPublish;
    props.setPublishState(newState);
  };

  const setScheduledPublishDate = (language: Language, date: string) => {
    const luxonDate = DateTime.fromISO(date);
    const newState = { ...props.publishState };
    newState[language].scheduledPublishDate = luxonDate;
    props.setPublishState(newState);
  };

  const toggleScheduledArchive = (language: Language, defaultDate: DateTime) => {
    const newState = { ...props.publishState };
    newState[language].scheduledArchiveDate = defaultDate;
    newState[language].scheduledArchive = !newState[language].scheduledArchive;
    props.setPublishState(newState);
  };

  const setScheduledArchiveDate = (language: Language, date: string) => {
    const luxonDate = DateTime.fromISO(date);
    const newState = { ...props.publishState };
    newState[language].scheduledArchiveDate = luxonDate;
    props.setPublishState(newState);
  };

  const isTranslationInProgress = (languageVersion: LanguageVersionBaseModel) => {
    return languageVersion.translationAvailability?.isInTranslation;
  };

  const getHintText = (languageVersion: LanguageVersionBaseModel) => {
    const statusKey = languageVersion.status;
    const languageKey = getKeyForLanguage(languageVersion.language);
    const statusText = t(getKeysForStatusType(statusKey));
    const translationStatusText = isTranslationInProgress(languageVersion) ? ` - ${t('Ptv.PublishingDialog.TranslationInProgress')}` : '';

    return `${t(languageKey)} - ${statusText}${translationStatusText}`;
  };

  const getHasValidationError = (language: Language) => {
    const fields = props.validationResult?.validatedFields;
    return !!props.publishState[language].checked && !!fields && !!fields[language];
  };

  const renderValidationMessage = () => {
    return (
      <StyledDiv>
        <Message type='error'>{t('Ptv.PublishingDialog.ValidationMessage')}</Message>
      </StyledDiv>
    );
  };

  const renderLanguageVersions = () => {
    return entityLanguages.map((lang) => {
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      const languageVersion = props.languageVersions[lang]!;
      const languageState = props.publishState[lang];
      const hasValidationError = getHasValidationError(lang);
      const modified = DateTime.fromISO(languageVersion.modified);
      const localTimeString = modified.toLocaleString({}, { locale: i18n.language });

      const disabled = !!(
        !allowedLanguageVersionStatusesToPublish.includes(languageVersion.status) || isTranslationInProgress(languageVersion)
      );
      const isTranslationSource = inTranslationSourceLangs.some((source) => source === lang);

      return (
        <div key={lang}>
          <Block>
            <Checkbox
              id={`publishingDialog.${lang}.publish`}
              disabled={disabled}
              checked={disabled ? false : isSelected(lang)}
              onClick={() => toggleSelectedLanguage(lang)}
              hintText={getHintText(languageVersion)}
            >
              <LangVersionTitleContent>
                <Text>{languageVersion.name}</Text>
                {hasValidationError && <ErrorInfo errorText={t('Ptv.PublishingDialog.ValidationError')} />}
              </LangVersionTitleContent>
            </Checkbox>
            {hasValidationError && renderValidationMessage()}
            {isSelected(lang) && (
              <ScheduleWrapper>
                {!hasValidationError && (
                  <>
                    <RadioButtonGroup
                      id={`publishingDialog.${lang}.scheduled-publish`}
                      name={`publish-${lang}`}
                      labelText={t('Ptv.PublishingDialog.ScheduledPublish.Title')}
                      value={!languageState.scheduledPublish ? 'publish-now' : 'publish-schedule'}
                      onChange={() =>
                        toggleScheduledPublish(
                          lang,
                          languageVersion.scheduledPublish ? DateTime.fromISO(languageVersion.scheduledPublish) : props.minScheduleDate
                        )
                      }
                    >
                      <RadioButton value='publish-now'>{t('Ptv.PublishingDialog.ScheduledPublish.Radio.Off')}</RadioButton>
                      <RadioButton value='publish-schedule'>{t('Ptv.PublishingDialog.ScheduledPublish.Radio.On')}</RadioButton>
                    </RadioButtonGroup>
                    <ScheduleLangVersion
                      id={`publishingDialog.${lang}.scheduled-publish`}
                      scheduled={languageState.scheduledPublish}
                      scheduledDate={languageState.scheduledPublishDate}
                      setDate={setScheduledPublishDate}
                      isValidDate={languageState.isValidPublishDate}
                      isValidDateCombination={languageState.isValidDateCombination}
                      minScheduleDate={props.minScheduleDate}
                      maxScheduleDate={props.maxScheduleDate}
                      disabled={disabled}
                      language={lang}
                      labelText={t('Ptv.PublishingDialog.ScheduledPublish.Calendar')}
                      hintText={t('Ptv.PublishingDialog.ScheduledPublish.CalendarDescription')}
                    />
                  </>
                )}
                <StyledDiv id={`publishingDialog.${lang}.modified`}>
                  <VisualHeading className='noTopMargin' variant='h5'>
                    {t('Ptv.PublishingDialog.Modified')}
                  </VisualHeading>
                  <Text>{localTimeString}</Text>
                </StyledDiv>
                <StyledDiv id={`publishingDialog.${lang}.modifiedBy`}>
                  <VisualHeading variant='h5'>{t('Ptv.PublishingDialog.ModifiedBy')}</VisualHeading>
                  <Text>{languageVersion.modifiedBy}</Text>
                </StyledDiv>
                {!hasValidationError && (
                  <>
                    <Label>{t('Ptv.PublishingDialog.ScheduledArchive.Title')}</Label>
                    <Checkbox
                      id={`publishingDialog.${lang}.scheduled-archive`}
                      disabled={disabled}
                      checked={languageState.scheduledArchive}
                      onClick={() =>
                        toggleScheduledArchive(
                          lang,
                          languageVersion.scheduledArchive ? DateTime.fromISO(languageVersion.scheduledArchive) : props.maxScheduleDate
                        )
                      }
                    >
                      {t('Ptv.PublishingDialog.ScheduledArchive.CheckBox')}
                    </Checkbox>
                    <ScheduleLangVersion
                      id={`publishingDialog.${lang}.scheduled-archive`}
                      scheduled={languageState.scheduledArchive}
                      scheduledDate={languageState.scheduledArchiveDate}
                      setDate={setScheduledArchiveDate}
                      isValidDate={languageState.isValidArchiveDate}
                      isValidDateCombination={languageState.isValidDateCombination}
                      minScheduleDate={props.minScheduleDate}
                      maxScheduleDate={props.maxScheduleDate}
                      disabled={disabled || isTranslationSource}
                      language={lang}
                      labelText={t('Ptv.PublishingDialog.ScheduledArchive.Calendar')}
                      hintText={t('Ptv.PublishingDialog.ScheduledArchive.CalendarDescription')}
                    />
                  </>
                )}
              </ScheduleWrapper>
            )}
          </Block>
          <FormDivider className='divider' />
        </div>
      );
    });
  };

  if (props.isLoading) {
    return (
      <div>
        <LoadingIndicator />
      </div>
    );
  } else {
    return (
      <LanguageVersionWrapper>
        <FormDivider className='divider' />
        {renderLanguageVersions()}
      </LanguageVersionWrapper>
    );
  }
}
