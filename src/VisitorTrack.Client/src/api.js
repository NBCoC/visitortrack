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

export const getUser = (token, entityId) =>
  getInstance(token)
    .get(`GetUserHttpTrigger?entityId=${entityId}`)
    .then(result => result.data);

export const createUser = (token, contextUserId, model) =>
  getInstance(token)
    .post(`CreateUserHttpTrigger?contextUserId=${contextUserId}`, model)
    .then(result => result.data);

export const updateUser = (token, contextUserId, entityId, model) =>
  getInstance(token)
    .put(
      `UpdateUserHttpTrigger?contextUserId=${contextUserId}&entityId=${entityId}`,
      model
    )
    .then(result => result.data);

export const deleteUser = (token, contextUserId, entityId) =>
  getInstance(token)
    .delete(
      `DeleteUserHttpTrigger?contextUserId=${contextUserId}&entityId=${entityId}`
    )
    .then(result => result.data);

export const authenticate = model =>
  getInstance()
    .post('AuthenticateUserHttpTrigger', model)
    .then(result => result.data);

export const updatePassword = (token, contextUserId, model) =>
  getInstance(token)
    .post(`UpdateUserPasswordHttpTrigger?contextUserId=${contextUserId}`, model)
    .then(result => result.data);

export const resetPassword = (token, contextUserId, entityId) =>
  getInstance(token)
    .post(
      `ResetUserPasswordHttpTrigger?contextUserId=${contextUserId}&entityId=${entityId}`
    )
    .then(result => result.data);
