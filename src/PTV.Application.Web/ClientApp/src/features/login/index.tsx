import React from 'react';
import { getSettings } from 'utils/settings';
import CloudLogin from 'features/login/components/CloudLogin';
import FakeLogin from 'features/login/components/FakeLogin';
import PahaLogin from 'features/login/components/PahaLogin';

export default function Login(): React.ReactElement | null {
  const settings = getSettings();

  if (settings.isPAHALoginEnabled) {
    return <PahaLogin />;
  }

  // Fake authentication used in local development
  if (settings.isFakeAuthenticationEnabled) {
    return <FakeLogin />;
  }

  // Cloud with non paha login
  return <CloudLogin />;
}
