import Vue from 'Vue';
import Vuex from 'Vuex';

const CACHE_KEY = 'visitortrack.client.credential.cache';

const loadCredentialCache = () =>
  typeof Storage === 'undefined'
    ? {}
    : JSON.parse(localStorage.getItem(CACHE_KEY));

const saveCredentialCache = value => {
  if (typeof Storage !== 'undefined')
    localStorage.setItem(CACHE_KEY, JSON.stringify(value));
};

const clearCredentialCache = () => {
  if (typeof Storage !== 'undefined') localStorage.setItem(CACHE_KEY, {});
};

Vue.use(Vuex);

const credentials = loadCredentialCache();

export default new Vuex.Store({
  strict: true,
  state: {
    user: credentials.user,
    token: credentials.token
  },
  mutations: {
    authenticate: (state, payload) => {
      saveCredentialCache(payload);

      state.user = payload.user;
      state.token = payload.token;
    }
  },
  actions: {
    authenticate: (context, payload) => {
      context.commit('authenticate', payload);
    }
  },
  getters: {
    token: state => state.token,
    user: state => state.user,
    isAuthenticated: state => {
      if (!state.token) {
        return false;
      }
      return true;
    }
  }
});
