import React, { useContext, useEffect } from 'react';
import { makeStyles } from '@mui/styles';
import { ServerErrorBox } from 'components/ErrorBox/ServerErrorBox';
import LoadingIndicator from 'components/LoadingIndicator';
import { DispatchContext } from 'context/DispatchContextProvider';
import useGetStaticData from 'hooks/queries/useGetStaticData';
import { useGetUserInfo } from 'hooks/queries/useGetUserInfo';
import { useGetUserOrganizationsAndRoles } from 'hooks/queries/useGetUserOrganizationsAndRoles';
import Routes from './Routes';

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    flex: 1,
  },
  loadingIndicator: {
    display: 'flex',
    flex: 1,
    justifyContent: 'center',
    alignItems: 'flex-start',
  },
}));

export default function AuthenticatedApp(): React.ReactElement {
  const classes = useStyles();
  const dispatch = useContext(DispatchContext);

  const orgQuery = useGetUserOrganizationsAndRoles();
  const userQuery = useGetUserInfo();
  const staticDataQuery = useGetStaticData();

  const isLoading = orgQuery.isLoading || userQuery.isLoading || staticDataQuery.isLoading;
  const error = orgQuery.error || userQuery.error || staticDataQuery.error;

  useEffect(() => {
    if (!staticDataQuery.isLoading && !staticDataQuery.error) {
      dispatch({ type: 'StaticDataLoaded', payload: staticDataQuery.data });
    }
  }, [staticDataQuery.isLoading, staticDataQuery.error, staticDataQuery.data, dispatch]);

  useEffect(() => {
    if (orgQuery.data) {
      dispatch({ type: 'UserOrgAndRolesLoaded', payload: orgQuery.data });
    }
  }, [orgQuery.data, dispatch]);

  useEffect(() => {
    if (userQuery.data?.data) {
      dispatch({ type: 'UserInfoLoaded', payload: userQuery.data.data });
    }
  }, [userQuery.data, dispatch]);

  if (isLoading) {
    return (
      <div className={classes.loadingIndicator}>
        <LoadingIndicator />
      </div>
    );
  }

  if (error) {
    return <ServerErrorBox httpError={error} />;
  }

  return <Routes />;
}
