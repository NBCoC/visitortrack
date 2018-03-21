import Vue from 'Vue';
import Vuex from 'Vuex';

Vue.use(Vuex);

const store = new Vuex.Store({
  strict: true,
  state: {
    user: {},
    token: ''
  },
  mutations: {
    authenticate: (state, payload) => {
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

export default store;
