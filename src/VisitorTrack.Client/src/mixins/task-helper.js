export default {
  data() {
    return {
      isWorking: false
    };
  },
  computed: {
    isBusy() {
      return this.isWorking;
    }
  }
};
