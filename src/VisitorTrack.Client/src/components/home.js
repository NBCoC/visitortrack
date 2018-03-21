import template from './home.html';

export default {
  template,
  data() {
    return {
      message: 'Home Page'
    };
  },
  methods: {
    sayHi: function() {
      alert(this.message);
    }
  }
};
