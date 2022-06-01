import React from 'react';

export default function RedirectRoot(): React.ReactElement | null {
  const redirectUrl = window.location.href;
  window.location.replace(redirectUrl);
  return null;
}
