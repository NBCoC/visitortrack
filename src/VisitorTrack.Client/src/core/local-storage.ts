import { AuthenticationResult } from './models';
const CACHE_KEY = 'visitortrack.client.credential.cache';

const isLocalStorageSupported = (): boolean => typeof Storage !== 'undefined';

export const setCredentials = (credentials: AuthenticationResult): void => {
  if (!isLocalStorageSupported()) return;
  localStorage.setItem(CACHE_KEY, JSON.stringify(credentials || {}));
};

export const clearCredentials = (): void => setCredentials({} as AuthenticationResult);

export const getCredentials = (): AuthenticationResult => {
  if (isLocalStorageSupported()) {
    const credentials = JSON.parse(localStorage.getItem(CACHE_KEY));
    const data = credentials as AuthenticationResult;
    return data || ({} as AuthenticationResult);
  }
  return {} as AuthenticationResult;
};
