import React from 'react';

type RedirectToPathProps = {
  path: string;
};

export default function RedirectToPath(props: RedirectToPathProps): React.ReactElement | null {
  const url = window.location.origin + props.path;
  window.location.replace(url);
  return null;
}
