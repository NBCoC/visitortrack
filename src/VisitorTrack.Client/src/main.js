import Vue from 'vue';
import router from './router';
import store from './store';
import App from './app';

new Vue({
  el: 'visitor-track',
  router,
  store,
  render: h => h(App)
});
