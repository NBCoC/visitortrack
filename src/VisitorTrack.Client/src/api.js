import axios from 'axios';

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

export const authenticate = model =>
  getInstance()
    .post('AuthenticateUserHttpTrigger', model)
    .then(result => result.data);
