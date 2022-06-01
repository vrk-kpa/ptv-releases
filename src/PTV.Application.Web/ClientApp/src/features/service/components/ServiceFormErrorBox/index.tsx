import React from 'react';
import { useTranslation } from 'react-i18next';
import _ from 'lodash';
import { ErrorBox } from 'components/ErrorBox/ErrorBox';
import { useGetServerError } from 'context/formMeta';

export function ServiceFormErrorBox(): React.ReactElement {
  const serverError = useGetServerError();
  const { t } = useTranslation();

  return (
    <>
      {!_.isEmpty(serverError) && (
        <ErrorBox title={t('Ptv.Error.SavingTitle')}>
          {serverError?.detail && serverError.instance
            ? t(serverError.detail, { duplicateInstance: serverError.instance })
            : t('Ptv.Error.SavingFailed')}
        </ErrorBox>
      )}
    </>
  );
}
