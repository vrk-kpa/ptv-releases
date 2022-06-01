import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { ApiResponseWrapper, HttpError } from 'types/miscellaneousTypes';
import { QualityRequest } from 'types/qualityAgentRequests';
import { QualityResponse } from 'types/qualityAgentResponses';
import { post } from 'utils/request';

type Options = UseQueryOptions<Response, HttpError> | undefined;

type Response = ApiResponseWrapper<QualityResponse>;

export const useCheckQuality = (qualityRequest: QualityRequest, options?: Options): UseQueryResult<Response, HttpError> => {
  const query = useQuery<Response, HttpError>(
    ['qualityAgent', qualityRequest],
    () => post<Response>(`qualityAgent/check`, { ...qualityRequest }),
    options
  );
  return query;
};
