import { UseQueryOptions, UseQueryResult, useQuery } from 'react-query';
import { HttpError } from 'types/miscellaneousTypes';
import { AppSettings } from 'types/settingTypes';
import { getFrom } from 'utils/request';

type Result = UseQueryResult<AppSettings, HttpError>;
type Options = UseQueryOptions<AppSettings, HttpError, AppSettings> | undefined;

export const useGetEnvironmentSettings = (options?: Options): Result => {
  return useQuery<AppSettings, HttpError>(
    'getEnvironmentSettings',
    () => getFrom<AppSettings>(`${window.location.protocol}//${window.location.host}/api/GetEnvironmentSettings`),
    options
  );
};
