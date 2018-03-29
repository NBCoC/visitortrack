<template>
  <nav class="navbar is-light">
    <div class="navbar-brand">
      <router-link class="navbar-item has-text-weight-bold" to="/home">
        Visitor-Track
      </router-link>
      <div class="navbar-burger burger" data-target="main-navbar" v-toggle-navbar-burger>
        <span></span>
        <span></span>
        <span></span>
      </div>
    </div>

    <div id="main-navbar" class="navbar-menu">
      <div class="navbar-start">
        <div class="navbar-item">
          <router-link to="/search">
            <i class="fa fa-search"></i> Search
          </router-link>
        </div>
        <div class="navbar-item" v-if="isAdminUser">
          <router-link to="/admin/users">
            <i class="fa fa-users"></i> Users
          </router-link>
        </div>
      </div>

      <div class="navbar-end">
        <div class="navbar-item has-dropdown is-hoverable">
          <div class="navbar-link">
            <span>
              <i class="fa fa-user"></i> {{ user.displayName }}</span>
          </div>
          <div class="navbar-dropdown is-boxed">
            <div class="navbar-item">
              <router-link to="/user/change-password">
                <span>
                  <i class="fa fa-key"></i> Change Password
                </span>
              </router-link>
            </div>
            <hr class="navbar-divider">
            <div class="navbar-item">
              <a @click="signOut">
                <span>
                  <i class="fa fa-sign-out"></i> Sign Out
                </span>
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>
<script>
import Vue from 'vue';

export default {
  created() {
    const that = this;

    that.$router.beforeEach((to, from, next) => {
      if (to.path === '/sign-in') {
        that.token ? next(from.path) : next();
      } else if (!that.token) {
        next('/sign-in');
      } else if (to.meta.adminView && !that.isAdminUser) {
        next(false);
      } else {
        next();
      }
    });

    if (!that.token) {
      that.$router.push('/sign-in');
    }
  },
  methods: {
    signOut() {
      this.$store.dispatch('clear');
      this.$router.push('/sign-in');
    }
  }
};
</script>
<style scoped></style>
