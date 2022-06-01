import React, { KeyboardEvent, MouseEvent, useState } from 'react';
import { Control, useFormState } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { makeStyles } from '@mui/styles';
import { Icon, Link } from 'suomifi-ui-components';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { useFormMetaContext } from 'context/formMeta';
import { ConfirmationModal } from 'features/service/components/ConfirmationModal';

type BackLinkProps = {
  control: Control<ServiceModel>;
  resetForm: () => void;
};

const useStyles = makeStyles((theme) => ({
  backLink: {
    alignItems: 'center',
    fontSize: '14px !important',
    color: 'rgb(99, 103, 105) !important',
  },
  icon: {
    verticalAlign: 'middle',
    marginRight: '5px',
  },
}));

export function BackLink(props: BackLinkProps): React.ReactElement {
  const { t } = useTranslation();

  const [isConfirmNavModalOpen, setIsConfirmNavModalOpen] = useState<boolean>(false);

  const navigate = useNavigate();

  const classes = useStyles();

  const { isDirty } = useFormState<ServiceModel>({
    control: props.control,
  });

  const meta = useFormMetaContext();

  function onBackClick(event: MouseEvent | KeyboardEvent, isDirty: boolean) {
    if (isDirty || meta.mode === 'edit') {
      event.preventDefault();
      setIsConfirmNavModalOpen(true);
    }
  }

  function discardChangesAndGoToFrontPage() {
    setIsConfirmNavModalOpen(false);
    props.resetForm();
    navigate('/frontpage/search');
  }

  return (
    <>
      <Link href='/frontpage/search' className={classes.backLink} onClick={(event) => onBackClick(event, isDirty)}>
        <Icon icon='arrowLeft' className={classes.icon} />
        {t('Ptv.Common.Back')}
      </Link>
      <ConfirmationModal
        title={t(`Ptv.Form.Cancel.ConfirmCancelDialog.Title`)}
        content={t(`Ptv.Form.Cancel.ConfirmCancelDialog.Description`)}
        confirm={discardChangesAndGoToFrontPage}
        cancel={() => setIsConfirmNavModalOpen(false)}
        isOpen={isConfirmNavModalOpen}
      />
    </>
  );
}
