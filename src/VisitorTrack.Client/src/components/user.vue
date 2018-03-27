<template>
  <section>
    <div class="mdl-grid">
      <div class="mdl-cell mdl-layout-spacer mdl-cell--4-col mdl-cell--2-col-tablet"></div>
      <div class="mdl-cell mdl-cell--4-col mdl-cell--4-col-tablet">
        <div class="mdl-card mdl-shadow--2dp full-width">
          <form>
              <div class="mdl-card__title mdl-card--expand">
                <h2 class="mdl-card__title-text">User</h2>
              </div>
              <div class="mdl-card__supporting-text mdl-color-text--grey-600">
                <div v-mdl:textfield class="mdl-textfield--floating-label full-width">
                  <input class="mdl-textfield__input" type="email" id="emailControl" name="emailControl" 
                         v-model="model.emailAddress" required>
                  <label class="mdl-textfield__label" for="emailControl">Email...</label>
                </div>

                <div v-mdl:textfield class="mdl-textfield--floating-label full-width">
                  <input class="mdl-textfield__input" type="text" id="displayNameControl" name="displayNameControl" 
                         v-model="model.displayName" required>
                  <label class="mdl-textfield__label" for="displayNameControl">Display Name...</label>
                </div>

                <div v-mdl:selectfield class="mdl-selectfield--floating-label full-width">
                    <select id="roleControl" name="roleControl" class="mdl-selectfield__select" 
                            v-model="model.roleId">
                      <option v-for="role in roles" :key="role.id" v-bind:value="role.id">{{ role.name }}</option>
                    </select>
                  <label class="mdl-selectfield__label" for="roleControl">Role...</label>
                </div>
              </div>
              <div class="mdl-card__actions mdl-card--border">
                <input v-mdl:button type="submit" 
                       class="mdl-button--colored mdl-js-ripple-effect full-width" 
                       value="Save" />
              </div>
          </form>
        </div>
      </div>
      <div class="mdl-cell mdl-layout-spacer mdl-cell--4-col mdl-cell--2-col-tablet"></div>
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
  methods: {
    getModel() {
      const that = this;
      const id = that.$route.params.id;
      if (id) {
        getUser(that.token, id).then(data => {
          that.model = data;
          setTimeout(() => that.refreshMdl(that.$el));
        });
      }
    },
    getRoles() {
      const that = this;
      getUserRoles(that.token).then(data => {
        that.roles = data;
      });
    }
  }
};
</script>
<style scoped>

</style>


