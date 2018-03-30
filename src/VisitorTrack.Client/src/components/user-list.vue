<template>
  <div class="page">
    <table class="table is-bordered is-striped is-hoverable is-fullwidth" v-show="dataSource.length">
      <thead>
        <tr>
          <th>Name</th>
          <th>Email</th>
          <th>Role</th>
          <th class="action-items">
            <router-link class="button is-success is-small" to="/admin/user">
              <i class="fa fa-plus"></i>
            </router-link>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in dataSource" :key="item.id">
          <td>{{ item.displayName }}</td>
          <td>{{ item.emailAddress }}</td>
          <td>{{ item.roleName }}</td>
          <td>
            <router-link class="button is-info is-small" :to="{ name: 'User', params: { id: item.id }}">
              <i class="fa fa-edit"></i>
            </router-link>

            <button class="button is-warning is-small" v-show="isAdminUser" @click="resetPassword(item)">
              Reset Password
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
<script>
import { getUsers, resetPassword } from '../api';
import { apiError, alert, confirm } from '../bus';

export default {
  created() {
    this.load();
  },
  data() {
    return {
      dataSource: []
    };
  },
  methods: {
    load() {
      const that = this;
      getUsers(that.token)
        .then(data => {
          that.dataSource = data;
        })
        .catch(apiError);
    },
    resetPassword(model) {
      if (!model) return;

      const that = this;
      const callback = () => {
        resetPassword(that.token, that.user.id, model.id)
          .then(() =>
            alert(
              'Visitor-Track',
              `${model.displayName}'s password has been reset!`
            )
          )
          .catch(error => apiError);
      };

      confirm(
        `Reset ${model.displayName}'s password`,
        "Are you sure you want to reset this user's password?",
        callback
      );
    }
  }
};
</script>
<style scoped>
th.action-items {
  width: 165px;
}
</style>
