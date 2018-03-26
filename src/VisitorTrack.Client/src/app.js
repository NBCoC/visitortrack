import template from './app.html';
import Vue from 'vue';
import { Mdl, ToggleMdlDrawer } from './directives/mdl';

Vue.directive('mdl', Mdl);
Vue.directive('toggle-mdl-drawer', ToggleMdlDrawer);

export default {
  template,
  created() {
    const that = this;

    that.$router.afterEach(to => (that.viewName = to.name));

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

    that.viewName = that.$router.history.current.name;

    if (!that.$store.getters.user) {
      that.$router.push('/sign-in');
    }
  },
  data() {
    return {
      viewName: 'Home'
    };
  },
  computed: {
    displayNavbar() {
      return this.$store.getters.user !== undefined;
    },
    user() {
      return this.$store.getters.user || {};
    },
    displayUserLink() {
      const user = this.$store.getters.user;
      return user && user.roleName === 'Admin';
    },
    currentView() {
      return this.viewName;
    }
  },
  methods: {
    signOut() {
      this.$store.dispatch('clear');
      this.$router.push('/sign-in');
    },
    changePassword() {
      Bus.$emit(ChangePasswordEvent);
    }
  }
};
