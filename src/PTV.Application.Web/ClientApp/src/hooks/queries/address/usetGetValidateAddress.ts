import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { StreetModel } from 'types/api/streetModel';
import { HttpError } from 'types/miscellaneousTypes';
import { get } from 'utils/request';

export type ValidateAddressQuery = {
  streetName: string;
  postalCode: string;
  streetNumber: string;
};

const apiPath = 'next/address/validate-address';
const makeKey = (params: ValidateAddressQuery): unknown[] => [apiPath, params];

function getSearch(params: ValidateAddressQuery): Promise<StreetModel> {
  const queryString = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    queryString.append(key, value.toString());
  });
  return get<StreetModel>(`${apiPath}?${queryString.toString()}`);
}

type Result = UseQueryResult<StreetModel, HttpError>;
type Options = UseQueryOptions<StreetModel, HttpError, StreetModel>;

export const useGetValidateAddress = (parameters: ValidateAddressQuery, options?: Options): Result => {
  return useQuery<StreetModel, HttpError>(makeKey(parameters), () => getSearch(parameters), options);
};
