import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Button } from 'suomifi-ui-components';
import LoadingIndicator from 'components/LoadingIndicator';

const useStyles = makeStyles(() => ({
  content: {
    display: 'flex',
    alignItems: 'center',
  },
  loadingIndicator: {
    display: 'flex',
    marginLeft: '10px',
  },
}));

type SaveDraftButtonProps = {
  canSave: boolean;
  saveInProgress: boolean;
  saveDraft: () => void;
};

export function SaveDraftButton(props: SaveDraftButtonProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  function saveDraft() {
    props.saveDraft();
  }

  return (
    <Button disabled={!props.canSave || props.saveInProgress} onClick={saveDraft} id='service-form-button-save'>
      {props.saveInProgress && (
        <div className={classes.content}>
          {t('Ptv.Form.SaveAdDraft.Text')}
          <div className={classes.loadingIndicator}>
            <LoadingIndicator size='24px' />
          </div>
        </div>
      )}
      {!props.saveInProgress && t('Ptv.Form.SaveAdDraft.Text')}
    </Button>
  );
}
