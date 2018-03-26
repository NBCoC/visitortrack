<template>
  <section>
    <div class="mdl-grid scrollable">
      <div class="mdl-layout-spacer"></div>
      <table v-mdl:data-table class="mdl-shadow--2dp" v-show="dataSource.length">
        <thead>
          <tr>
            <th>Name</th>
            <th>Email</th>
            <th>Role</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in dataSource" :key="item.id">
            <td>{{ item.displayName }}</td>
            <td>{{ item.emailAddress }}</td>
            <td>{{ item.roleName }}</td>
            <td>
              <router-link v-mdl:button class="mdl-button--colored" to="home">
                Edit
              </router-link>

              <button v-mdl:button class="mdl-button--accent">
                Delete
              </button>

              <button v-mdl:button to="home" v-show="isAdminUser">
                Reset Password
              </button>
            </td>
          </tr>
        </tbody>
      </table>
      <div class="mdl-layout-spacer"></div>
    </div>
    <button v-mdl:button type="button" class="mdl-button--raised mdl-js-ripple-effect mdl-button--primary add-btn" v-show="isAdminUser">
      New User
    </button>
  </section>
</template>
<script>
import { getUsers } from '../api';

export default {
  created() {
    const that = this;
    const token = that.$store.getters.token;
    getUsers(token).then(data => {
      that.dataSource = data;
    });
  },
  data() {
    return {
      message: 'Users Page',
      dataSource: []
    };
  }
};
</script>
<style scoped></style>
