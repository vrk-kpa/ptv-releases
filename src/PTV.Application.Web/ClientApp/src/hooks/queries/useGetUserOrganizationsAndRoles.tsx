import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { HttpError } from 'types/miscellaneousTypes';
import { UserOrganizationsAndRolesResponse } from 'types/organizationTypes';
import { get } from 'utils/request';

const makeKey = () => 'getUserOrganizationsAndRoles';

type Result = UseQueryResult<UserOrganizationsAndRolesResponse, HttpError>;
type Options = UseQueryOptions<UserOrganizationsAndRolesResponse, HttpError, UserOrganizationsAndRolesResponse>;

export const useGetUserOrganizationsAndRoles = (options?: Options | undefined): Result => {
  return useQuery<UserOrganizationsAndRolesResponse, HttpError>(
    makeKey(),
    () => {
      return get<UserOrganizationsAndRolesResponse>('next/organizations/user-organizations-and-roles');
    },
    options
  );
};
