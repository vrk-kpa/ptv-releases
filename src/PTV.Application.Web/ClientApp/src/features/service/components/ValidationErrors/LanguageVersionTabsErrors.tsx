import React, { useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { Notification, Paragraph } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { LinkButton } from 'components/Buttons';
import { DispatchContext as DispatchFormMetaContext, switchFormModeAndSelectLanguage } from 'context/formMeta';
import { setOtherTabsErrorsVisibility as setLanguageVersionTabsErrorsVisibility } from 'context/formMeta/actions';
import { useGetNotificationStatuses } from 'context/formMeta/useGetNotificationStatuses';
import { getKeyForLanguage } from 'utils/translations';
import { useServiceContext } from 'features/service/ServiceContextProvider';
import { getLanguagesWithErrors } from './selectors';
import { useStyles } from './styles';

export type LanguageVersionTabsErrorsProps = {
  language: Language;
};

export function LanguageVersionTabsErrors(props: LanguageVersionTabsErrorsProps): React.ReactElement | null {
  const ctx = useServiceContext();
  const dispatch = useContext(DispatchFormMetaContext);
  const classes = useStyles();
  const { t } = useTranslation();

  const languageVersionErrors = getLanguagesWithErrors(ctx, props.language);
  const notificationStatuses = useGetNotificationStatuses();

  const handleLanguageSwitch = (language: Language) => {
    switchFormModeAndSelectLanguage(dispatch, { mode: 'edit', language: language });
  };

  return (
    <div className={classes.boxSpace}>
      {notificationStatuses.languageVersionTabsErrorsVisible && languageVersionErrors.length > 0 && (
        <Notification
          status='error'
          closeText={t('Ptv.Service.Form.Header.NewVersionNotification.Close')}
          headingVariant='h2'
          headingText={t('Ptv.Form.Validation.OtherLanguages.Title')}
          onCloseButtonClick={() => {
            setLanguageVersionTabsErrorsVisibility(dispatch, false);
          }}
        >
          <Paragraph>{t('Ptv.Form.Validation.RequiredInformation.Description')}</Paragraph>
          <ul className={classes.errorList}>
            {languageVersionErrors.map((language) => {
              return (
                <li key={language}>
                  <LinkButton label={t(getKeyForLanguage(language))} onClick={() => handleLanguageSwitch(language)} />
                </li>
              );
            })}
          </ul>
        </Notification>
      )}
    </div>
  );
}
