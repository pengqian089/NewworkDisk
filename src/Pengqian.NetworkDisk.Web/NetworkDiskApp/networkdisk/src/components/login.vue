<template>
  <div>
    <label for="account">帐号</label><input id="account" type="text"/>
    <label for="password">帐号</label><input id="password" type="password"/>
    <button type="button" v-on:click="login">登录</button>
  </div>
</template>

<script>
export default {
  name: "login.vue",
  methods:{
    login:function(event){
      console.log(event);
    }
  }
}
</script>

<style scoped>

</style>