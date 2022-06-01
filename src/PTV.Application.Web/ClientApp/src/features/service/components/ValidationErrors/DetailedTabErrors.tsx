import React, { useContext, useLayoutEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { capitalize } from '@mui/material';
import { Notification, Paragraph } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { LinkButton } from 'components/Buttons';
import { DispatchContext as DispatchFormMetaContext, switchFormMode, useFormMetaContext } from 'context/formMeta';
import { setDetailedTabErrorsVisibility } from 'context/formMeta/actions';
import { useGetNotificationStatuses } from 'context/formMeta/useGetNotificationStatuses';
import { useServiceContext } from 'features/service/ServiceContextProvider';
import { getMissingRequiredInfo } from './selectors';
import { useStyles } from './styles';

export type DetailedTabErrorsProps = {
  formName: string;
  language: Language;
};

export function DetailedTabErrors(props: DetailedTabErrorsProps): React.ReactElement | null {
  const classes = useStyles();
  const ctx = useServiceContext();
  const { mode } = useFormMetaContext();
  const dispatchFormMeta = useContext(DispatchFormMetaContext);
  const [focusId, setFocusId] = useState<string | null>(null);
  const { t } = useTranslation();
  const notificationStatuses = useGetNotificationStatuses();

  useLayoutEffect(() => {
    if (!focusId) {
      return;
    }
    const element = document.getElementById(focusId);
    if (!element) {
      return;
    }
    element.focus();
    setFocusId(null);
  }, [mode, focusId]);

  const errors = getMissingRequiredInfo(ctx, props.language);

  const getTranslationKey = (errorKey: string) => {
    return `Ptv.${props.formName}.Form.ValidationError.${capitalize(errorKey)}`;
  };

  const handleErrorClick = (errorKey: string) => {
    switchFormMode(dispatchFormMeta, 'edit');
    const id = `languageVersions.${props.language}.${errorKey}`;
    setFocusId(id);
  };

  if (!!errors && errors.length > 0 && notificationStatuses.detailedTabErrorsVisible) {
    return (
      <div className={classes.boxSpace}>
        <Notification
          status='error'
          closeText={t('Ptv.Service.Form.Header.NewVersionNotification.Close')}
          headingVariant='h2'
          headingText={t('Ptv.Form.Validation.RequiredInformation.Title')}
          onCloseButtonClick={() => {
            setDetailedTabErrorsVisibility(dispatchFormMeta, false);
          }}
        >
          <Paragraph>{t('Ptv.Form.Validation.RequiredInformation.Description')}</Paragraph>
          <ul className={classes.errorList}>
            {errors.map((fieldName) => {
              return (
                <li key={fieldName}>
                  <LinkButton label={t(getTranslationKey(fieldName))} onClick={() => handleErrorClick(fieldName)} />
                </li>
              );
            })}
          </ul>
        </Notification>
      </div>
    );
  }

  return null;
}
