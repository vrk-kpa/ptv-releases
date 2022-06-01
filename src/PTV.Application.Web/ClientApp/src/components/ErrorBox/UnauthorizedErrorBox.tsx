import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, Paragraph } from 'suomifi-ui-components';
import { useLogout } from 'hooks/auth/useLogout';
import { ErrorBox } from './ErrorBox';

export function UnauthorizedErrorBox(): React.ReactElement {
  const { t } = useTranslation();
  const logout = useLogout();
  return (
    <div>
      <ErrorBox title={t('Ptv.Error.Unauthorized.Title')}>
        <Paragraph>{t('Ptv.Error.Unauthorized.Description')}</Paragraph>
        <Link onClick={() => logout()} href={window.location.href}>
          {t('Ptv.Error.Unauthorized.Link.Title')}
        </Link>
      </ErrorBox>
    </div>
  );
}
