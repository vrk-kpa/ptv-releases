import React, { useContext, useEffect, useState } from 'react';
import { makeStyles } from '@mui/styles';
import { getKeys } from 'utils';
import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import { ServerErrorBox } from 'components/ErrorBox/ServerErrorBox';
import LoadingIndicator from 'components/LoadingIndicator';
import { DispatchContext as DispatchFormMetaContext, setFirstTabAsSelected } from 'context/formMeta';
import useGetServiceUiModel from 'hooks/queries/useGetServiceUiModel';
import { DispatchContext } from 'features/service/DispatchContextProvider';
import { ServiceForm } from 'features/service/components/ServiceForm';

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    flex: 1,
    marginTop: '20px',
    marginBottom: '20px',
  },
  loadingIndicator: {
    display: 'flex',
    flex: 1,
    justifyContent: 'center',
    alignItems: 'flex-start',
  },
}));

type EditExistingServiceContainerProps = {
  serviceId: string;
};

export default function EditExistingServiceContainer(props: EditExistingServiceContainerProps): React.ReactElement {
  const [service, setService] = useState<ServiceFormValues | null>(null);
  const classes = useStyles();
  const dispatch = useContext(DispatchContext);
  const dispatchFormMeta = useContext(DispatchFormMetaContext);
  const query = useGetServiceUiModel(props.serviceId);

  function updateService(newService: ServiceFormValues) {
    setService(newService);
  }

  useEffect(() => {
    if (query.service && !service) {
      setService(query.service);
      setFirstTabAsSelected(
        dispatchFormMeta,
        getKeys(query.service.languageVersions).filter((langKey) => query.service?.languageVersions[langKey].isEnabled)
      );
    }
  }, [dispatch, dispatchFormMeta, query.service, service]);

  if (query.error) {
    return <ServerErrorBox httpError={query.error} />;
  }

  if (query.isLoading || !service) {
    return (
      <div className={classes.loadingIndicator}>
        <LoadingIndicator />
      </div>
    );
  }

  return (
    <div className={classes.root}>
      <ServiceForm service={service} updateService={updateService} />
    </div>
  );
}
