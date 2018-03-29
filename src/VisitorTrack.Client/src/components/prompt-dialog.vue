<template>
  <div class="modal" :class="{ 'is-active': displayDialog }">
    <div class="modal-background"></div>
    <div class="modal-card">
      <header class="modal-card-head">
        <p class="modal-card-title">{{ title }}</p>
      </header>
      <section class="modal-card-body">
        <h3>{{ message }}</h3>
      </section>
      <footer class="modal-card-foot">
        <button class="button is-success">Save changes</button>
        <button class="button" @click="cancel">Cancel</button>
      </footer>
    </div>
  </div>
</template>
<script>
import { Bus, PromptEvent } from '../bus';

export default {
  created() {
    const that = this;

    that.show = false;
    Bus.$on(PromptEvent, () => {
      that.show = true;
    });
  },
  destroyed() {
    Bus.$off(PromptEvent);
  },
  data() {
    return {
      show: false,
      title: 'Prompt',
      message: ''
    };
  },
  computed: {
    displayDialog() {
      return this.show;
    }
  },
  methods: {
    cancel() {
      this.show = false;
    }
  }
};
</script>
<style scoped>

</style>


