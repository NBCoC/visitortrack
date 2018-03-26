<template>
  <section>
    <div class="mdl-grid">
      <div class="mdl-layout-spacer"></div>
      <div v-mdl:textfield class="mdl-textfield--floating-label">
        <input class="mdl-textfield__input" type="text" id="searchControl" name="searchControl">
        <label class="mdl-textfield__label" for="searchControl">Search members / visitors...</label>
      </div>
      <div class="mdl-layout-spacer"></div>
    </div>
    <div class="mdl-grid scrollable">
      <div class="mdl-layout-spacer"></div>
      <span class="loading-text" v-show="!dataSource.length">
        No results found...
      </span>
      <table v-mdl:data-table class="mdl-shadow--2dp" v-show="dataSource.length">
        <thead>
          <tr>
            <th>Name</th>
            <th>Status</th>
            <th>Age Group</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in dataSource" :key="item.id">
            <td>{{ item.fullName }}</td>
            <td>{{ item.statusName }}</td>
            <td>{{ item.ageGroupName }}</td>
            <td>
              <router-link v-mdl:button class="mdl-button--accent" to="home">
                View Details
              </router-link>
            </td>
          </tr>
        </tbody>
      </table>
      <div class="mdl-layout-spacer"></div>
    </div>
  </section>
</template>
<script>

  export default {
    data() {
      return {
        filter: '',
        dataSource: [
          { id: 1, fullName: 'Jose Diaz', statusName: 'Active', ageGroupName: 'Unknown' },
          { id: 2, fullName: 'Liz Howell', statusName: 'Member', ageGroupName: '60+' },
          { id: 3, fullName: 'Tony Starks', statusName: 'Inactive', ageGroupName: '29 - 39' }
        ]
      };
    }
  };

</script>
<style scoped></style>
