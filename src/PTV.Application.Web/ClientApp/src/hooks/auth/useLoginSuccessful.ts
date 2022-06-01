import { useCookies } from 'react-cookie';
import { PtvCookieName, getCookieOptions } from 'utils/auth';

export function useLoginSuccessful(): (token: string) => void {
  const [, setCookie] = useCookies();

  return (token) => {
    setCookie(PtvCookieName, token, getCookieOptions());
  };
}
