import template from './sign-in.html';
import { authenticate } from '../visitor-track-api';

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

      const model = {
        emailAddress: this.email,
        password: this.password
      };

      authenticate(model).then(result => {
        this.$store.dispatch('authenticate', {
          user: result.user,
          token: result.token
        });
        this.$router.push('/home');
      });
    }
  }
};
