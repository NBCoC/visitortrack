import Vue from 'vue';
import Vuex from 'vuex';

const CACHE_KEY = 'visitortrack.client.credential.cache';

const setCredentialCache = value => {
  if (typeof Storage !== 'undefined')
    localStorage.setItem(CACHE_KEY, JSON.stringify(value || {}));
};

const credentials =
  typeof Storage === 'undefined'
    ? {}
    : JSON.parse(localStorage.getItem(CACHE_KEY));

Vue.use(Vuex);

export default new Vuex.Store({
  strict: true,
  state: {
    user: credentials.user,
    token: credentials.token
  },
  mutations: {
    authenticate: (state, payload) => {
      setCredentialCache(payload);

      state.user = payload.user;
      state.token = payload.token;
    },
    clear: state => {
      setCredentialCache();

      state.user = undefined;
      state.token = '';
    }
  },
  actions: {
    authenticate: (context, payload) => context.commit('authenticate', payload),
    clear: context => context.commit('clear')
  },
  getters: {
    token: state => state.token,
    user: state => state.user
  }
});
