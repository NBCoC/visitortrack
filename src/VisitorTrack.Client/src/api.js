import axios from 'axios';

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

export const getUsers = token =>
  getInstance(token)
    .get('GetAllUsersHttpTrigger')
    .then(result => result.data);

export const getUser = (token, id) =>
  getInstance(token)
    .get(`GetUserHttpTrigger?entityId=${id}`)
    .then(result => result.data);

export const authenticate = model =>
  getInstance()
    .post('AuthenticateUserHttpTrigger', model)
    .then(result => result.data);

export const changePassword = (id, model) =>
  getInstance(token)
    .post(`UpdateUserPasswordHttpTrigger?contextUserId=${id}`, model)
    .then(result => result.data);
