import React from 'react';
import { useCookies } from 'react-cookie';
import { PtvCookieName, hasTokenExpired } from 'utils/auth';
import { getLocalStorageItem } from 'utils/localStorage';
import { getSettings } from 'utils/settings';

export default function PahaLogin(): React.ReactElement {
  const [cookies] = useCookies([PtvCookieName]);

  if (hasTokenExpired(cookies[PtvCookieName])) {
    const settings = getSettings();
    const language = getLocalStorageItem('i18nextLng') || 'fi';
    const pahaUrl = settings.pahaRedirectUrl + '/' + language + '/kirjautuminen/kirjaudu?redirectUrl=' + settings.pahaReturnUrl;
    window.location.href = pahaUrl;
  }

  return <></>;
}
