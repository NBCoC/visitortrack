import Vue from 'vue';
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
  created() {
    const that = this;

    that.$router.beforeEach((to, from, next) => {
      const user = that.$store.getters.user;

      if (to.path === '/sign-in') {
        user ? next(from.path) : next();
      } else if (!user) {
        next('/sign-in');
      } else if (to.meta.adminView && user.roleName !== 'Admin') {
        next(false);
      } else {
        next();
      }
    });

    that.$store.getters.user
      ? that.$router.push('/home')
      : that.$router.push('/sign-in');
  }
};
