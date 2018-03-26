<template>
  <section>
    <div class="mdl-grid">
      <div class="mdl-cell mdl-layout-spacer mdl-cell--4-col mdl-cell--2-col-tablet"></div>
      <div class="mdl-cell mdl-cell--4-col mdl-cell--4-col-tablet">
        <div class="mdl-card mdl-shadow--2dp full-width">
          <form @submit.prevent="signIn">
            <div class="mdl-card__title mdl-card--expand">
              <h2 class="mdl-card__title-text">Visitor-Track</h2>
            </div>
            <div class="mdl-card__supporting-text mdl-color-text--grey-600">
              <div v-mdl:textfield class="mdl-textfield--floating-label full-width">
                <input class="mdl-textfield__input" type="email" id="emailControl" name="emailControl" v-model="email" required>
                <label class="mdl-textfield__label" for="emailControl">Email...</label>
              </div>
              <div v-mdl:textfield class="mdl-textfield--floating-label full-width">
                <input class="mdl-textfield__input" type="password" id="passwordControl" name="passwordControl" v-model="password" required>
                <label class="mdl-textfield__label" for="passwordControl">Password...</label>
              </div>
            </div>
            <div class="mdl-card__actions mdl-card--border">
              <input v-mdl:button type="submit" class="mdl-button--colored mdl-js-ripple-effect full-width" value="Sign In" />
            </div>
          </form>
        </div>
      </div>
      <div class="mdl-cell mdl-layout-spacer mdl-cell--4-col mdl-cell--2-col-tablet"></div>
    </div>
  </section>
</template>
<script>
  import { authenticate } from '../api';

  export default {
    data() {
      return {
        email: '',
        password: '',
        isWorking: false
      };
    },
    computed: {
      isBusy() {
        return this.isWorking;
      }
    },
    methods: {
      signIn() {
        const that = this;

        if (!that.email || !that.password) return;

        const model = {
          emailAddress: that.email,
          password: that.password
        };

        that.isWorking = true;

        authenticate(model)
          .then(result => {
            that.isWorking = false;
            that.$store.dispatch('authenticate', result);
            that.$router.push('/home');
          })
          .catch(error => {
            that.isWorking = false;
          });
      }
    }
  };

</script>
<style scoped></style>
