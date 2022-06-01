import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { stringify } from 'query-string';
import { HttpError } from 'types/miscellaneousTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { get } from 'utils/request';

const apiPath = 'next/organizations/byids';
const makeKey = (ids: string[]): string[] => [apiPath, ...ids];

type Result = UseQueryResult<OrganizationModel[], HttpError>;
type Options = UseQueryOptions<OrganizationModel[], HttpError, OrganizationModel[]> | undefined;

export const useGetOrganizationsByIds = (ids: string[], options?: Options | undefined): Result => {
  return useQuery<OrganizationModel[], HttpError>(
    makeKey(ids),
    () => {
      const query = stringify({ ids: ids });
      return get<OrganizationModel[]>(`${apiPath}?${query}`);
    },
    options
  );
};
