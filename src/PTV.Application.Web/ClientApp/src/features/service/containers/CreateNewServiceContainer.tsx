import React, { useContext, useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router';
import { makeStyles } from '@mui/styles';
import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import { LanguageVersionWithName } from 'types/languageVersionTypes';
import { LocationState } from 'types/locationState';
import { DispatchContext as FormMetaDispatchContext, switchFormModeAndSelectLanguage } from 'context/formMeta';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { createEmptyServiceUiModel } from 'utils/service';
import { getUserOrganization } from 'utils/userInfo';
import { EmptyServiceForm } from 'features/service/components/EmptyServiceForm';
import { ServiceForm } from 'features/service/components/ServiceForm';

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    flex: 1,
    marginTop: '20px',
    marginBottom: '20px',
  },
}));

export default function CreateNewServiceContainer(): React.ReactElement {
  const location = useLocation();
  const locationState = location.state as LocationState | undefined;
  const navigate = useNavigate();

  // User might come here because she wanted to create a copy of an existing service
  const [service, setService] = useState<ServiceFormValues | null | undefined>(locationState?.serviceCopy);

  useEffect(() => {
    // When copying service we receive the copy via state and it needs to be cleared once it has been used.
    if (locationState?.serviceCopy) {
      const newState: LocationState = {
        ...locationState,
        serviceCopy: undefined,
      };
      navigate(location.pathname, { state: newState });
    }
  });

  const classes = useStyles();
  const dispatch = useContext(FormMetaDispatchContext);
  const appContext = useAppContextOrThrow();

  function initializeNewService(languageVersions: LanguageVersionWithName[]) {
    const responsibleOrganization = getUserOrganization(appContext);
    const service = createEmptyServiceUiModel(responsibleOrganization);
    for (const languageVersion of languageVersions) {
      service.languageVersions[languageVersion.language].isEnabled = true;
      service.languageVersions[languageVersion.language].name = languageVersion.name;
    }
    switchFormModeAndSelectLanguage(dispatch, { mode: 'edit', language: languageVersions[0].language });
    setService(service);
  }

  if (!service) {
    return <EmptyServiceForm addLanguages={initializeNewService} />;
  }

  return (
    <div className={classes.root}>
      <ServiceForm service={service} />
    </div>
  );
}
