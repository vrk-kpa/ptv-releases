import React, { useContext, useLayoutEffect, useState } from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Notification } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { LinkButton } from 'components/Buttons';
import { DispatchContext as DispatchFormMetaContext, useFormMetaContext } from 'context/formMeta';
import { setMissingOrganizationErrorsVisibilty } from 'context/formMeta/actions';
import { useGetNotificationStatuses } from 'context/formMeta/useGetNotificationStatuses';
import { getKeyForLanguage } from 'utils/translations';
import { useServiceContext } from 'features/service/ServiceContextProvider';
import { ConfirmationModal } from 'features/service/components/ConfirmationModal';
import { getMissingOrganizationLanguages } from './selectors';
import { useStyles } from './styles';

export type MissingOrganizationErrorsProps = {
  language: Language;
  control: Control<ServiceModel>;
};

export function MissingOrganizationErrors(props: MissingOrganizationErrorsProps): React.ReactElement | null {
  const ctx = useServiceContext();
  const { mode } = useFormMetaContext();
  const dispatchFormMeta = useContext(DispatchFormMetaContext);
  const classes = useStyles();
  const { t } = useTranslation();
  const [focusId, setFocusId] = useState<string | null>(null);
  const [isConfirmationModalOpen, setIsConfirmationModalOpen] = useState<boolean>(false);

  const missingOrganizationLanguages = getMissingOrganizationLanguages(ctx);
  const notificationStatuses = useGetNotificationStatuses();
  const responsibleOrganization = useWatch({ control: props.control, name: `${cService.responsibleOrganization}` });

  const handleErrorClick = () => {
    if (mode === 'view') {
      navigateToResponsibleOrg();
    } else {
      setIsConfirmationModalOpen(true);
    }
  };

  const navigateToResponsibleOrg = () => {
    if (responsibleOrganization?.versionedId) {
      window.location.assign(`/organization/${responsibleOrganization.versionedId}`);
    }
  };

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

  if (!missingOrganizationLanguages || missingOrganizationLanguages.length < 1 || !notificationStatuses.missingOrganizationErrorsVisible) {
    return null;
  }

  const translatedLanguages = missingOrganizationLanguages.map((language) => t(getKeyForLanguage(language))).join(', ');

  return (
    <div className={classes.boxSpace}>
      <Notification
        status='error'
        closeText={t('Ptv.Common.Close')}
        headingVariant='h2'
        headingText={t('Ptv.Form.Validation.MissingOrganizationLanguages.Title')}
        onCloseButtonClick={() => {
          setMissingOrganizationErrorsVisibilty(dispatchFormMeta, false);
        }}
      >
        <ul className={classes.errorList}>
          <li>
            <LinkButton
              label={t('Ptv.Form.Validation.MissingOrganizationLanguages.Description', { languages: translatedLanguages })}
              onClick={() => handleErrorClick()}
            />
          </li>
        </ul>
      </Notification>
      <ConfirmationModal
        title={t(`Ptv.Form.ConfirmMoveToOrganizationDialog.Title`)}
        content={t(`Ptv.Form.ConfirmMoveToOrganizationDialog.Description`)}
        confirm={navigateToResponsibleOrg}
        cancel={() => setIsConfirmationModalOpen(false)}
        isOpen={isConfirmationModalOpen}
      />
    </div>
  );
}
