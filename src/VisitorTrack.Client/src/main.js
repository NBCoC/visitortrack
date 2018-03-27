import '../node_modules/bulma/css/bulma.css';
import '../node_modules/font-awesome/css/font-awesome.css';
import './styles.css';

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
