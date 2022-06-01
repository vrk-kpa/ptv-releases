import React from 'react';
import { Message } from 'components/Message';

type DuplicateOrganizationsProps = {
  status: 'default' | 'error';
  statusText: string | undefined;
};

export function DuplicateOrganizations(props: DuplicateOrganizationsProps): React.ReactElement | null {
  if (props.status === 'default') return null;

  return <Message type='error'>{props.statusText}</Message>;
}
