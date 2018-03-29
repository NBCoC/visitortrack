<template>
    <div>
      <navbar v-show="isAuthenticated" />
      <div class="container">
        <router-view />
      </div>
      <prompt-dialog />
    </div>
</template>
<script>
import Vue from 'vue';
import CurrentUser from './mixins/current-user';
import ToggleNavbarBurger from './directives/toggle-navbar-burger';
import Navbar from './components/navbar.vue';
import PromptDialog from './components/prompt-dialog.vue';

Vue.mixin(CurrentUser);
Vue.directive('toggle-navbar-burger', ToggleNavbarBurger);

export default {
  components: {
    navbar: Navbar,
    'prompt-dialog': PromptDialog
  }
};
</script>
<style scoped></style>
