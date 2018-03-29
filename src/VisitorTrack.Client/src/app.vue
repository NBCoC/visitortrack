<template>
    <div>
      <navbar v-show="isAuthenticated" />
      <div class="container">
        <router-view />
      </div>
      <modal />
    </div>
</template>
<script>
import Vue from 'vue';
import ContextUser from './mixins/context-user';
import TaskHelper from './mixins/task-helper';
import ToggleNavbarBurger from './directives/toggle-navbar-burger';
import Navbar from './components/navbar.vue';
import Modal from './components/modal.vue';

Vue.mixin(ContextUser);
Vue.mixin(TaskHelper);
Vue.directive('toggle-navbar-burger', ToggleNavbarBurger);

export default {
  components: {
    navbar: Navbar,
    modal: Modal
  }
};
</script>
<style scoped></style>
