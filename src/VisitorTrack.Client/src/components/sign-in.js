import template from './sign-in.html';
import { authenticate } from '../api';

export default {
  template,
  data() {
    return {
      email: '',
      password: ''
    };
  },
  methods: {
    signIn() {
      if (!this.email || !this.password) return;

      const model = {
        emailAddress: this.email,
        password: this.password
      };

      authenticate(model).then(result => {
        this.$store.dispatch('authenticate', result);
        this.$router.push('/home');
      });
    }
  }
};
