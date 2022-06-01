import React from 'react';
import { useTranslation } from 'react-i18next';
import { Paragraph } from 'suomifi-ui-components';
import { HttpError } from 'types/miscellaneousTypes';
import { ErrorBox } from './ErrorBox';
import { UnauthorizedErrorBox } from './UnauthorizedErrorBox';

type ServerErrorBoxProps = {
  httpError: HttpError;
};

export function ServerErrorBox(props: ServerErrorBoxProps): React.ReactElement {
  const { t } = useTranslation();

  if (props.httpError.isUnathorizedError()) {
    return <UnauthorizedErrorBox />;
  }

  const title = props.httpError.isNotFoundError() ? 'Ptv.Error.NotFound.Title' : 'Ptv.Error.ServerError.Title';
  const description = props.httpError.isNotFoundError() ? 'Ptv.Error.NotFound.Description' : 'Ptv.Error.ServerError.Description';

  return (
    <ErrorBox title={t(title)}>
      <Paragraph>{t(description)}</Paragraph>
    </ErrorBox>
  );
}
