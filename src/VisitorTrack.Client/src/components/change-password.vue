<template>
  <section>
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
                      <input class="input" type="password" placeholder="Current Password..." v-model="model.oldPassword">
                    </div>
                  </div>

                  <div class="field">
                    <label class="label">New Password</label>
                    <div class="control">
                      <input class="input" type="password" placeholder="New Password..." v-model="model.newPassword">
                    </div>
                  </div>

                  <div class="field">
                    <label class="label">Confirm Password</label>
                    <div class="control">
                      <input class="input" type="password" placeholder="Confirm Password..." v-model="model.confirmPassword">
                    </div>
                  </div>

                  <div class="field">
                    <button class="button is-primary full-width" type="submit">
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
  </section>
</template>
<script>
export default {
  data() {
    return {
      model: {
        oldPassword: '',
        newPassword: '',
        confirmPassword: ''
      }
    };
  },
  methods: {
    save() {}
  }
};
</script>
<style scoped>

</style>


