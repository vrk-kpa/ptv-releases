import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { stringify } from 'query-string';
import { HttpError } from 'types/miscellaneousTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { get } from 'utils/request';

const apiPath = 'next/public/organizations/search';
const makeKey = (params: GetOrganizationsQuery): unknown[] => [apiPath, params];

export type GetOrganizationsQuery = {
  searchValue?: string | null;
  searchAll?: boolean;
  searchOnlyDraftAndPublished?: boolean;
};

type PublicGetOrganizationsQueryResult = UseQueryResult<OrganizationModel[], HttpError>;
type Options = UseQueryOptions<OrganizationModel[], HttpError, OrganizationModel[]> | undefined;

export function getQuery(params: GetOrganizationsQuery): Promise<OrganizationModel[]> {
  const query = stringify(params);
  return get<OrganizationModel[]>(`${apiPath}?${query}`);
}

export const usePublicGetOrganizations = (params: GetOrganizationsQuery, options?: Options): PublicGetOrganizationsQueryResult => {
  return useQuery<OrganizationModel[], HttpError>(makeKey(params), () => getQuery(params), options);
};
