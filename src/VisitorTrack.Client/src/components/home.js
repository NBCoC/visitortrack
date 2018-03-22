import template from './home.html';

export default {
  template,
  data() {
    return {
      message: 'Home Page'
    };
  },
  methods: {
    sayHi() {
      alert(this.message);
    }
  }
};
