import template from './navbar.html';

export default {
  template,
  computed: {
    user() {
      return this.$store.getters.user;
    }
  }
};
