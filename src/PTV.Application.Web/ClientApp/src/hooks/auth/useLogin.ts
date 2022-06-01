import { UseMutationResult, useMutation } from 'react-query';
import { AppEnvironment } from 'types/enumTypes';
import { LoginModel, LoginResponseModel } from 'types/loginTypes';
import { post } from 'utils/request';
import { useLoginSuccessful } from './useLoginSuccessful';

type error = (error: unknown) => void;

export function useLogin(
  fakeAuthEnabled: boolean,
  env: AppEnvironment,
  onError: error
): UseMutationResult<LoginResponseModel, unknown, LoginModel, unknown> {
  const loginSuccessful = useLoginSuccessful();

  const postLogin = (body: unknown): Promise<LoginResponseModel> => {
    return post<LoginResponseModel>('auth/CreateConfiguration', body);
  };

  const login = useMutation(
    (loginData: LoginModel) => {
      // Local dev environment
      if (fakeAuthEnabled) {
        return postLogin(loginData);
      }

      // Cloud dev environment
      if (env === 'Dev') {
        return postLogin({
          name: loginData.name,
          password: loginData.password,
          userAccessRightsGroup: loginData.userAccessRightsGroup,
        });
      }

      // All other non production cloud environments
      return postLogin({
        name: loginData.name,
        password: loginData.password,
      });
    },
    {
      onError: (error) => {
        onError(error);
      },
      onSuccess: (response) => {
        if (!response.data?.token) {
          // Response contains also userInfo but we fetch it separately
          // TODO: Not sure how to detect failure but success scenario should return token
          console.log('Login failed', response);
          return;
        }

        loginSuccessful(response.data.token);
      },
    }
  );

  return login;
}
