import React from 'react';
import { Control, useFormState } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { Button } from 'suomifi-ui-components';
import { PublishingStatus } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { useCanPublishService } from 'hooks/security/useCanPublishService';

const useStyles = makeStyles(() => ({
  buttonRowRight: {
    display: 'flex',
    justifyContent: 'flex-end',
    gap: '15px',
    '& > button': {
      marginTop: '15px',
    },
  },
  buttonRowLeft: {
    display: 'flex',
    justifyContent: 'flex-start',
    gap: '15px',
    '& > button': {
      marginTop: '15px',
    },
  },
}));

type PublishButtonProps = {
  control: Control<ServiceModel>;
  hasOtherModifiedVersion: boolean;
  responsibleOrgId: string | null | undefined;
  status: PublishingStatus;
  onOpenPublishDialog: () => void;
  alignItemsLeft: boolean;
};

export function PublishButton(props: PublishButtonProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  const canPublishService = useCanPublishService({
    hasOtherModifiedVersion: props.hasOtherModifiedVersion,
    responsibleOrgId: props.responsibleOrgId,
    status: props.status,
  });

  // Note: you need to deconstruct invidivual things,
  // see https://react-hook-form.com/api/useformstate/
  const { isValid, isValidating } = useFormState<ServiceModel>({
    control: props.control,
  });

  const canPublish = isValid && !isValidating && canPublishService;

  return (
    <div className={clsx(props.alignItemsLeft ? classes.buttonRowLeft : classes.buttonRowRight)}>
      <Button onClick={props.onOpenPublishDialog} disabled={!canPublish} id='service-form-button-publish'>
        {t('Ptv.Form.Publish.Text')}
      </Button>
    </div>
  );
}
