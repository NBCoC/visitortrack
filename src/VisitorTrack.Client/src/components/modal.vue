<template>
  <div class="modal" :class="{ 'is-active': show }">
    <div class="modal-background"></div>
    <div class="modal-card">
      <header class="modal-card-head">
        <p class="modal-card-title">{{ title }}</p>
      </header>
      <section class="modal-card-body">
        <h3>{{ message }}</h3>
      </section>
      <footer class="modal-card-foot">
        <button class="button is-primary" @click="save">
          <span>
            <i :class="saveBtnIcon"></i> {{ saveBtnText }}
          </span>
        </button>
        <button class="button" @click="cancel" v-show="!isAlert">
          <span>
            <i :class="cancelBtnIcon"></i> {{ cancelBtnText }}
          </span>
        </button>
      </footer>
    </div>
  </div>
</template>
<script>
import { Bus, DialogEvent } from '../bus';

export default {
  created() {
    const that = this;

    Bus.$on(DialogEvent, data => {
      that.title = data.title;
      that.message = data.message;
      that.callback = data.callback;
      that.saveBtnText = data.saveBtnText || 'Save';
      that.saveBtnIcon = data.saveBtnIcon || '';
      that.cancelBtnText = data.cancelBtnText || 'Cancel';
      that.cancelBtnIcon = data.cancelBtnIcon || '';
      that.isAlert = data.isAlert || false;
      that.show = true;
    });
  },
  destroyed() {
    Bus.$off(DialogEvent);
  },
  data() {
    return {
      show: false,
      isAlert: false,
      title: '',
      message: '',
      saveBtnText: 'Save',
      saveBtnIcon: '',
      cancelBtnText: 'Cancel',
      cancelBtnIcon: '',
      callback: undefined
    };
  },
  methods: {
    cancel() {
      this.show = false;
    },
    save() {
      this.show = false;
      if (this.callback) {
        this.callback();
      }
    }
  }
};
</script>
<style scoped></style>


