<template>
  <section>
    <div class="columns is-mobile">
      <div class="column"></div>
      <div class="column is-three-quarters-mobile">
        <div class="card">
            <form @submit.prevent="save">
              <header class="card-header">
                <p class="card-header-title">
                  {{ title }}
                </p>
              </header>
              <div class="card-content">
                <div class="content">
                  <div class="field">
                    <label class="label">Email</label>
                    <div class="control">
                      <input class="input" type="email" placeholder="Email..." v-model="model.emailAddress" :readonly="isReadonly">
                    </div>
                  </div>

                  <div class="field">
                    <label class="label">Display Name</label>
                    <div class="control">
                      <input class="input" type="text" placeholder="Display Name..." v-model="model.displayName">
                    </div>
                  </div>

                  <div class="field">
                    <label class="label">Role</label>
                    <div class="control">
                      <div class="select full-width">
                        <select v-model="model.roleId" class="full-width">
                          <option v-for="role in roles" :key="role.id" :value="role.id">{{ role.name }}</option>
                        </select>
                      </div>
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
import { getUser, getUserRoles } from '../api';
export default {
  created() {
    this.getRoles();
    this.getModel();
  },
  data() {
    return {
      model: {},
      roles: []
    };
  },
  props: ['id'],
  computed: {
    title() {
      return this.model.id ? 'Edit User' : 'New User';
    },
    isReadonly() {
      return this.model.id !== undefined;
    }
  },
  methods: {
    getModel() {
      const that = this;
      const id = that.$route.params.id;
      if (id) {
        getUser(that.token, id).then(data => {
          that.model = data;
        });
      }
    },
    getRoles() {
      const that = this;
      getUserRoles(that.token).then(data => {
        that.roles = data;
      });
    },
    save() {
      if (
        !this.model.emailAddress ||
        !this.model.displayName ||
        !this.model.roleId
      )
        return;
    }
  }
};
</script>
<style scoped>

</style>


