import Vue from 'Vue';
import Navbar from './components/navbar';
import ToggleNavbarBurger from './directives/toggle-navbar-burger';

Vue.component('navbar', Navbar);
Vue.directive('toggle-navbar-burger', ToggleNavbarBurger);

export default {
  template: `
  <div class="container">
    <router-view></router-view>
  </div>
  `,
  created: function() {
    const that = this;

    that.$router.beforeEach((to, from, next) => {
      const isAuthenticated = that.$store.getters.isAuthenticated;

      if (to.path === '/sign-in') {
        isAuthenticated ? next(from.path) : next();
      } else if (!isAuthenticated) {
        next('/sign-in');
      } else {
        next();
      }
    });

    that.$store.getters.isAuthenticated
      ? that.$router.push('/home')
      : that.$router.push('/sign-in');
  }
};
