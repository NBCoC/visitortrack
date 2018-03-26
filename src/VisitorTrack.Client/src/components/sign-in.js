import template from './sign-in.html';
import { authenticate } from '../api';

export default {
  template,
  data() {
    return {
      email: '',
      password: '',
      isWorking: false
    };
  },
  computed: {
    isBusy() {
      return this.isWorking;
    }
  },
  methods: {
    signIn() {
      if (!this.email || !this.password) return;

      const model = {
        emailAddress: this.email,
        password: this.password
      };

      this.isWorking = true;

      authenticate(model)
        .then(result => {
          this.isWorking = false;
          this.$store.dispatch('authenticate', result);
          this.$router.push('/home');
        })
        .catch(error => {
          this.isWorking = false;
        });
    }
  }
};
