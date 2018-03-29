<template>
  <div class="page">
    {{ message }}
  </div>
</template>
<script>
  export default {
    data() {
      return {
        message: 'Home Page'
      };
    },
    methods: {
      sayHi() {
        alert(this.message);
      }
    }
  };

</script>
<style scoped></style>
