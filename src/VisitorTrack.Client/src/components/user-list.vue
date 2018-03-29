<template>
  <section>
    <table class="table is-bordered is-striped is-hoverable is-fullwidth" v-show="dataSource.length">
      <thead>
        <tr>
          <th>Name</th>
          <th>Email</th>
          <th>Role</th>
          <th class="action-items">
            <router-link class="button is-success is-small" :to="{ name: 'User' }">
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

            <button class="button is-danger is-small">
              <i class="fa fa-trash"></i>
            </button>

            <button class="button is-warning is-small" to="home" v-show="isAdminUser">
              Reset Password
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </section>
</template>
<script>
import { getUsers } from '../api';

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
      getUsers(that.token).then(data => {
        that.dataSource = data;
      });
    }
  }
};
</script>
<style scoped>
th.action-items {
  width: 200px;
}
</style>
