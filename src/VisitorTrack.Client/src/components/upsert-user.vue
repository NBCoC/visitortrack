<template>
  <div class="page">
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
                    <button type="submit" class="button is-primary full-width" :class="{ 'is-loading' : isBusy }">
                      <span>
                        <i class="fa fa-save"></i> Save
                      </span>
                    </button>
                  </div>

                  <div class="field">
                    <button type="button" class="button is-danger full-width" :class="{ 'is-loading' : isBusy }" @click="remove" v-show="isReadonly">
                      <span>
                        <i class="fa fa-trash"></i> Delete
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
import {
  getUser,
  getUserRoles,
  createUser,
  updateUser,
  deleteUser
} from '../api';

import { confirm, apiError } from '../bus';

export default {
  created() {
    this.getRoles();
    this.getModel();
  },
  data() {
    return {
      model: { roleId: 0 },
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
        getUser(that.token, id)
          .then(data => {
            that.model = data;
          })
          .catch(apiError);
      }
    },
    getRoles() {
      const that = this;
      getUserRoles(that.token)
        .then(data => {
          that.roles = data;
        })
        .catch(apiError);
    },
    save() {
      const that = this;

      if (!that.model.emailAddress || !that.model.displayName) return;

      const model = {
        emailAddress: that.model.emailAddress,
        displayName: that.model.displayName,
        roleId: that.model.roleId
      };

      that.isWorking = true;

      const upsert = that.model.id
        ? updateUser(that.token, that.user.id, that.model.id, model)
        : createUser(that.token, that.user.id, model);

      upsert
        .then(() => {
          that.isWorking = false;
          that.$router.push('/admin/users');
        })
        .catch(error => {
          that.isWorking = false;
          apiError(error);
        });
    },
    remove() {
      const that = this;
      if (!that.isReadonly) return;

      const callback = () => {
        that.isWorking = true;
        deleteUser(that.token, that.user.id, that.model.id)
          .then(() => {
            that.isWorking = false;
            that.$router.push('/admin/users');
          })
          .catch(error => {
            that.isWorking = false;
            apiError(error);
          });
      };

      confirm(
        `Delete ${that.model.displayName}`,
        'Are you sure you want to delete this user?',
        callback
      );
    }
  }
};
</script>
<style scoped></style>


