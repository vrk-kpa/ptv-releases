import React from 'react';
import { Control, useFormState } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Button } from 'suomifi-ui-components';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { useCanCreateService } from 'hooks/security/useCanCreateService';
import { SaveDraftButton } from './SaveDraftButton';

const useStyles = makeStyles(() => ({
  buttonRowRight: {
    display: 'flex',
    justifyContent: 'flex-end',
    marginTop: '15px',
    gap: '15px',
    '& > button': {
      marginBottom: '15px',
    },
  },
  buttonRowLeft: {
    display: 'flex',
    justifyContent: 'flex-start',
    marginTop: '15px',
    gap: '15px',
    '& > button': {
      marginBottom: '15px',
    },
  },
}));

type ButtonRowProps = {
  control: Control<ServiceModel>;
  hasOtherModifiedVersion: boolean;
  responsibleOrgId: string | null | undefined;
  saveInProgress: boolean;
  saveDraft: () => void;
  onCancelClick: () => void;
  alignItemsLeft: boolean;
};

export function ButtonRow(props: ButtonRowProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  const canCreateService = useCanCreateService({
    hasOtherModifiedVersion: props.hasOtherModifiedVersion,
    responsibleOrgId: props.responsibleOrgId,
  });

  // Note: you need to deconstruct invidivual things,
  // see https://react-hook-form.com/api/useformstate/
  const { isValid, isValidating } = useFormState<ServiceModel>({
    control: props.control,
  });

  const canSaveAsDraft = isValid && !isValidating && canCreateService;

  return (
    <div className={props.alignItemsLeft ? classes.buttonRowLeft : classes.buttonRowRight}>
      {!!props.alignItemsLeft && (
        <SaveDraftButton saveInProgress={props.saveInProgress} canSave={canSaveAsDraft} saveDraft={props.saveDraft} />
      )}
      <Button variant='secondaryNoBorder' id='service-form-button-cancel' disabled={props.saveInProgress} onClick={props.onCancelClick}>
        {t('Ptv.Form.Cancel.Text')}
      </Button>
      {!props.alignItemsLeft && (
        <SaveDraftButton saveInProgress={props.saveInProgress} canSave={canSaveAsDraft} saveDraft={props.saveDraft} />
      )}
    </div>
  );
}
