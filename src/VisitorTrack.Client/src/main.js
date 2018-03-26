import 'material-design-lite';
import 'dialog-polyfill';
import 'mdl-selectfield';
import 'moment';
import 'chart';

import bluebird from 'bluebird';
import Vue from 'vue';
import router from './router';
import store from './store';
import App from './app';
/*
// Configure Bluebird Promises.
Promise.config({
  warnings: {
    wForgottenReturn: false
  }
});
*/

new Vue({
  el: 'visitor-track',
  router,
  store,
  render: h => h(App)
});
