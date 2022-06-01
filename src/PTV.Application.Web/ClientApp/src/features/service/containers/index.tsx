import React from 'react';
import { useMatch } from 'react-router';
import CreateNewServiceContainer from './CreateNewServiceContainer';
import EditExistingServiceContainer from './EditExistingServiceContainer';

export default function ServiceContainer(): React.ReactElement {
  const match = useMatch('/service/:id');

  if (match && match.params['id']) {
    return <EditExistingServiceContainer serviceId={match.params['id']} />;
  }

  return <CreateNewServiceContainer />;
}
