import { HttpError, ProblemDetails } from 'types/miscellaneousTypes';
import { getJwt } from './auth';
import { getApiUrl } from './settings';

export const get = async <T>(path: string, args: RequestInit = {}): Promise<T> => {
  return getFrom(getApiUrl(path), {
    method: 'GET',
    headers: getDefaultHeaders(),
    ...args,
  });
};

export const getFrom = async <T>(url: string, args: RequestInit = {}): Promise<T> => {
  let response: Response;
  let text: string;
  try {
    response = await fetch(url, args);
    text = await response.text();
  } catch (e) {
    throw new HttpError();
  }

  if (hasInvalidStatus(response)) {
    throw new HttpError(response);
  }

  const json = text.length ? JSON.parse(text) : {};
  return json;
};

export const post = async <T>(path: string, body?: unknown, args: RequestInit = {}): Promise<T> => {
  return postTo(getApiUrl(path), body, {
    method: 'POST',
    headers: getDefaultHeaders(),
    ...args,
  });
};

export const postTo = async <T>(url: string, body?: unknown, args: RequestInit = {}): Promise<T> => {
  let response: Response;
  let text: string;
  try {
    response = await fetch(url, {
      body: !body ? null : JSON.stringify(body),
      ...args,
    });
    text = await response.text();
  } catch (e) {
    throw new HttpError();
  }

  if (hasInvalidStatus(response)) {
    const details = text.length ? (JSON.parse(text) as ProblemDetails) : null;
    if (details && details.title && details.detail) {
      throw new HttpError(response, details.title, details);
    } else {
      throw new HttpError(response);
    }
  }

  const json = text.length ? JSON.parse(text) : {};

  return json;
};

export const httpDelete = async <T>(path: string, body?: unknown, args: RequestInit = {}): Promise<T> => {
  return httpDeleteTo(getApiUrl(path), body, {
    method: 'DELETE',
    headers: getDefaultHeaders(),
    ...args,
  });
};

export const httpDeleteTo = async <T>(url: string, body?: unknown, args: RequestInit = {}): Promise<T> => {
  let response: Response;
  let text: string;
  try {
    response = await fetch(url, {
      body: !body ? null : JSON.stringify(body),
      ...args,
    });
    text = await response.text();
  } catch (e) {
    throw new HttpError();
  }

  if (hasInvalidStatus(response)) {
    throw new HttpError(response);
  }

  const json = text.length ? JSON.parse(text) : {};

  return json;
};

const hasInvalidStatus = (response: Response) => {
  return response.status >= 300 && response.status < 600;
};

const getDefaultHeaders = (): HeadersInit => {
  const headers: HeadersInit = new Headers();
  headers.set('Accept', 'application/json');
  headers.set('Content-Type', 'application/json');

  const jwt = getJwt();
  if (jwt) {
    headers.set('Authorization', `Bearer ${jwt}`);
  }

  return headers;
};
