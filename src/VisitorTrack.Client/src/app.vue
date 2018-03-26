<template>
  <div v-mdl:layout class="mdl-layout--fixed-header">
    <navbar v-show="isAuthenticated"></navbar>
    <main class="mdl-layout__content">
      <router-view></router-view>
    </main>
  </div>
</template>
<script>
import Vue from 'vue';
import CurrentUser from './mixins/current-user';
import { Mdl, ToggleMdlDrawer } from './directives/mdl';
import Navbar from './components/navbar.vue';

Vue.mixin(CurrentUser);
Vue.directive('mdl', Mdl);
Vue.component('navbar', Navbar);

export default {};
</script>
<style scoped></style>
