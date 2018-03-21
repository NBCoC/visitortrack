import axios from 'axios';

const CACHE_KEY = 'visitortrack.client.credential.cache';
const USER_ID = 'contextUserId';
const ID = 'entityId';

const getInstance = token => {
  return axios.create({
    baseURL: 'https://visitor-track-func.azurewebsites.net/api/',
    headers: { 'X-Visitor-Track-Token': token || '' }
  });
};

export const getUserRoles = token =>
  getInstance(token)
    .get('GetUserRolesHttpTrigger')
    .then(result => result.data);

export const authenticate = (email, psw) =>
  getInstance()
    .post('AuthenticateUserHttpTrigger', {
      emailAddress: email,
      password: psw
    })
    .then(result => result.data);

export const loadCredentialCache = () =>
  typeof Storage === 'undefined'
    ? {}
    : JSON.parse(localStorage.getItem(CACHE_KEY));

export const saveCredentialCache = (credentials) => {
  if (typeof Storage === 'undefined') return;

  localStorage.setItem(CACHE_KEY, JSON.stringify(credentials));
};

export const clearCredentialCache = () => {
  if (typeof Storage === 'undefined') return;

  localStorage.setItem(CACHE_KEY, {});
};
