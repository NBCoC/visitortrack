<template>
  <section>
 
      <input class="input" type="text" placeholder="Search members / visitors...">
  
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
     
  </section>
</template>
<script>
export default {
  data() {
    return {
      filter: '',
      dataSource: [
        {
          id: 1,
          fullName: 'Jose Diaz',
          statusName: 'Active',
          ageGroupName: 'Unknown'
        },
        {
          id: 2,
          fullName: 'Liz Howell',
          statusName: 'Member',
          ageGroupName: '60+'
        },
        {
          id: 3,
          fullName: 'Tony Starks',
          statusName: 'Inactive',
          ageGroupName: '29 - 39'
        }
      ]
    };
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
