import template from './sign-in.html';
import { authenticate, saveCredentialCache } from '../visitor-track-api';

export default {
  template,
  data: () => {
    return {
      email: '',
      password: ''
    };
  },
  methods: {
    signIn: function() {
      if (!this.email || !this.password) return;

      authenticate(this.email, this.password).then(result => {
        saveCredentialCache(result);
        this.$store.dispatch('authenticate', {
          user: result.user,
          token: result.token
        });
        this.$router.push('/');
      });
    }
  }
};
