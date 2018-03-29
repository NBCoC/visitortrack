<template>
  <div class="page">
    <div class="columns is-mobile">
      <div class="column"></div>
      <div class="column is-three-quarters-mobile">
        <div class="card">
            <form @submit.prevent="save">
              <header class="card-header">
                <p class="card-header-title">
                  Change Password
                </p>
              </header>
              <div class="card-content">
                <div class="content">
                  <div class="field">
                    <label class="label">Current Password</label>
                    <div class="control">
                      <input class="input" type="password" placeholder="Current Password..." v-model="oldPassword">
                    </div>
                  </div>

                  <div class="field">
                    <label class="label">New Password</label>
                    <div class="control">
                      <input class="input" type="password" placeholder="New Password..." v-model="newPassword">
                    </div>
                  </div>

                  <div class="field">
                    <label class="label">Confirm Password</label>
                    <div class="control">
                      <input class="input" type="password" placeholder="Confirm Password..." v-model="confirmPassword">
                    </div>
                  </div>

                  <div class="field">
                    <button class="button is-primary full-width" type="submit" :class="{ 'is-loading' : isBusy }">
                      <span>
                        <i class="fa fa-save"></i> Save
                      </span>
                    </button>
                  </div>
                </div>
              </div>
            </form>
          </div>
      </div>
      <div class="column"></div>
    </div>
  </div>
</template>
<script>
import { updatePassword } from '../api';
import { alert, apiError } from '../bus';

export default {
  data() {
    return {
      oldPassword: '',
      newPassword: '',
      confirmPassword: ''
    };
  },
  methods: {
    save() {
      const that = this;

      if (!that.oldPassword || !that.newPassword || !that.confirmPassword)
        return;

      if (that.oldPassword === that.newPassword) {
        alert('Change Password', 'New password cannot be the same as current.');
        return;
      }

      if (that.newPassword !== that.confirmPassword) {
        alert('Change Password', 'Confirm password must match new password.');
        return;
      }

      const model = {
        oldPassword: that.oldPassword,
        newPassword: that.newPassword
      };

      that.isWorking = true;
      updatePassword(that.token, that.user.id, model)
        .then(() => {
          that.isWorking = false;
          alert('Change Password', 'Your password has been changed!');
        })
        .catch(error => {
          that.isWorking = false;
          apiError(error);
        });
    }
  }
};
</script>
<style scoped>

</style>


