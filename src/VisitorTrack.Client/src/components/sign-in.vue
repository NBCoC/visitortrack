<template>
  <section>
    <div sign-in-page class="columns is-mobile">
      <div class="column"></div>
      <div class="column is-three-quarters-mobile">
        <form @submit.prevent="signIn">
          <div class="card">
            <header class="card-header">
              <div class="card-header-title is-centered">
                Visitor-Track
              </div>
            </header>
            <div class="card-content">
              <div class="content">
                <div class="field">
                  <div class="control has-icons-left">
                    <input class="input" type="email" placeholder="Email" v-model="email">
                    <span class="icon is-small is-left">
                      <i class="fa fa-envelope"></i>
                    </span>
                  </div>
                </div>

                <div class="field">
                  <div class="control has-icons-left">
                    <input class="input" type="password" placeholder="Password" v-model="password">
                    <span class="icon is-small is-left">
                      <i class="fa fa-key"></i>
                    </span>
                  </div>
                </div>

                <div class="field">
                  <button class="button is-primary full-width" type="submit">
                    <span>
                      <i class="fa fa-sign-in"></i> Sign In
                    </span>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </form>
      </div>
      <div class="column"></div>
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
<style scoped>
[sign-in-page] {
  padding-top: 50px;
}

[sign-in-page] header.card-header div.card-header-title {
  font-size: 30px;
}
</style>
