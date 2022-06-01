import React, { useContext, useState } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { Language } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { DispatchContext, cancelModification } from 'context/formMeta';
import { ConfirmationModal } from 'features/service/components/ConfirmationModal';
import { ButtonRow } from './ButtonRow';

type ServiceFormEditFunctionsProps = {
  control: Control<ServiceModel>;
  enabledLanguages: Language[];
  selectedLanguage: Language;
  hasOtherModifiedVersion: boolean;
  serviceId: string | null | undefined;
  responsibleOrgId: string | null | undefined;
  saveInProgress: boolean;
  resetForm: () => void;
  saveDraft: () => void;
  alignItemsLeft: boolean;
};

export function ServiceFormEditFunctions(props: ServiceFormEditFunctionsProps): React.ReactElement {
  const { t } = useTranslation();
  const dispatch = useContext(DispatchContext);
  const navigate = useNavigate();
  const [isCancelModalOpen, setIsCancelModalOpen] = useState<boolean>(false);

  function discardChangesAndGoToViewMode() {
    setIsCancelModalOpen(false);
    props.resetForm();
    cancelModification(dispatch, { enabledLanguages: props.enabledLanguages, selectedLanguage: props.selectedLanguage });
  }

  function discardChangesAndGoToFrontPage() {
    setIsCancelModalOpen(false);
    props.resetForm();
    navigate('/frontpage/search');
  }

  function discardChanges() {
    if (props.serviceId) {
      discardChangesAndGoToViewMode();
    } else {
      discardChangesAndGoToFrontPage();
    }
  }

  return (
    <>
      <ButtonRow
        control={props.control}
        hasOtherModifiedVersion={props.hasOtherModifiedVersion}
        responsibleOrgId={props.responsibleOrgId}
        saveInProgress={props.saveInProgress}
        onCancelClick={() => setIsCancelModalOpen(true)}
        saveDraft={props.saveDraft}
        alignItemsLeft={props.alignItemsLeft}
      />
      <ConfirmationModal
        title={t(`Ptv.Form.Cancel.ConfirmCancelDialog.Title`)}
        content={t(`Ptv.Form.Cancel.ConfirmCancelDialog.Description`)}
        confirm={discardChanges}
        cancel={() => setIsCancelModalOpen(false)}
        isOpen={isCancelModalOpen}
      />
    </>
  );
}
