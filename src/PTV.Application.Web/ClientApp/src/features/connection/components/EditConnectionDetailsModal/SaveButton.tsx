import React from 'react';
import { Control, useFormState } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Button } from 'suomifi-ui-components';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';

type SaveButtonProps = {
  control: Control<ConnectionFormModel>;
  isQueryRunning: boolean;
  onSave: () => void;
};

export function SaveButton(props: SaveButtonProps): React.ReactElement {
  const { t } = useTranslation();

  // Note: you need to deconstruct invidivual things,
  // see https://react-hook-form.com/api/useformstate/
  const { isValidating, isValid } = useFormState<ConnectionFormModel>({
    control: props.control,
  });

  const disabled = props.isQueryRunning || isValidating || !isValid;

  return (
    <Button disabled={disabled} onClick={props.onSave}>
      {t('Ptv.Common.Save')}
    </Button>
  );
}
