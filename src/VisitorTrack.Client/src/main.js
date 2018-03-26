import '../node_modules/material-design-icons/iconfont/material-icons.css';
import '../node_modules/material-design-lite/dist/material.css';
import '../node_modules/mdl-selectfield/dist/mdl-selectfield.css';
import '../node_modules/dialog-polyfill/dialog-polyfill.css';
import './styles/visitor-track.css';

import 'material-design-lite';
import 'mdl-selectfield';
import 'dialog-polyfill';
//import * as moment from 'moment';
import 'chart';

import Promise from 'bluebird';
import Vue from 'vue';
import router from './router';
import store from './store';
import App from './app.vue';

new Vue({
  el: 'visitor-track',
  router,
  store,
  render: h => h(App)
});
