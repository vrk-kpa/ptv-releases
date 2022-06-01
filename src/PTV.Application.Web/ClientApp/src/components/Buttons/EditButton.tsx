import React, { useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { Button } from 'suomifi-ui-components';
import { DispatchContext, switchFormMode } from 'context/formMeta';

type EditButtonProps = {
  disabled: boolean;
};

export default function EditButton(props: EditButtonProps): React.ReactElement {
  const dispatch = useContext(DispatchContext);
  const { t } = useTranslation();

  const handleClick = () => {
    switchFormMode(dispatch, 'edit');
  };

  return (
    <Button aria-disabled={props.disabled} variant='secondary' onClick={handleClick} id='service-form-button-edit'>
      {t('Ptv.Form.Edit.Text')}
    </Button>
  );
}
