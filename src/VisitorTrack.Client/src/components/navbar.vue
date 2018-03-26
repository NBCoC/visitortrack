<template>
  <header class="mdl-layout__header mdl-color--grey-100 mdl-color-text--grey-700">
    <div class="mdl-layout__header-row">
      <div class="mdl-navigation-text">Visitor-Track | {{ currentView }}</div>
      <div class="mdl-layout-spacer"></div>

      <router-link id="home-link" class="mdl-navigation__link mdl-color-text--grey-600" to="home">
        <i class="material-icons" role="presentation">home</i>
      </router-link>
      <span v-mdl:tooltip class="mdl-tooltip--large" data-mdl-for="home-link">
        Home
      </span>

      <router-link id="search-link" class="mdl-navigation__link mdl-color-text--grey-600" to="search">
        <i class="material-icons" role="presentation">search</i>
      </router-link>
      <span v-mdl:tooltip class="mdl-tooltip--large" data-mdl-for="search-link">
        Search
      </span>

      <router-link id="users-link" class="mdl-navigation__link mdl-color-text--grey-600" to="users" v-if="isAdminUser">
        <i class="material-icons" role="presentation">people</i>
      </router-link>
      <span v-mdl:tooltip class="mdl-tooltip--large" data-mdl-for="users-link">
        User Administration
      </span>

      <router-link id="psw-link" class="mdl-navigation__link mdl-color-text--grey-600" to="users">
        <i class="material-icons" role="presentation">lock</i>
      </router-link>
      <span v-mdl:tooltip class="mdl-tooltip--large" data-mdl-for="psw-link">
        Change Password
      </span>

      <a href="#/home" id="logout-link" class="mdl-navigation__link mdl-color-text--grey-600" @click.stop="signOut">
        <i class="material-icons" role="presentation">exit_to_app</i>
      </a>
      <span v-mdl:tooltip class="mdl-tooltip--large" data-mdl-for="logout-link">
        Sign Out
      </span>

      <div class="mdl-navigation-text">{{ user.displayName }}</div>
    </div>
  </header>
</template>
<script>
import Vue from 'vue';

export default {
  created() {
    const that = this;

    that.$router.afterEach(to => (that.viewName = to.name));

    that.$router.beforeEach((to, from, next) => {
      if (to.path === '/sign-in') {
        that.user ? next(from.path) : next();
      } else if (!that.user) {
        next('/sign-in');
      } else if (to.meta.adminView && !that.isAdminUser) {
        next(false);
      } else {
        next();
      }
    });

    that.viewName = that.$router.history.current.name;

    if (!that.user) {
      that.$router.push('/sign-in');
    }
  },
  data() {
    return {
      viewName: 'Home'
    };
  },
  computed: {
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
</script>
<style scoped>
.mdl-navigation-text {
  font-weight: bold;
  padding-top: 3px;
}
</style>
