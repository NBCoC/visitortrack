<template>
    <div>
      <navbar v-show="isAuthenticated" />
      <div class="container">
        <router-view />
      </div>
    </div>
</template>
<script>
import Vue from 'vue';
import CurrentUser from './mixins/current-user';
import ToggleNavbarBurger from './directives/toggle-navbar-burger';
import Navbar from './components/navbar.vue';

Vue.mixin(CurrentUser);
Vue.directive('toggle-navbar-burger', ToggleNavbarBurger);
Vue.component('navbar', Navbar);

export default {};
</script>
<style scoped></style>
