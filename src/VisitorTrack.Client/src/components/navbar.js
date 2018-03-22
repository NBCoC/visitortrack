import template from './navbar.html';

export default {
  template,
  computed: {
    user() {
      return this.$store.getters.user;
    },
    displayUserLink() {
      return this.$store.getters.user.roleName === 'Admin';
    }
  },
  methods: {
    signOut() {
      this.$store.dispatch('clear');
      this.$router.push('/sign-in');
    }
  }
};
