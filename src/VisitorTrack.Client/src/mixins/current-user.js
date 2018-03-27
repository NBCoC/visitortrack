export default {
  computed: {
    token() {
      return this.$store.getters.token;
    },
    user() {
      return this.$store.getters.user || {};
    },
    isAdminUser() {
      return this.user && this.user.roleName === 'Admin';
    },
    isEditorUser() {
      return (
        this.user &&
        (this.user.roleName === 'Admin' || this.user.roleName === 'Editor')
      );
    },
    isAuthenticated() {
      return this.$store.getters.user !== undefined;
    }
  }
};
