import { Cookies } from 'react-cookie';
import jwtDecode, { JwtPayload } from 'jwt-decode';
import { CookieSetOptions } from 'universal-cookie';

export const PtvCookieName = 'ptv_token';
const ExpirationDeltaInSeconds = 30; // avoid using token that is about to expire
const CookieExpirationInDays = 1; // How long the cookie is stored

export const getJwt = (): string | undefined => {
  const cookies = new Cookies();
  return cookies.get(PtvCookieName);
};

export const getCookieOptions = (): CookieSetOptions => {
  const expires = new Date();
  expires.setDate(expires.getDate() + CookieExpirationInDays);
  return {
    path: '/',
    sameSite: 'strict',
    expires: expires,
  };
};

export function hasTokenExpired(token: string | null): boolean {
  if (!token) {
    return true;
  }

  return hasJwtExpired(jwtDecode<JwtPayload>(token));
}

function hasJwtExpired(token: JwtPayload | null): boolean {
  if (!token?.exp) {
    return true;
  }
  const expiration = token.exp - ExpirationDeltaInSeconds;
  const now = new Date().getTime() / 1000; // jwt exp is in seconds
  return expiration < now;
}
