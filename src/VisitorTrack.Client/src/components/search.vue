<template>
  <div class="page">
      <form @submit.prevent="search">
        <input type="text" class="input" placeholder="Search members / visitors..." v-model="filterText">
      </form>
      
      <table class="table is-bordered is-striped is-hoverable is-fullwidth" v-show="dataSource.length">
        <thead>
          <tr>
            <th>Name</th>
            <th>Status</th>
            <th>Age Group</th>
            <th class="action-items"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in dataSource" :key="item.id">
            <td>{{ item.fullName }}</td>
            <td>{{ item.statusName }}</td>
            <td>{{ item.ageGroupName }}</td>
            <td>
              <router-link class="button is-primary is-small" to="home">
                View Details
              </router-link>
            </td>
          </tr>
        </tbody>
      </table>
     
  </div>
</template>
<script>
import { searchVisitor } from '../api';
import { apiError } from '../bus';

export default {
  data() {
    return {
      filterText: '',
      dataSource: []
    };
  },
  methods: {
    search() {
      const that = this;
      searchVisitor(that.token, that.filterText || '')
        .then(result => (that.dataSource = result))
        .catch(apiError);
    }
  }
};
</script>
<style scoped>
th.action-items {
  width: 100px;
}
input.input {
  margin-bottom: 10px !important;
}
</style>
