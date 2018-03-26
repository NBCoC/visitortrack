import template from './change-psw-dialog.html';
import { Bus, ChangePasswordEvent } from '../bus';

export default {
  template,
  created() {
    const that = this;
    Bus.$on(ChangePasswordEvent, () => {
      that.isWorking = false;
      that.displayDialog = true;
    });
  },
  data() {
    return {
      displayDialog: false,
      isWorking: false,
      oldPassword: '',
      newPassword: '',
      confirmPassword: ''
    };
  },
  computed: {
    show() {
      return this.displayDialog;
    },
    isBusy() {
      return this.isWorking;
    }
  },
  methods: {
    save() {
      if (!this.oldPassword || !this.newPassword) return;

      const model = {
        oldPassword: this.oldPassword,
        newPassword: this.newPassword
      };

      this.isWorking = true;
    },
    cancel() {
      this.displayDialog = false;
    }
  }
};
