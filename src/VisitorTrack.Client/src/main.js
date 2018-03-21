import Vue from 'Vue';
import router from './router';
import store from './store/visitor-track-store';
import App from './app';

new Vue({
  el: 'visitor-track',
  router,
  store,
  render: h => h(App)
});
